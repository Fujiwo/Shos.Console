namespace Shos.Console.Tests;

[TestClass]
public class ConsoleHelperTests
{
    [TestMethod]
    public void HashPassword_UsesDifferentSaltsAndVerifiesPassword()
    {
        var first = ConsoleHelper.HashPassword("Correct!Horse9");
        var second = ConsoleHelper.HashPassword("Correct!Horse9");
        var fields = first.Split('$');

        Assert.AreNotEqual(first, second);
        Assert.AreEqual(5, fields.Length);
        Assert.AreEqual("shos-pbkdf2-sha256", fields[0]);
        Assert.AreEqual("v1", fields[1]);
        Assert.AreEqual("600000", fields[2]);
        Assert.AreEqual(16, Convert.FromBase64String(fields[3]).Length);
        Assert.AreEqual(32, Convert.FromBase64String(fields[4]).Length);
        Assert.IsTrue(ConsoleHelper.VerifyPassword("Correct!Horse9", first));
        Assert.IsFalse(ConsoleHelper.VerifyPassword("Incorrect!Horse9", first));
        Assert.IsTrue(ConsoleHelper.VerifyPassword("Correct!Horse9", second));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("not-a-password-hash")]
    [DataRow("shos-pbkdf2-sha256$v2$600000$AA==$AA==")]
    [DataRow("shos-pbkdf2-sha256$v1$not-an-integer$AA==$AA==")]
    [DataRow("shos-pbkdf2-sha256$v1$600000$%%%$AA==")]
    [DataRow("shos-pbkdf2-sha256$v1$600000$AA==$AA==")]
    public void VerifyPassword_ReturnsFalseForInvalidHash(string hash)
        => Assert.IsFalse(ConsoleHelper.VerifyPassword("password", hash));
}
