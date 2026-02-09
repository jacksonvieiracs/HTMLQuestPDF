using System.Collections.Generic;
using HtmlAgilityPack;
using HTMLQuestPDF.Extensions;
using HTMLToQPDF.Components;
using HTMLToQPDF.Utils;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components
{
    internal class ParagraphComponent : IComponent
    {
        private static readonly Dictionary<string, Func<TextStyle, TextStyle>> TagModifiers = new(StringComparer.OrdinalIgnoreCase)
        {
            { "b", s => s.Bold() },
            { "strong", s => s.Bold() },
            { "i", s => s.Italic() },
            { "em", s => s.Italic() },
            { "small", s => s.Light() },
            { "strike", s => s.Strikethrough() },
            { "del", s => s.Strikethrough() },
            { "s", s => s.Strikethrough() },
            { "u", s => s.Underline() },
            { "a", s => s.Underline() },
            { "sup", s => s.Superscript() },
            { "sub", s => s.Subscript() },
            { "h1", s => s.FontSize(24).Bold() },
            { "h2", s => s.FontSize(18).Bold() },
            { "h3", s => s.FontSize(14.04f).Bold() },
            { "h4", s => s.FontSize(12).Bold() },
            { "h5", s => s.FontSize(9.96f).Bold() },
            { "h6", s => s.FontSize(8.04f).Bold() },
            { "p", s => s.FontSize(8) }, // 8 (6pt)
        };

        private readonly List<HtmlNode> lineNodes;
        private readonly HTMLComponentsArgs args;

        public ParagraphComponent(List<HtmlNode> lineNodes, HTMLComponentsArgs args)
        {
            this.lineNodes = lineNodes;
            this.args = args;
        }

        private HtmlNode? GetParrentBlock(HtmlNode node)
        {
            if (node == null) return null;
            return node.IsBlockNode() ? node : GetParrentBlock(node.ParentNode);
        }

        private HtmlNode? GetListItemNode(HtmlNode node)
        {
            if (node == null || node.IsList()) return null;
            return node.IsListItem() ? node : GetListItemNode(node.ParentNode);
        }

        private void ApplyClassTextAlignments(HtmlNode node, TextDescriptor text)
        {
            if (node == null) return;

            // Get classes from this node and apply alignments
            var classes = node.GetClasses();
            foreach (var className in classes)
            {
                if (args.ClassTextAlignments.TryGetValue(className, out var alignment))
                {
                    alignment(text);
                }
            }

            // Apply inline text-align (highest priority, applied after class alignments)
            if (node.IsBlockNode())
            {
                var textAlign = CssValueParser.GetTextAlign(node.GetInlineStyles());
                if (textAlign != null)
                {
                    switch (textAlign)
                    {
                        case "center": text.AlignCenter(); break;
                        case "right": text.AlignRight(); break;
                        case "left": text.AlignLeft(); break;
                        case "justify": text.Justify(); break;
                    }
                }
            }

            // Also check parent nodes for alignment classes
            if (node.ParentNode != null)
            {
                ApplyClassTextAlignments(node.ParentNode, text);
            }
        }

        public void Compose(IContainer container)
        {
            var listItemNode = GetListItemNode(lineNodes.First()) ?? GetParrentBlock(lineNodes.First());
            if (listItemNode == null) return;

            var numberInList = listItemNode.GetNumberInList();

            if (numberInList != -1 || listItemNode.GetListNode() != null)
            {
                container.Row(row =>
                {
                    var listPrefix = numberInList == -1 ? "" : numberInList == 0 ? "•  " : $"{numberInList}. ";
                    row.AutoItem().MinWidth(26).AlignCenter().Text(listPrefix);
                    container = row.RelativeItem();
                });
            }

            var first = lineNodes.First();
            var last = lineNodes.First();

            first.InnerHtml = first.InnerHtml.TrimStart();
            last.InnerHtml = last.InnerHtml.TrimEnd();

            container.Text(text =>
            {
                // Apply class-based text alignments from the block parent or list item
                ApplyClassTextAlignments(listItemNode, text);

                // Apply the text content
                GetAction(lineNodes)(text);
            });
        }

        private Action<TextDescriptor> GetAction(List<HtmlNode> nodes)
        {
            return text =>
            {
                lineNodes.ForEach(node => GetAction(node).Invoke(text));
            };
        }

        private Action<TextDescriptor> GetAction(HtmlNode node)
        {
            return text =>
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    var span = text.Span(node.InnerText);
                    GetTextSpanAction(node).Invoke(span);
                }
                else if (node.IsBr())
                {
                    var span = text.Span("\n");
                    GetTextSpanAction(node).Invoke(span);
                }
                else
                {
                    foreach (var item in node.ChildNodes)
                    {
                        var action = GetAction(item);
                        action(text);
                    }
                }
            };
        }

        private TextSpanAction GetTextSpanAction(HtmlNode node)
        {
            return spanAction => spanAction.Style(GetMergedTextStyle(node));
        }

        private TextStyle GetMergedTextStyle(HtmlNode node)
        {
            var path = new List<HtmlNode>();
            for (var n = node; n != null; n = n.ParentNode)
                path.Add(n);
            path.Reverse(); // root first

            var style = TextStyle.Default;
            foreach (var n in path)
            {
                if (TagModifiers.TryGetValue(n.Name, out var mod))
                    style = mod(style);
                style = CssValueParser.ApplyInlineTextStyle(style, n.GetInlineStyles());
            }

            // Class-based styles: apply from leaf node (last in path), override merged tag+inline
            var leaf = path[path.Count - 1];
            var classes = leaf.GetClasses();
            foreach (var className in classes)
            {
                if (args.ClassTextStyles.TryGetValue(className, out var classStyle))
                    style = classStyle;
            }

            return style;
        }

        public TextSpanAction GetTextStyles(HtmlNode element)
        {
            return (span) => span.Style(GetMergedTextStyle(element));
        }

        public TextStyle GetTextStyle(HtmlNode element)
        {
            return GetMergedTextStyle(element);
        }
    }
}