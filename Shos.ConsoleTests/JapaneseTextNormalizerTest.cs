using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shos.Console.Tests
{
    [TestClass()]
    public class JapaneseTextNormalizerTest
    {
        [TestMethod()]
        public void AsciiCharactersTest()
        {
            const string inappropriateCharacters = "　！”＃＄％＆’（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［￥］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝￣";
            const string appropriateCharacters   = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

            var text = inappropriateCharacters.Apply();
            Assert.AreEqual(appropriateCharacters, text);
        }

        [TestMethod()]
        public void KanaCharactersTest()
        {
            const string inappropriateCharacters = "゠ｧｱｨｲｩｳｪｴｫｵｶｶﾞｷｷﾞｸｸﾞｹｹﾞｺｺﾞｻｻﾞｼｼﾞｽｽﾞｾｾﾞｿｿﾞﾀﾀﾞﾁﾁﾞｯﾂﾂﾞﾃﾃﾞﾄﾄﾞﾅﾆﾇﾈﾉﾊﾊﾞﾊﾟﾋﾋﾞﾋﾟﾌﾌﾞﾌﾟﾍﾍﾞﾍﾟﾎﾎﾞﾎﾟﾏﾐﾑﾒﾓｬﾔｭﾕｮﾖﾗﾘﾙﾚﾛヮﾜヰヱｦﾝｳﾞヵヶヷヸヹヺ･ｰヽヾヿ｡｢｣､ﾞﾟﾟﾞﾞ";
            const string appropriateCharacters   = "゠ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺ・ーヽヾヿ。「」、゛゜゜゛゛";

            var text = inappropriateCharacters.Apply();
            Assert.AreEqual(appropriateCharacters, text);
        }

        [TestMethod()]
        public void CharactersTest()
        {
            const string inappropriateCharacters = "　！”＃＄％＆’（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［￥］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝￣゠ｧｱｨｲｩｳｪｴｫｵｶｶﾞｷｷﾞｸｸﾞｹｹﾞｺｺﾞｻｻﾞｼｼﾞｽｽﾞｾｾﾞｿｿﾞﾀﾀﾞﾁﾁﾞｯﾂﾂﾞﾃﾃﾞﾄﾄﾞﾅﾆﾇﾈﾉﾊﾊﾞﾊﾟﾋﾋﾞﾋﾟﾌﾌﾞﾌﾟﾍﾍﾞﾍﾟﾎﾎﾞﾎﾟﾏﾐﾑﾒﾓｬﾔｭﾕｮﾖﾗﾘﾙﾚﾛヮﾜヰヱｦﾝｳﾞヵヶヷヸヹヺ･ｰヽヾヿ｡｢｣､ﾞﾟﾟﾞﾞあ山";
            const string appropriateCharacters   = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~゠ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺ・ーヽヾヿ。「」、゛゜゜゛゛あ山";

            var text = inappropriateCharacters.Apply();
            Assert.AreEqual(appropriateCharacters, text);
        }
    }
}