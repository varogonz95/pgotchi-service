using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Pgotchi.Functions;

internal class JsonCapitalCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        // Split the input by non-alphanumeric characters
        string[] words = name.Split(new[] { '-', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // Capitalize the first letter of each word
        StringBuilder result = new StringBuilder();
        foreach (string word in words)
        {
            result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word));
        }

        return result.ToString();
    }
}
