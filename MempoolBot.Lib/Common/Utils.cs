using System.Text.RegularExpressions;

namespace MempoolBot.Lib.Common
{
	public class Utils
	{
        public static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }

        public static string EscapeMarkdownSpecialCharacters(string input)
        {
            // Define a dictionary of Markdown special characters and their escaped counterparts
            var escapeMap = new Dictionary<string, string>
        {
            //{ "*", "\\*" },
            //{ "_", "\\_" },
            //{ "[", "\\[" },
            //{ "]", "\\]" },
            //{ "~", "\\~" },
            { "(", "\\(" },
            { ")", "\\)" },
            { "`", "\\`" },
            { "!", "\\!" },
            { ">", "\\>" },
            { "#", "\\#" },
            { "+", "\\+" },
            { "-", "\\-" },
            { "=", "\\=" },
            //{ "|", "\\|" },
            { ".", "\\." },
            { "{", "\\{" },
            { "}", "\\}" },
        };

            // Create a regular expression pattern that matches any of the special characters
            string pattern = string.Join("|", escapeMap.Keys.Select(Regex.Escape));

            // Use a regular expression to replace the special characters with their escaped counterparts
            string escaped = Regex.Replace(input, pattern, match => escapeMap[match.Value]);

            return escaped;
        }
    }
}

