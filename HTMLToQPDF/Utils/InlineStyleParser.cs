namespace HTMLToQPDF.Utils
{
    internal static class InlineStyleParser
    {
        public static Dictionary<string, string> Parse(string styleAttribute)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(styleAttribute))
                return result;

            var declarations = styleAttribute.Split(';');

            foreach (var declaration in declarations)
            {
                var trimmed = declaration.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                var colonIndex = trimmed.IndexOf(':');
                if (colonIndex < 0)
                    continue;

                var property = trimmed.Substring(0, colonIndex).Trim();
                var value = trimmed.Substring(colonIndex + 1).Trim();

                if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(value))
                    continue;

                // Last declaration wins
                result[property] = value;
            }

            return result;
        }
    }
}
