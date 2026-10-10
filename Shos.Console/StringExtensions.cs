using System.Text.RegularExpressions;

namespace Shos.Console;

public static partial class StringExtensions
{
    public static int Width(this string @this) => @this.Sum(character => character.IsZenkaku() ? 2 : 1);

    public static bool IsZenkaku(this char @this) => ZenkakuRegex().IsMatch(@this.ToString());

    [GeneratedRegex("[^\x01-\x7Eｦ-ﾟ]")]
    private static partial Regex ZenkakuRegex();
}
