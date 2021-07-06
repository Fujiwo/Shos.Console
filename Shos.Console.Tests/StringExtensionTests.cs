using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shos.Console.Tests
{
    [TestClass()]
    public class StringExtensionTests
    {
        [TestMethod()]
        public void ZenkakuTest()
        {
            Assert.IsFalse('!'.IsZenkaku());
            Assert.IsFalse('1'.IsZenkaku());
            Assert.IsFalse('a'.IsZenkaku());
            Assert.IsFalse('A'.IsZenkaku());
            Assert.IsFalse('ｱ'.IsZenkaku());

            Assert.IsTrue('！'.IsZenkaku());
            Assert.IsTrue('１'.IsZenkaku());
            Assert.IsTrue('ａ'.IsZenkaku());
            Assert.IsTrue('Ａ'.IsZenkaku());
            Assert.IsTrue('ア'.IsZenkaku());
            Assert.IsTrue('あ'.IsZenkaku());
            Assert.IsTrue('亜'.IsZenkaku());
        }

        [TestMethod()]
        public void LengthTest()
        {
            Assert.AreEqual("".Length(), 0);
            Assert.AreEqual("!".Length(), 1);
            Assert.AreEqual("あいうえお".Length(), 5 * 2);
            Assert.AreEqual("ｱｲｳｴｵ".Length(), 5);
            Assert.AreEqual("!！ｱア".Length(), 1 + 2 + 1 + 2);
        }
    }
}