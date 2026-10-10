using System.Collections;
using System.Text;

namespace Shos.Console;

public static class GridView
{
    class Column(string header, IEnumerable<object?> values)
    {
        int? width;

        public int Width => width ??= GetWidth();

        string[]? texts;

        string[] Texts => texts ??= GetTexts();

        public string this[int index] => 0 <= index && index < Texts.Length ? Texts[index] : ToCellText("");

        int GetWidth() => Items.Max(item => ToString(item).Width());
        string[] GetTexts() => [.. Items.Select(item => ToCellText(ToString(item), rightJustified: IsRightJustified(item)))];

        static bool IsRightJustified(object? item)
            => item is int or uint or short or ushort or long or ulong or double or float or decimal;

        static string ToString(object? item)
            => item switch {
                double value => value.ToString("F1"),
                float  value => value.ToString("F1"),
                _            => item?.ToString(    )
            } ?? "";

        string ToCellText(string? text, bool rightJustified = false)
        {
            text     ??= "";
            var spaces = new string(' ', Width - text.Width());
            return rightJustified ? $"{spaces}{text}" : $"{text}{spaces}";
        }

        IEnumerable<object?> Items {
            get {
                yield return header;
                foreach (var value in values)
                    yield return value;
            }
        }
    }

    static class FrameCharacters
    {
        public const char Space         = ' ';
        public const char Cross         = '+';
        public const char HorizontalBar = '-';
        public const char VerticalBar   = '|';
    }

    enum FramePosition
    {
        First ,
        Middle,
        Last
    }

    public static Action<char  > WriteCharacter = character => System.Console.Write    (character);
    public static Action<string> Write          = text      => System.Console.Write    (text     );
    public static Action<string> WriteLine      = text      => System.Console.WriteLine(text     );
    public static Action         NewLine        = ()        => System.Console.WriteLine(         );

    public static bool ShowTable(this object @this, bool hasFrame = true) => Show(@this, hasFrame);
    public static void ShowTable(this IEnumerable<(string colomnName, IEnumerable<object?> values)> @this, bool hasFrame = true) => Show(@this, hasFrame);

    public static bool Show(object dataSource, bool hasFrame = false)
    {
        Initialize();

        if (dataSource is not IEnumerable collection)
            return false;

        List<object?> rows = [.. collection.Cast<object?>()];
        if (rows.FirstOrDefault(row => row is not null) is not { } firstRow)
            return false;

        var properties = firstRow.GetType()
                                 .GetProperties()
                                 .Where(property => property.GetIndexParameters().Length == 0)
                                 .ToArray();
        if (properties.Length == 0)
            return false;

        Column[] columns = [.. properties.Select(property => new Column(
            header: property.Name,
            values: [.. rows.Select(row => row is null ? null : property.GetValue(row))]))];
        Show(columns, rows.Count, hasFrame);
        return true;
    }

    public static void Show(IEnumerable<(string colomnName, IEnumerable<object?> values)> dataSource, bool hasFrame = false)
    {
        var sourceColumns = dataSource.Select(data => (data.colomnName, values: data.values.ToArray())).ToArray();
        if (sourceColumns.Length == 0)
            return;

        var rowNumber = sourceColumns.Max(data => data.values.Length);
        Column[] columns = [.. sourceColumns.Select(data => new Column(header: data.colomnName, values: data.values))];
        Show(columns, rowNumber, hasFrame);
    }

    static void Show(IEnumerable<Column> columns, int rowNumber, bool hasFrame = false)
    {
        Initialize();

        ShowSeparator(columns, hasFrame);

        Enumerable.Range(0, rowNumber + 1)
                  .ForEach(rowIndex => {
                      columns.ForEach((columnIndex, column) => {
                          ShowVerticalBar(hasFrame, columnIndex == 0 ? FramePosition.First : FramePosition.Middle);
                          Write(column[rowIndex]);
                      });
                      ShowVerticalBar(hasFrame, FramePosition.Last);
                      NewLine();
                      if (rowIndex == 0)
                          ShowSeparator(columns, hasFrame);
                  });

        ShowSeparator(columns, hasFrame);
    }

    static void ShowSeparator(IEnumerable<Column> columns, bool hasFrame)
    {
        if (hasFrame)
            WriteLine(GetSeparator(columns));
    }

    static void ShowVerticalBar(bool hasFrame, FramePosition position)
    {
        switch (position) {
            case FramePosition.First:
                if (hasFrame) {
                    WriteCharacter(FrameCharacters.VerticalBar);
                    WriteCharacter(FrameCharacters.Space      );
                }
                break;
            case FramePosition.Last:
                if (hasFrame) {
                    WriteCharacter(FrameCharacters.Space      );
                    WriteCharacter(FrameCharacters.VerticalBar);
                }
                break;
            default:
                WriteCharacter(FrameCharacters.Space);
                if (hasFrame) {
                    WriteCharacter(FrameCharacters.VerticalBar);
                    WriteCharacter(FrameCharacters.Space      );
                }
                break;
        }
    }

    static string separator = "";

    static string Initialize() => separator = "";

    static string GetSeparator(IEnumerable<Column> columns)
    {
        if (string.IsNullOrWhiteSpace(separator))
            separator = CreateSeparator(columns);
        return separator;
    }

    static string CreateSeparator(IEnumerable<Column> columns)
    {
        StringBuilder stringBuilder = new();

        stringBuilder.Append(FrameCharacters.Cross);
        columns.ForEach(column => {
            stringBuilder.Append(FrameCharacters.HorizontalBar, 1 + column.Width + 1);
            stringBuilder.Append(FrameCharacters.Cross);
        });

        return stringBuilder.ToString();
    }
}
