# Shos.Console
Shos.Console

## Summary

Console Library (.NET 6.0)

A library for Console.

## NuGet

You can install Shos.CsvHelper to your project with [NuGet](https://www.nuget.org) on Visual Studio.

* [NuGet Gallery | Shos.Console](https://www.nuget.org/packages/Shos.Console/)

### Package Manager

    PM>Install-Package Shos.Console -version 1.0.2

### .NET CLI

    >dotnet add package Shos.Console --version 1.0.2

### PackageReference

    <PackageReference Include="Shos.Console" Version="1.0.2" />

## Projects

### GridView

* Draw a collection as a table to console
* Support for full-width and half-width characters (所謂全角・半角文字対応)
* for .NET 6.0 or later

## Sample

See the sample: https://github.com/Fujiwo/Shos.Console/blob/main/Shos.Console.Sample/Program.cs

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
                dataSource: staffs.Select(staff => new { 名前 = staff.Name, 番号 = staff.Number, 部署 = staff.Department?.Name ?? "" }),
                hasFrame: true
            );

            System.Console.WriteLine("(2.3) staffs.Select(...).ShowTable() | Extension method version");

            // Extension method version
            staffs.OrderBy(staff => staff.Number)
                  .Select(staff => new { 名前 = staff.Name, 番号 = staff.Number, 部署 = staff.Department?.Name ?? "" })
                  .ShowTable();
        }

        static void Main()
        {
            new System.Action[] { 英数字記号のみの場合, 所謂全角半角混じりの場合, クラス利用の場合, クラスを2つ利用した場合 }
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

Result:

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
+-------------+------+--------+
| 名前        | 番号 | 部署   |
+-------------+------+--------+
| 西村 要     |    3 | 人事部 |
| 川村 咲     |  101 | 経理部 |
| 東 さくら   |   40 | 経理部 |
| 柴咲 育三郎 |   27 | R&D室  |
+-------------+------+--------+
(2.3) staffs.Select(...).ShowTable() | Extension method version
+-------------+------+--------+
| 名前        | 番号 | 部署   |
+-------------+------+--------+
| 西村 要     |    3 | 人事部 |
| 柴咲 育三郎 |   27 | R&D室  |
| 東 さくら   |   40 | 経理部 |
| 川村 咲     |  101 | 経理部 |
+-------------+------+--------+

```

## Author Info

Fujio Kojima: a software developer in Japan
* Microsoft MVP for Development Tools - Visual C# (Jul. 2005 - Dec. 2014)
* Microsoft MVP for .NET (Jan. 2015 - Oct. 2015)
* Microsoft MVP for Visual Studio and Development Technologies (Nov. 2015 - Jun. 2018)
* Microsoft MVP for Developer Technologies (Nov. 2018 - Jun. 2022)
* [MVP Profile](https://mvp.microsoft.com/en-us/PublicProfile/21482 "MVP Profile")
* [Blog (Japanese)](http://wp.shos.info "Blog (Japanese)")
* [Web Site (Japanese)](http://www.shos.info "Web Site (Japanese)")
* [Twitter](https://twitter.com/Fujiwo)
* [Instagram](https://www.instagram.com/fujiwo/)

## License

This library is under the MIT License.
