using System.Collections;
using System.Text;

namespace Shos.Console.Tests;

[TestClass]
[DoNotParallelize]
public class GridViewTests
{
    [TestMethod]
    public void Show_ObjectRows_EnumeratesInputOnce()
    {
        var output = Capture(() => Assert.IsTrue(GridView.Show(new SingleUseEnumerable<Row>([new Row { Name = "Ada" }]))));

        StringAssert.Contains(output, "Ada");
    }

    [TestMethod]
    public void Show_ObjectRows_ReadsEachPropertyOnce()
    {
        var row = new GetterRow();

        Capture(() => Assert.IsTrue(GridView.Show(new[] { row })));

        Assert.AreEqual(1, row.ReadCount);
    }

    [TestMethod]
    public void Show_ExplicitColumns_EnumeratesInputsOnce()
    {
        var values = new SingleUseEnumerable<object?>(["Ada"]);
        var columns = new SingleUseEnumerable<(string colomnName, IEnumerable<object?> values)>([("Name", values)]);

        var output = Capture(() => GridView.Show(columns));

        StringAssert.Contains(output, "Ada");
    }

    [TestMethod]
    public void Show_ObjectRows_SkipsNullRowsAndIndexerProperties()
    {
        var rows = new Row?[] { null, new Row { Name = "Ada" } };

        var output = Capture(() => Assert.IsTrue(GridView.Show(rows)));

        StringAssert.Contains(output, "Ada");
        Assert.IsFalse(output.Contains("Item", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Show_ObjectRows_ReturnsFalseWhenAllRowsAreNull()
        => Assert.IsFalse(GridView.Show(new Row?[] { null, null }));

    [TestMethod]
    public void Show_ExplicitColumns_DoesNothingWhenThereAreNoColumns()
    {
        var output = Capture(() => GridView.Show([]));

        Assert.AreEqual("", output);
    }

    static string Capture(Action action)
    {
        var output = new StringBuilder();
        var originalWriteCharacter = GridView.WriteCharacter;
        var originalWrite = GridView.Write;
        var originalWriteLine = GridView.WriteLine;
        var originalNewLine = GridView.NewLine;
        try {
            GridView.WriteCharacter = character => output.Append(character);
            GridView.Write = text => output.Append(text);
            GridView.WriteLine = text => output.AppendLine(text);
            GridView.NewLine = () => output.AppendLine();
            action();
            return output.ToString();
        } finally {
            GridView.WriteCharacter = originalWriteCharacter;
            GridView.Write = originalWrite;
            GridView.WriteLine = originalWriteLine;
            GridView.NewLine = originalNewLine;
        }
    }

    sealed class Row
    {
        public string Name { get; init; } = "";

        public string this[int index] => throw new InvalidOperationException("Indexer properties must not be read.");
    }

    sealed class GetterRow
    {
        public int ReadCount;

        public string Name {
            get {
                ReadCount++;
                return "Ada";
            }
        }
    }

    sealed class SingleUseEnumerable<T>(IEnumerable<T> values) : IEnumerable<T>
    {
        bool enumerated;

        public IEnumerator<T> GetEnumerator()
        {
            if (enumerated)
                throw new InvalidOperationException("This sequence can only be enumerated once.");

            enumerated = true;
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
