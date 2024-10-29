using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Helpers
{
    public class Strings
    {
        public static string StringsToString(IList<string> vals)
        {
            return string.Join(';', vals.Where(v => !string.IsNullOrWhiteSpace(v)));
        }

        public static string StringsToString(string? val1, string? val2)
        {
            if (val1 == null) val1 = string.Empty;
            if (val2 == null) val2 = string.Empty;
            return StringsToString(new string[] { val1, val2 });
        }

        public static string FirstCharToUpper(string input, bool onlyInFirstWord = true, StringSplitOptions stringSplitOptions = StringSplitOptions.RemoveEmptyEntries)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if (onlyInFirstWord) 
                return Regex.Replace(input, "^[a-zżźćńółęąś]", c => c.Value.ToUpper());
            else
            {
                var tab = input.Split(' ', stringSplitOptions);
                for(int i = 0; i<tab.Length; i++)
                {
                    tab[i] = FirstCharToUpper(tab[i]);
                }
                input = string.Join(' ', tab);
                tab = input.Split('-', stringSplitOptions);
                for (int i = 0; i < tab.Length; i++)
                {
                    tab[i] = FirstCharToUpper(tab[i]);
                }
                return string.Join('-', tab);
            }
        }

        public static string[][]? SplitLines(string[] lines, string splitText)
        {
            if (lines==null || lines.Length==0)
                return null;
            var result = new List<string[]>();
            List<string>? segmentElements = null;
            foreach (var line in lines)
            {
                if (line.StartsWith(splitText))
                {
                    if (segmentElements != null)
                    {
                        result.Add(segmentElements.ToArray());
                        segmentElements = null;
                    }
                    continue;
                }
                if (segmentElements == null)
                    segmentElements = new List<string>();
                segmentElements.Add(line);
            }
            if (segmentElements != null)
                result.Add(segmentElements.ToArray());
            return result.ToArray();
        }
    }
}
