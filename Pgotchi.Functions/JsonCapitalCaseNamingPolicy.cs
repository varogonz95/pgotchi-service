using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Pgotchi.Functions;

internal class JsonCapitalCaseNamingPolicy : JsonNamingPolicy
{
    internal static readonly char[] separator = ['-', '_', ' '];

    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        // Split the input by non-alphanumeric characters
        string[] words = name.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 1)
        {
            return name[0].ToString().ToUpperInvariant() + name[1..];
        }

        // Capitalize the first letter of each word
        var result = new StringBuilder();
        foreach (string word in words)
        {
            result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word));
        }

        return result.ToString();
    }
}
