using System.Text;

namespace Pgotchi.Core.Common;

public static class StringExtensions
{
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new StringBuilder();
        var words = input.Split(new char[] { ' ', '_', '-' });

        for (int i = 0; i < words.Length; i++)
        {
            if (i == 0)
            {
                sb.Append(words[i].Substring(0, 1).ToLower() + words[i].Substring(1));
            }
            else
            {
                sb.Append(words[i].Substring(0, 1).ToUpper() + words[i].Substring(1).ToLower());
            }
        }

        return sb.ToString();
    }
}
