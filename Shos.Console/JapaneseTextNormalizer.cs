namespace Shos.Console
{
    using System;
    using System.Collections.Generic;

    /// <summary>Provides methods to normalize Japanese text by converting inappropriate ASCII characters and half-width katakana to their appropriate forms.</summary>
    public static class JapaneseTextNormalizer
    {
        const string inappropriateAsciiCharacters = "　！”＃＄％＆’（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［￥］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝￣";
        const string appropriateAsciiCharacters   = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        static readonly Dictionary<string, char> characterTable = new() {
            { "ｧ", 'ァ' }, { "ｱ", 'ア' }, { "ｨ", 'ィ' }, { "ｲ", 'イ' }, { "ｩ", 'ゥ' }, { "ｳ", 'ウ' }, { "ｪ", 'ェ' }, { "ｴ", 'エ' }, { "ｫ", 'ォ' }, { "ｵ", 'オ' },
            { "ｶ", 'カ' }, { "ｶﾞ",'ガ' }, { "ｷ", 'キ' }, { "ｷﾞ",'ギ' }, { "ｸ", 'ク' }, { "ｸﾞ",'グ' }, { "ｹ", 'ケ' }, { "ｹﾞ",'ゲ' }, { "ｺ", 'コ' }, { "ｺﾞ",'ゴ' },
            { "ｻ", 'サ' }, { "ｻﾞ",'ザ' }, { "ｼ", 'シ' }, { "ｼﾞ",'ジ' }, { "ｽ", 'ス' }, { "ｽﾞ",'ズ' }, { "ｾ", 'セ' }, { "ｾﾞ",'ゼ' }, { "ｿ", 'ソ' }, { "ｿﾞ",'ゾ' },
            { "ﾀ", 'タ' }, { "ﾀﾞ",'ダ' }, { "ﾁ", 'チ' }, { "ﾁﾞ",'ヂ' }, { "ｯ", 'ッ' }, { "ﾂ", 'ツ' }, { "ﾂﾞ",'ヅ' }, { "ﾃ", 'テ' }, { "ﾃﾞ",'デ' }, { "ﾄ", 'ト' }, { "ﾄﾞ",'ド' },
            { "ﾅ", 'ナ' }, { "ﾆ", 'ニ' }, { "ﾇ", 'ヌ' }, { "ﾈ", 'ネ' }, { "ﾉ", 'ノ' },
            { "ﾊ", 'ハ' }, { "ﾊﾞ",'バ' }, { "ﾊﾟ",'パ' }, { "ﾋ", 'ヒ' }, { "ﾋﾞ",'ビ' }, { "ﾋﾟ",'ピ' }, { "ﾌ", 'フ' }, { "ﾌﾞ",'ブ' }, { "ﾌﾟ",'プ' }, { "ﾍ", 'ヘ' }, { "ﾍﾞ",'ベ' }, { "ﾍﾟ",'ペ' }, { "ﾎ", 'ホ' }, { "ﾎﾞ",'ボ' }, { "ﾎﾟ",'ポ' },
            { "ﾏ", 'マ' }, { "ﾐ", 'ミ' }, { "ﾑ", 'ム' }, { "ﾒ", 'メ' }, { "ﾓ", 'モ' },
            { "ｬ", 'ャ' }, { "ﾔ", 'ヤ' }, { "ｭ", 'ュ' }, { "ﾕ", 'ユ' }, { "ｮ", 'ョ' }, { "ﾖ", 'ヨ' },
            { "ﾗ", 'ラ' }, { "ﾘ", 'リ' }, { "ﾙ", 'ル' }, { "ﾚ", 'レ' }, { "ﾛ", 'ロ' }, { "ﾜ", 'ワ' }, { "ｦ", 'ヲ' }, { "ﾝ", 'ン' }, { "ｳﾞ",'ヴ' },
            { "･", '・' }, { "ｰ", 'ー' }, { "｡", '。' }, { "｢", '「' }, { "｣", '」' }, { "､", '、' }, { "ﾞ", '゛' }, { "ﾟ", '゜' }
        };

        /// <summary>Normalizes the specified string by converting inappropriate ASCII characters and half-width katakana to their appropriate forms.</summary>
        /// <param name="this">The string to normalize.</param>
        /// <returns>A new string with normalized characters.</returns>
        public static string Apply(this string @this) => new(@this.ApplyAll().ToArray());

        /// <summary>Normalizes the specified string by converting inappropriate ASCII characters and half-width katakana to their appropriate forms.</summary>
        /// <param name="this">The string to normalize.</param>
        /// <returns>An enumerable of normalized characters.</returns>
        static IEnumerable<char> ApplyAll(this string @this)
        {
            for (var index = 0; index < @this.Length; index++) {
                if (index < @this.Length - 1 && !@this[index].IsTurbidOrSemiTurbid() && @this[index + 1].IsTurbidOrSemiTurbid() &&
                    Apply(@this[index], @this[index + 1], out var result)) {
                    index++;
                    yield return result;
                } else {
                    yield return @this[index].Apply();
                }
            }
        }

        static bool IsTurbidOrSemiTurbid(this char @this) => @this is 'ﾞ' or 'ﾟ';

        /// <summary>Normalizes the specified character by converting it to its appropriate form if it is an inappropriate ASCII character or half-width katakana.</summary>
        /// <param name="this">The character to normalize.</param>
        /// <returns>The normalized character.</returns>
        static char Apply(this char @this)
            => characterTable.TryGetValue(new string([@this]), out var result) ? result : @this;

        /// <summary>Normalizes the specified characters by converting them to their appropriate form if they are inappropriate ASCII characters or half-width katakana.</summary>
        /// <param name="character1">The first character to normalize.</param>
        /// <param name="character2">The second character to normalize.</param>
        /// <returns>The normalized character.</returns>
        static bool Apply(char character1, char character2, out char result)
            => characterTable.TryGetValue(new string([character1, character2]), out result);

        /// <summary>Initializes the <see cref="JapaneseTextNormalizer"/> class by populating the character table with mappings from inappropriate ASCII characters to appropriate ASCII characters.</summary>
        static JapaneseTextNormalizer()
        {
            var length = Math.Min(inappropriateAsciiCharacters.Length, appropriateAsciiCharacters.Length);
            for (var index = 0; index < length; index++)
                characterTable[new string([inappropriateAsciiCharacters[index]])] = appropriateAsciiCharacters[index];
        }
    }
}
