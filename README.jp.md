# Shos.Console

[English](https://github.com/Fujiwo/Shos.Console/blob/main/README.md) | [日本語](https://github.com/Fujiwo/Shos.Console/blob/main/README.jp.md)

## 概要

コンソール用ライブラリ (.NET 8.0 / .NET 10.0)

コンソール アプリケーション向けのライブラリです。

* GridView: コレクションを表形式でコンソールに表示
* ConsoleHelper: コンソールからのパスワード入力と、そのハッシュ化・検証
* ColorSetter: コンソールの前景色・背景色を一時的に変更
* JapaneseTextNormalizer: 日本語テキストの正規化

## 対応フレームワーク

* .NET 8.0 (`net8.0`)
* .NET 10.0 (`net10.0`)

NuGet パッケージには両方のターゲット フレームワーク向けのアセンブリが含まれています。

## NuGet

Visual Studio などから [NuGet](https://www.nuget.org) を使ってプロジェクトに Shos.Console をインストールできます。

* [NuGet Gallery | Shos.Console](https://www.nuget.org/packages/Shos.Console/)

### パッケージ マネージャー

    PM>Install-Package Shos.Console -version 1.1.6

### .NET CLI

    >dotnet add package Shos.Console --version 1.1.6

### PackageReference

    <PackageReference Include="Shos.Console" Version="1.1.6" />

## プロジェクト構成

### Shos.Console

ライブラリ本体 (.NET 8.0 および .NET 10.0 対応)。

#### GridView

* コレクションを表形式でコンソールに表示
* 所謂全角・半角文字混在に対応 (表示幅を考慮して桁を揃えます)
* カラム名ごとにデータを渡すことも可能 (`IEnumerable<(string, IEnumerable<object?>)>`)
* 拡張メソッド版: `ShowTable()`

#### ConsoleHelper

* `public static string ReadPassword(string message)`
    * コンソールからパスワードを入力し、ソルト付き PBKDF2-SHA256 ハッシュを返します。
* `public static string HashPassword(string password)`
    * コンソールを使わずに、ソルト付き PBKDF2-SHA256 ハッシュを生成します。
* `public static bool VerifyPassword(string password, string passwordHash)`
    * `ReadPassword` または `HashPassword` が返したハッシュとパスワードを照合します。
    * 旧バージョンが返したハッシュ値 (ソルトなし SHA-256) とは互換性がないため、移行が必要です。

#### ColorSetter

* コンソールの前景色・背景色を一時的に設定するクラスです。

#### JapaneseTextNormalizer

* 不適切な ASCII 文字や半角カタカナを適切な形式に変換し、日本語テキストを正規化するメソッドを提供します。

### Shos.Console.Sample

Shos.Console のサンプル (.NET 8.0 および .NET 10.0 対応)。

### Shos.ConsoleTests

Shos.Console のテスト (MSTest、.NET 8.0 および .NET 10.0 対応)。

## ビルドとテスト

.NET 10 SDK が必要です (`net8.0` ターゲットもビルドできます)。
各ターゲット フレームワークでテストを実行するには、対応する .NET ランタイムが必要です。

    >dotnet build Shos.Console.sln
    >dotnet test Shos.Console.sln

特定のフレームワークのみで実行する場合:

    >dotnet test Shos.Console.sln -f net10.0

## リリース ノート

### 1.1.6

* .NET 8.0 と .NET 10.0 の両方に対応 (マルチターゲット)。
* `ConsoleHelper`: パスワード ハッシュをソルト付き PBKDF2-SHA256 に変更。`HashPassword` と `VerifyPassword` を追加。
* `GridView`: 入力シーケンスの列挙を 1 回のみに変更、インデクサー プロパティを除外、先頭行が null の場合にも対応。
* `JapaneseTextNormalizer`: 全角波ダッシュ・全角チルダを `~` に正規化。

## サンプル

サンプル: https://github.com/Fujiwo/Shos.Console/blob/main/Shos.Console.Sample/Program.cs

```Shos.Console.Sample/Program.cs

using System.Collections.Generic;
using System.Linq;

namespace Shos.Console.Sample
{
    // Japanese (日本語)
    class Program
    {
        static void 英数字記号のみの場合()
        {
            var staffs = new[] {
                new { Number = 101, Name = "Takuro Yoshida" , Email = "takuro.y@xxx.com"  , Score =   0.0 },
                new { Number = 111, Name = "Miyuki Nakajima", Email = "m.nakajima@xxx.com", Score =   8.3 },
                new { Number = 120, Name = "Sho Kiryuin"    , Email = "eiichi@xxx.com"    , Score =  99.7 },
                new { Number =   9, Name = "Kana Nishino"   , Email = "kana@xxx.com"      , Score = 100.0 }
            }.OrderBy(staff => staff.Number);

            GridView.Show(dataSource: staffs);
        }

        static void 所謂全角半角混じりの場合()
        {
            var 全社員 = new[] {
                new { 社員番号 = 101, 氏名 = "吉田 拓郎  ", ﾌﾘｶﾞﾅ = "ﾖｼﾀﾞ ﾀｸﾛｳ" , ﾒｰﾙ = "takuro.y@xxx.com"  , 点数 =   0.0 },
                new { 社員番号 = 111, 氏名 = "中島 みゆき", ﾌﾘｶﾞﾅ = "ﾅｶｼﾞﾏ ﾐﾕｷ" , ﾒｰﾙ = "m.nakajima@xxx.com", 点数 =   8.3 },
                new { 社員番号 = 120, 氏名 = "鬼龍院 翔"  , ﾌﾘｶﾞﾅ = "ｷﾘｭｳｲﾝ ｼｮｳ", ﾒｰﾙ = "eiichi@xxx.com"    , 点数 =  99.7 },
                new { 社員番号 =   9, 氏名 = "西野 かな"  , ﾌﾘｶﾞﾅ = "ﾆｼﾉ ｶﾅ"    , ﾒｰﾙ = "kana@xxx.com"      , 点数 = 100.0 }
            }.OrderBy(社員 => 社員.氏名);

            GridView.Show(dataSource: 全社員, hasFrame: true);
        }

        class Staff
        {
            public int    Number         { get; set; }
            public string FamilyName     { get; set; } = "";
            public string GivenName      { get; set; } = "";
            public string FullName => $"{FamilyName} {GivenName}";
            public string FamilyNameRuby { get; set; } = "";
            public string GivenNameRuby  { get; set; } = "";
            public string FullNameRuby => $"{FamilyNameRuby} {GivenNameRuby}";
            public string Email          { get; set; } = "";
        }

        static void クラス利用の場合()
        {
            var staffs = new List<Staff> {
                new Staff { Number = 101, FamilyName = "吉田"  , GivenName = "拓郎  ", FamilyNameRuby = "ﾖｼﾀﾞ"  , GivenNameRuby = "ﾀｸﾛｳ", Email = "takuro.y@xxx.com"   },
                new Staff { Number = 111, FamilyName = "中島"  , GivenName = "みゆき", FamilyNameRuby = "ﾅｶｼﾞﾏ" , GivenNameRuby = "ﾐﾕｷ" , Email = "m.nakajima@xxx.com" },
                new Staff { Number =  12, FamilyName = "鬼龍院", GivenName = "翔"    , FamilyNameRuby = "ｷﾘｭｳｲﾝ", GivenNameRuby = "ｼｮｳ" , Email = "eiichi@xxx.com"     },
                new Staff { Number =   9, FamilyName = "西野"  , GivenName = "かな"  , FamilyNameRuby = "ﾆｼﾉ"   , GivenNameRuby = "ｶﾅ"  , Email = "kana@xxx.com"       }
            };

            var scoreTable = new Dictionary<int, double> {
                { 101, 100.0 },
                { 111,  80.0 },
                {  12,   8.0 },
                {   9,   0.0 }
            };

            GridView.Show(
                dataSource: staffs.OrderBy(staff => staff.Number)
                                  .Select(staff => new {
                    社員番号 = staff.Number      ,
                    氏名     = staff.FullName    ,
                    ﾌﾘｶﾞﾅ    = staff.FullNameRuby,
                    ﾒｰﾙ      = staff.Email       ,
                    得点     = scoreTable[staff.Number]
                }),
                hasFrame: true
            );
        }

        class Department
        {
            public int Code { get; set; }
            public string Name { get; set; } = "";
        }

        class Employee
        {
            public string Name { get; set; } = "";
            public int Number { get; set; }
            public Department? Department { get; set; }
        }

        static void クラスを2つ利用した場合()
        {
            var departments = new[] {
                new Department { Code = 1001, Name = "人事部"        },
                new Department { Code =  501, Name = "経理部"        },
                new Department { Code = 3001, Name = "R&D室"         },
                new Department { Code =   27, Name = "土木開発事業部"}
            };

            var staffs = new[] {
                new Employee { Name = "西村 要"    , Number =   3, Department = departments[0] },
                new Employee { Name = "川村 咲"    , Number = 101, Department = departments[1] },
                new Employee { Name = "東 さくら"  , Number =  40, Department = departments[1] },
                new Employee { Name = "柴咲 育三郎", Number =  27, Department = departments[2] }
            };

            System.Console.WriteLine("(1) GridView.Show(departments)");

            GridView.Show(dataSource: departments, hasFrame: true);

            System.Console.WriteLine("(2.1) GridView.Show(staffs)");

            GridView.Show(dataSource: staffs, hasFrame: true);

            System.Console.WriteLine("(2.2) GridView.Show(staffs.Select(...))");

            GridView.Show(
                dataSource: staffs.Select(staff => new { 名前 = staff.Name, 番号 = staff.Number, 部署 = $"{staff.Department?.Name ?? ""}({staff.Department?.Code})" }),
                hasFrame: true
            );

            System.Console.WriteLine("(2.3) staffs.Select(...).ShowTable() | Extension method version");

            // Extension method version
            staffs.OrderBy(staff => staff.Number)
                  .Select(staff => new { 名前 = staff.Name, 番号 = staff.Number, 部署 = $"{staff.Department?.Name ?? ""}({staff.Department?.Code})" })
                  .ShowTable();
        }
        
        static void カラム名ごとにデータを渡す場合()
        {
            var 全社員 = new (string, IEnumerable<object?>)[] {
                ("社員番号", new object[] { 101, 111, 120, 9, 100 }),
                ("氏名"   , new object[] { "吉田 拓郎", "中島 みゆき", "鬼龍院 翔", "西野 かな" }),
                ("ﾌﾘｶﾞﾅ"   , new object[] { "ﾖｼﾀﾞ ﾀｸﾛｳ", "ﾅｶｼﾞﾏ ﾐﾕｷ", "ｷﾘｭｳｲﾝ ｼｮｳ", "ﾆｼﾉ ｶﾅ" }),
                ("ﾒｰﾙ"   , new object[] { "takuro.y@xxx.com", "m.nakajima@xxx.com", "eiichi@xxx.com", "kana@xxx.com" }),
            };

            GridView.Show(dataSource: 全社員, hasFrame: true);
        }
        struct MenuItem
        {
            public enum Kind { 社員, 部署, その他 };

            public string Key { get; set; }
            public string Title { get; set; }
        }

        static void カラム名ごとにデータを渡す場合応用編()
        {
            var menuItems = new (MenuItem, MenuItem.Kind)[] {
                (new MenuItem { Key = "1", Title = "検索" }, MenuItem.Kind.社員),
                (new MenuItem { Key = "2", Title = "追加" }, MenuItem.Kind.社員),
                (new MenuItem { Key = "3", Title = "削除" }, MenuItem.Kind.社員),
                (new MenuItem { Key = "4", Title = "変更" }, MenuItem.Kind.社員),
                (new MenuItem { Key = "1", Title = "検索" }, MenuItem.Kind.部署),
                (new MenuItem { Key = "2", Title = "追加" }, MenuItem.Kind.部署),
                (new MenuItem { Key = "3", Title = "削除" }, MenuItem.Kind.部署),
                (new MenuItem { Key = "4", Title = "変更" }, MenuItem.Kind.部署),
                (new MenuItem { Key = "1", Title = "保存" }, MenuItem.Kind.その他),
                (new MenuItem { Key = "2", Title = "終了" }, MenuItem.Kind.その他)
            };

            var groups = menuItems.GroupBy(x => x.Item2);
            var dataSource = groups.Select(x => (x.Key.ToString(), x.Select(_ => (object?)$"({_.Item1.Key}){_.Item1.Title}")));
            dataSource.ShowTable(hasFrame: true);
        }

        static void Main()
        {
            new System.Action[] { 英数字記号のみの場合, 所謂全角半角混じりの場合, クラス利用の場合, クラスを2つ利用した場合, カラム名ごとにデータを渡す場合, カラム名ごとにデータを渡す場合応用編 }
            .ForEach((index, test) =>
            {
                System.Console.WriteLine($"■ Test {index + 1}");
                test();
                System.Console.WriteLine();
            });
        }
    }
}

```

実行結果:

```

■ Test 1
Number Name            Email              Score
     9 Kana Nishino    kana@xxx.com       100.0
   101 Takuro Yoshida  takuro.y@xxx.com     0.0
   111 Miyuki Nakajima m.nakajima@xxx.com   8.3
   120 Sho Kiryuin     eiichi@xxx.com      99.7

■ Test 2
+----------+-------------+------------+--------------------+-------+
| 社員番号 | 氏名        | ﾌﾘｶﾞﾅ      | ﾒｰﾙ                | 点数  |
+----------+-------------+------------+--------------------+-------+
|      120 | 鬼龍院 翔   | ｷﾘｭｳｲﾝ ｼｮｳ | eiichi@xxx.com     |  99.7 |
|      101 | 吉田 拓郎   | ﾖｼﾀﾞ ﾀｸﾛｳ  | takuro.y@xxx.com   |   0.0 |
|        9 | 西野 かな   | ﾆｼﾉ ｶﾅ     | kana@xxx.com       | 100.0 |
|      111 | 中島 みゆき | ﾅｶｼﾞﾏ ﾐﾕｷ  | m.nakajima@xxx.com |   8.3 |
+----------+-------------+------------+--------------------+-------+

■ Test 3
+----------+-------------+------------+--------------------+-------+
| 社員番号 | 氏名        | ﾌﾘｶﾞﾅ      | ﾒｰﾙ                | 得点  |
+----------+-------------+------------+--------------------+-------+
|        9 | 西野 かな   | ﾆｼﾉ ｶﾅ     | kana@xxx.com       |   0.0 |
|       12 | 鬼龍院 翔   | ｷﾘｭｳｲﾝ ｼｮｳ | eiichi@xxx.com     |   8.0 |
|      101 | 吉田 拓郎   | ﾖｼﾀﾞ ﾀｸﾛｳ  | takuro.y@xxx.com   | 100.0 |
|      111 | 中島 みゆき | ﾅｶｼﾞﾏ ﾐﾕｷ  | m.nakajima@xxx.com |  80.0 |
+----------+-------------+------------+--------------------+-------+

■ Test 4
(1) GridView.Show(departments)
+------+----------------+
| Code | Name           |
+------+----------------+
| 1001 | 人事部         |
|  501 | 経理部         |
| 3001 | R&D室          |
|   27 | 土木開発事業部 |
+------+----------------+
(2.1) GridView.Show(staffs)
+-------------+--------+----------------------------------------+
| Name        | Number | Department                             |
+-------------+--------+----------------------------------------+
| 西村 要     |      3 | Shos.Console.Sample.Program+Department |
| 川村 咲     |    101 | Shos.Console.Sample.Program+Department |
| 東 さくら   |     40 | Shos.Console.Sample.Program+Department |
| 柴咲 育三郎 |     27 | Shos.Console.Sample.Program+Department |
+-------------+--------+----------------------------------------+
(2.2) GridView.Show(staffs.Select(...))
+-------------+------+--------------+
| 名前        | 番号 | 部署         |
+-------------+------+--------------+
| 西村 要     |    3 | 人事部(1001) |
| 川村 咲     |  101 | 経理部(501)  |
| 東 さくら   |   40 | 経理部(501)  |
| 柴咲 育三郎 |   27 | R&D室(3001)  |
+-------------+------+--------------+
(2.3) staffs.Select(...).ShowTable() | Extension method version
+-------------+------+--------------+
| 名前        | 番号 | 部署         |
+-------------+------+--------------+
| 西村 要     |    3 | 人事部(1001) |
| 柴咲 育三郎 |   27 | R&D室(3001)  |
| 東 さくら   |   40 | 経理部(501)  |
| 川村 咲     |  101 | 経理部(501)  |
+-------------+------+--------------+

■ Test 5
+----------+-------------+------------+--------------------+
| 社員番号 | 氏名        | ﾌﾘｶﾞﾅ      | ﾒｰﾙ                |
+----------+-------------+------------+--------------------+
|      101 | 吉田 拓郎   | ﾖｼﾀﾞ ﾀｸﾛｳ  | takuro.y@xxx.com   |
|      111 | 中島 みゆき | ﾅｶｼﾞﾏ ﾐﾕｷ  | m.nakajima@xxx.com |
|      120 | 鬼龍院 翔   | ｷﾘｭｳｲﾝ ｼｮｳ | eiichi@xxx.com     |
|        9 | 西野 かな   | ﾆｼﾉ ｶﾅ     | kana@xxx.com       |
|      100 |             |            |                    |
+----------+-------------+------------+--------------------+
```

## 作者情報

Fujio Kojima: 日本のソフトウェア開発者
* Microsoft MVP for Development Tools - Visual C# (2005年7月 - 2014年12月)
* Microsoft MVP for .NET (2015年1月 - 2015年10月)
* Microsoft MVP for Visual Studio and Development Technologies (2015年11月 - 2018年6月)
* Microsoft MVP for Developer Technologies (2018年11月 - 2027年6月)
* [MVP Profile](https://mvp.microsoft.com/en-us/PublicProfile/21482 "MVP Profile")
* [ブログ](http://wp.shos.info "ブログ")
* [Web サイト](http://www.shos.info "Web サイト")
* [Twitter](https://twitter.com/Fujiwo)
* [Instagram](https://www.instagram.com/fujiwo/)

## ライセンス

このライブラリは MIT ライセンスの下で提供されています。
