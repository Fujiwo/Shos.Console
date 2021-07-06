using System.Text.RegularExpressions;

namespace Shos.Console
{
    public static class StringExtensions
    {
        public static int Length(this string @this)
        {
            var length = 0;
            @this.ForEach(character => length += IsZenkaku(character) ? 2 : 1);
            return length;
        }

        public static bool IsZenkaku(this char @this) => new string(@this, 1).IsZenkaku();
        static bool IsZenkaku(this string @this) => Regex.IsMatch(@this, "[^\x01-\x7Eｦ-ﾟ]");
    }
}
