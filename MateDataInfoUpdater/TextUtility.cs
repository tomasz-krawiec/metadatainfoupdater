using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MateDataInfoUpdater
{
    public class TextUtility
    {
        public static string CleanToken(string input)
        {
            var result = input.Replace(Constants.NvarcharPrefix, string.Empty);
            result = result.Trim('\'');
            return result.Trim();
        }

        public static string GetFormattedText(string text)
        {
            return string.IsNullOrEmpty(text) || text.Contains(Constants.NULLFragment) ? Constants.NULLFragment : $"'{text}'";
        }
    }
}
