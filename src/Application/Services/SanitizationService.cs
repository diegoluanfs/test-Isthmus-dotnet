using System.Text.RegularExpressions;

namespace Application.Services
{
    public class SanitizationService
    {
        public string SanitizeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remover tags HTML e scripts
            input = Regex.Replace(input, "<.*?>", string.Empty);

            // Escapar caracteres perigosos
            input = input.Replace("'", "&#39;")
                         .Replace("\"", "&quot;")
                         .Replace("<", "&lt;")
                         .Replace(">", "&gt;")
                         .Replace("&", "&amp;");

            return input;
        }
    }
}