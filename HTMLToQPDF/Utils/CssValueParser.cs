using System.Diagnostics;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Utils
{
    internal static class CssValueParser
    {
        private static readonly HashSet<string> RecognizedProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            "font-size", "font-weight", "font-style", "text-decoration",
            "color", "background-color", "text-align", "line-height"
        };

        public static TextStyle ApplyInlineTextStyle(TextStyle baseStyle, Dictionary<string, string>? properties)
        {
            if (properties == null || properties.Count == 0)
                return baseStyle;

            var style = baseStyle;

            foreach (var kvp in properties)
            {
                switch (kvp.Key.ToLowerInvariant())
                {
                    case "font-size":
                        var fontSize = ParseFontSize(kvp.Value);
                        if (fontSize.HasValue)
                            style = style.FontSize(fontSize.Value);
                        break;

                    case "font-weight":
                        if (IsBold(kvp.Value))
                            style = style.Bold();
                        else
                            style = style.NormalWeight();
                        break;

                    case "font-style":
                        if (IsItalic(kvp.Value))
                            style = style.Italic();
                        else
                            style = style.NormalPosition();
                        break;

                    case "text-decoration":
                        if (kvp.Value.Trim().Equals("underline", StringComparison.OrdinalIgnoreCase))
                            style = style.Underline();
                        break;

                    case "color":
                        var fontColor = CssColorParser.Parse(kvp.Value);
                        if (fontColor.HasValue)
                            style = style.FontColor(fontColor.Value);
                        break;

                    case "background-color":
                        var bgColor = CssColorParser.Parse(kvp.Value);
                        if (bgColor.HasValue)
                            style = style.BackgroundColor(bgColor.Value);
                        break;

                    case "line-height":
                        var lineHeight = ParseLineHeight(kvp.Value);
                        if (lineHeight.HasValue)
                            style = style.LineHeight(lineHeight.Value);
                        break;

                    case "text-align":
                        // Handled at container/text-descriptor level, not TextStyle
                        break;

                    default:
                        Debug.WriteLine($"HTMLQuestPDF: Unrecognized inline CSS property '{kvp.Key}': '{kvp.Value}'");
                        break;
                }
            }

            return style;
        }

        public static string? GetTextAlign(Dictionary<string, string>? properties)
        {
            if (properties == null)
                return null;

            if (properties.TryGetValue("text-align", out var value))
                return value.Trim().ToLowerInvariant();

            return null;
        }

        private static float? ParseFontSize(string value)
        {
            value = value.Trim().ToLowerInvariant();

            if (value.EndsWith("pt"))
            {
                if (float.TryParse(value.Substring(0, value.Length - 2).Trim(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var pt))
                    return pt;
            }
            else if (value.EndsWith("px"))
            {
                if (float.TryParse(value.Substring(0, value.Length - 2).Trim(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var px))
                    return px * 0.75f; // 1px = 0.75pt
            }

            return null;
        }

        private static bool IsBold(string value)
        {
            value = value.Trim().ToLowerInvariant();

            if (value == "bold" || value == "bolder")
                return true;

            if (value == "normal" || value == "lighter")
                return false;

            if (int.TryParse(value, out var weight))
                return weight >= 700;

            return false;
        }

        private static bool IsItalic(string value)
        {
            value = value.Trim().ToLowerInvariant();
            return value == "italic" || value == "oblique";
        }

        private static float? ParseLineHeight(string value)
        {
            value = value.Trim().ToLowerInvariant();

            if (value.EndsWith("pt"))
            {
                if (float.TryParse(value.Substring(0, value.Length - 2).Trim(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var pt))
                {
                    // Convert absolute pt to relative multiplier (base 12pt)
                    return pt / 12f;
                }
            }
            else if (value.EndsWith("px"))
            {
                if (float.TryParse(value.Substring(0, value.Length - 2).Trim(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var px))
                {
                    // Convert px to pt, then to relative multiplier
                    return (px * 0.75f) / 12f;
                }
            }
            else if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var unitless))
            {
                // Unitless value is already a multiplier
                return unitless;
            }

            return null;
        }
    }
}
