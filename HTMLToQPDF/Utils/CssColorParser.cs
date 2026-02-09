using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Utils
{
    internal static class CssColorParser
    {
        public static Color? Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim().ToLowerInvariant();

            if (value.StartsWith('#'))
                return ParseHex(value);

            if (value.StartsWith("rgb("))
                return ParseRgb(value);

            if (NamedColors.TryGetValue(value, out var named))
                return named;

            return null;
        }

        private static Color? ParseHex(string hex)
        {
            hex = hex.TrimStart('#');

            if (hex.Length == 3)
            {
                hex = new string(new[] { hex[0], hex[0], hex[1], hex[1], hex[2], hex[2] });
            }

            if (hex.Length != 6)
                return null;

            if (!int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out var r) ||
                !int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var g) ||
                !int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var b))
                return null;

            return Color.FromRGB((byte)r, (byte)g, (byte)b);
        }

        private static Color? ParseRgb(string value)
        {
            // rgb(255, 0, 0) or rgb(255,0,0)
            var start = value.IndexOf('(');
            var end = value.IndexOf(')');
            if (start < 0 || end < 0 || end <= start)
                return null;

            var inner = value.Substring(start + 1, end - start - 1);
            var parts = inner.Split(',');
            if (parts.Length != 3)
                return null;

            if (!int.TryParse(parts[0].Trim(), out var r) ||
                !int.TryParse(parts[1].Trim(), out var g) ||
                !int.TryParse(parts[2].Trim(), out var b))
                return null;

            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);

            return Color.FromRGB((byte)r, (byte)g, (byte)b);
        }

        private static readonly Dictionary<string, Color> NamedColors = new(StringComparer.OrdinalIgnoreCase)
        {
            { "black", Color.FromRGB(0, 0, 0) },
            { "white", Color.FromRGB(255, 255, 255) },
            { "red", Color.FromRGB(255, 0, 0) },
            { "green", Color.FromRGB(0, 128, 0) },
            { "blue", Color.FromRGB(0, 0, 255) },
            { "yellow", Color.FromRGB(255, 255, 0) },
            { "cyan", Color.FromRGB(0, 255, 255) },
            { "magenta", Color.FromRGB(255, 0, 255) },
            { "orange", Color.FromRGB(255, 165, 0) },
            { "purple", Color.FromRGB(128, 0, 128) },
            { "pink", Color.FromRGB(255, 192, 203) },
            { "brown", Color.FromRGB(165, 42, 42) },
            { "gray", Color.FromRGB(128, 128, 128) },
            { "grey", Color.FromRGB(128, 128, 128) },
            { "silver", Color.FromRGB(192, 192, 192) },
            { "navy", Color.FromRGB(0, 0, 128) },
            { "teal", Color.FromRGB(0, 128, 128) },
            { "maroon", Color.FromRGB(128, 0, 0) },
            { "olive", Color.FromRGB(128, 128, 0) },
            { "lime", Color.FromRGB(0, 255, 0) },
            { "aqua", Color.FromRGB(0, 255, 255) },
            { "fuchsia", Color.FromRGB(255, 0, 255) },
            { "coral", Color.FromRGB(255, 127, 80) },
            { "salmon", Color.FromRGB(250, 128, 114) },
            { "tomato", Color.FromRGB(255, 99, 71) },
            { "gold", Color.FromRGB(255, 215, 0) },
            { "khaki", Color.FromRGB(240, 230, 140) },
            { "plum", Color.FromRGB(221, 160, 221) },
            { "violet", Color.FromRGB(238, 130, 238) },
            { "indigo", Color.FromRGB(75, 0, 130) },
            { "orchid", Color.FromRGB(218, 112, 214) },
            { "tan", Color.FromRGB(210, 180, 140) },
            { "crimson", Color.FromRGB(220, 20, 60) },
            { "turquoise", Color.FromRGB(64, 224, 208) },
            { "sienna", Color.FromRGB(160, 82, 45) },
            { "peru", Color.FromRGB(205, 133, 63) },
            { "chocolate", Color.FromRGB(210, 105, 30) },
            { "firebrick", Color.FromRGB(178, 34, 34) },
            { "darkred", Color.FromRGB(139, 0, 0) },
            { "darkgreen", Color.FromRGB(0, 100, 0) },
            { "darkblue", Color.FromRGB(0, 0, 139) },
            { "darkcyan", Color.FromRGB(0, 139, 139) },
            { "darkmagenta", Color.FromRGB(139, 0, 139) },
            { "darkorange", Color.FromRGB(255, 140, 0) },
            { "darkviolet", Color.FromRGB(148, 0, 211) },
            { "deeppink", Color.FromRGB(255, 20, 147) },
            { "deepskyblue", Color.FromRGB(0, 191, 255) },
            { "dimgray", Color.FromRGB(105, 105, 105) },
            { "dimgrey", Color.FromRGB(105, 105, 105) },
            { "dodgerblue", Color.FromRGB(30, 144, 255) },
            { "forestgreen", Color.FromRGB(34, 139, 34) },
            { "gainsboro", Color.FromRGB(220, 220, 220) },
            { "hotpink", Color.FromRGB(255, 105, 180) },
            { "indianred", Color.FromRGB(205, 92, 92) },
            { "ivory", Color.FromRGB(255, 255, 240) },
            { "lavender", Color.FromRGB(230, 230, 250) },
            { "lawngreen", Color.FromRGB(124, 252, 0) },
            { "lightblue", Color.FromRGB(173, 216, 230) },
            { "lightcoral", Color.FromRGB(240, 128, 128) },
            { "lightgray", Color.FromRGB(211, 211, 211) },
            { "lightgrey", Color.FromRGB(211, 211, 211) },
            { "lightgreen", Color.FromRGB(144, 238, 144) },
            { "lightpink", Color.FromRGB(255, 182, 193) },
            { "lightsalmon", Color.FromRGB(255, 160, 122) },
            { "lightseagreen", Color.FromRGB(32, 178, 170) },
            { "lightskyblue", Color.FromRGB(135, 206, 250) },
            { "lightsteelblue", Color.FromRGB(176, 196, 222) },
            { "lightyellow", Color.FromRGB(255, 255, 224) },
            { "limegreen", Color.FromRGB(50, 205, 50) },
            { "linen", Color.FromRGB(250, 240, 230) },
            { "mediumaquamarine", Color.FromRGB(102, 205, 170) },
            { "mediumblue", Color.FromRGB(0, 0, 205) },
            { "mediumorchid", Color.FromRGB(186, 85, 211) },
            { "mediumpurple", Color.FromRGB(147, 112, 219) },
            { "mediumseagreen", Color.FromRGB(60, 179, 113) },
            { "mediumslateblue", Color.FromRGB(123, 104, 238) },
            { "mediumspringgreen", Color.FromRGB(0, 250, 154) },
            { "mediumturquoise", Color.FromRGB(72, 209, 204) },
            { "mediumvioletred", Color.FromRGB(199, 21, 133) },
            { "midnightblue", Color.FromRGB(25, 25, 112) },
            { "mintcream", Color.FromRGB(245, 255, 250) },
            { "mistyrose", Color.FromRGB(255, 228, 225) },
            { "moccasin", Color.FromRGB(255, 228, 181) },
            { "navajowhite", Color.FromRGB(255, 222, 173) },
            { "oldlace", Color.FromRGB(253, 245, 230) },
            { "olivedrab", Color.FromRGB(107, 142, 35) },
            { "orangered", Color.FromRGB(255, 69, 0) },
            { "palegoldenrod", Color.FromRGB(238, 232, 170) },
            { "palegreen", Color.FromRGB(152, 251, 152) },
            { "paleturquoise", Color.FromRGB(175, 238, 238) },
            { "palevioletred", Color.FromRGB(219, 112, 147) },
            { "papayawhip", Color.FromRGB(255, 239, 213) },
            { "peachpuff", Color.FromRGB(255, 218, 185) },
            { "powderblue", Color.FromRGB(176, 224, 230) },
            { "rosybrown", Color.FromRGB(188, 143, 143) },
            { "royalblue", Color.FromRGB(65, 105, 225) },
            { "saddlebrown", Color.FromRGB(139, 69, 19) },
            { "sandybrown", Color.FromRGB(244, 164, 96) },
            { "seagreen", Color.FromRGB(46, 139, 87) },
            { "seashell", Color.FromRGB(255, 245, 238) },
            { "skyblue", Color.FromRGB(135, 206, 235) },
            { "slateblue", Color.FromRGB(106, 90, 205) },
            { "slategray", Color.FromRGB(112, 128, 144) },
            { "slategrey", Color.FromRGB(112, 128, 144) },
            { "snow", Color.FromRGB(255, 250, 250) },
            { "springgreen", Color.FromRGB(0, 255, 127) },
            { "steelblue", Color.FromRGB(70, 130, 180) },
            { "thistle", Color.FromRGB(216, 191, 216) },
            { "wheat", Color.FromRGB(245, 222, 179) },
            { "whitesmoke", Color.FromRGB(245, 245, 245) },
            { "yellowgreen", Color.FromRGB(154, 205, 50) },
            { "darkgoldenrod", Color.FromRGB(184, 134, 11) },
            { "darkgray", Color.FromRGB(169, 169, 169) },
            { "darkgrey", Color.FromRGB(169, 169, 169) },
            { "darkkhaki", Color.FromRGB(189, 183, 107) },
            { "darkolivegreen", Color.FromRGB(85, 107, 47) },
            { "darksalmon", Color.FromRGB(233, 150, 122) },
            { "darkseagreen", Color.FromRGB(143, 188, 143) },
            { "darkslateblue", Color.FromRGB(72, 61, 139) },
            { "darkslategray", Color.FromRGB(47, 79, 79) },
            { "darkslategrey", Color.FromRGB(47, 79, 79) },
            { "darkturquoise", Color.FromRGB(0, 206, 209) },
            { "floralwhite", Color.FromRGB(255, 250, 240) },
            { "ghostwhite", Color.FromRGB(248, 248, 255) },
            { "goldenrod", Color.FromRGB(218, 165, 32) },
            { "greenyellow", Color.FromRGB(173, 255, 47) },
            { "honeydew", Color.FromRGB(240, 255, 240) },
            { "aliceblue", Color.FromRGB(240, 248, 255) },
            { "antiquewhite", Color.FromRGB(250, 235, 215) },
            { "azure", Color.FromRGB(240, 255, 255) },
            { "beige", Color.FromRGB(245, 245, 220) },
            { "bisque", Color.FromRGB(255, 228, 196) },
            { "blanchedalmond", Color.FromRGB(255, 235, 205) },
            { "blueviolet", Color.FromRGB(138, 43, 226) },
            { "burlywood", Color.FromRGB(222, 184, 135) },
            { "cadetblue", Color.FromRGB(95, 158, 160) },
            { "chartreuse", Color.FromRGB(127, 255, 0) },
            { "cornflowerblue", Color.FromRGB(100, 149, 237) },
            { "cornsilk", Color.FromRGB(255, 248, 220) },
        };
    }
}
