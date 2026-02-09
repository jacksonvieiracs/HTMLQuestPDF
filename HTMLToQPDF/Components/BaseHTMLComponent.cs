using HtmlAgilityPack;
using HTMLQuestPDF.Extensions;
using HTMLToQPDF.Components;
using HTMLToQPDF.Utils;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components
{
    internal class BaseHTMLComponent : IComponent
    {
        protected readonly HTMLComponentsArgs args;
        protected readonly HtmlNode node;

        public BaseHTMLComponent(HtmlNode node, HTMLComponentsArgs args)
        {
            this.node = node;
            this.args = args;
        }

        public void Compose(IContainer container)
        {
            if (node.Name.ToLower() == "head") return;
            // Always render block elements (p, div, etc.) even when empty, so <p></p> or <p>&nbsp;</p> produces a line
            if (!node.HasContent() && !node.IsBlockNode()) return;

            container = ApplyStyles(container);

            if (node.ChildNodes.Any())
            {
                ComposeMany(container);
            }
            else
            {
                // Empty block: still take one line so <p></p> or <p>&nbsp;</p> produces a new line
                if (node.IsBlockNode())
                    container.Text(t => t.Span("\u00A0")); // non-breaking space for line height
                else
                    ComposeSingle(container);
            }
        }

        protected virtual IContainer ApplyStyles(IContainer container)
        {
            if (args.ContainerStyles.TryGetValue(node.Name.ToLower(), out var tagStyle))
            {
                container = tagStyle(container);
            }

            var classes = node.GetClasses();
            foreach (var className in classes)
            {
                if (args.ClassContainerStyles.TryGetValue(className, out var classStyle))
                {
                    container = classStyle(container);
                }
            }

            if (node.IsBlockNode())
            {
                var textAlign = CssValueParser.GetTextAlign(node.GetInlineStyles());
                if (textAlign != null)
                {
                    container = textAlign switch
                    {
                        "center" => container.AlignCenter(),
                        "right" => container.AlignRight(),
                        "left" => container.AlignLeft(),
                        _ => container
                    };
                }
            }

            return container;
        }

        protected virtual void ComposeSingle(IContainer container)
        {
        }

        protected virtual void ComposeMany(IContainer container)
        {
            container.Column(col =>
            {
                var buffer = new List<HtmlNode>();
                foreach (var item in node.ChildNodes)
                {
                    if (item.IsBlockNode() || item.HasBlockElement())
                    {
                        ComposeMany(col, buffer);
                        buffer.Clear();

                        col.Item().Component(item.GetComponent(args));
                    }
                    else
                    {
                        buffer.Add(item);
                    }
                }
                ComposeMany(col, buffer);
            });
        }

        private void ComposeMany(ColumnDescriptor col, List<HtmlNode> nodes)
        {
            if (nodes.Count == 1)
            {
                col.Item().Component(nodes.First().GetComponent(args));
            }
            else if (nodes.Count > 0)
            {
                col.Item().Component(new ParagraphComponent(nodes, args));
            }
        }
    }
}