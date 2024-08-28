using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shos.Console
{
    public static class GridView
    {
        class Column
        {
            int? width = null;

            public int Width => width ??= GetWidth();

            string[]? texts = null;

            IList<string> Texts => texts ??= GetTexts();

            string Header { get; init; }
            IEnumerable<object?> Values { get; init; }

            public Column(string header, IEnumerable<object?> values) => (Header, Values) = (header, values);

            public string this[int index] => Texts[index];

            int GetWidth() => Items.Max(item => ToString(item).Width());
            string[] GetTexts() => Items.Select(item => ToCellText(ToString(item), rightJustified: IsRightJustified(item))).ToArray();

            static bool IsRightJustified(object? item)
                => item switch {
                    int     => true ,
                    uint    => true ,
                    short   => true ,
                    ushort  => true ,
                    long    => true ,
                    ulong   => true ,
                    double  => true ,
                    float   => true ,
                    decimal => true ,
                    _        => false
                };

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

                StringBuilder stringBuilder = new();
                if (rightJustified) {
                    stringBuilder.Append(spaces);
                    stringBuilder.Append(text);
                } else {
                    stringBuilder.Append(text);
                    stringBuilder.Append(spaces);
                }
                return stringBuilder.ToString();
            }

            IEnumerable<object?> Items {
                get {
                    yield return Header;
                    foreach (var value in Values)
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

        public static bool Show(object dataSource, bool hasFrame = false)
        {
            Initialize();

            var (collection, rowNumber, properties) = dataSource.GetProperties();
            if (collection is null || rowNumber == 0 || properties is null)
                return false;

            var columns = properties.Select(property => new Column(header: property.Name,
                                                                   values: collection.Select<object, object?>(element => property.GetValue(element))));

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

            return true;
        }

        static (IEnumerable?, int, PropertyInfo[]?) GetProperties(this object? @this)
        {
            var collection = @this as IEnumerable;
            if (collection is not null) {
                var collectionCount = collection.Count();
                if (collectionCount > 0) {
                    var enumerator = collection.GetEnumerator();
                    enumerator.MoveNext();
                    var type       = enumerator.Current.GetType();
                    var properties = type.GetProperties();

                    if (properties.Length > 0)
                        return (collection, collectionCount, properties);
                }
            }
            return (collection, 0, null);
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
                stringBuilder.Append(new string(FrameCharacters.HorizontalBar, 1 + column.Width + 1));
                stringBuilder.Append(FrameCharacters.Cross);
            });

            return stringBuilder.ToString();
        }
    }
}
