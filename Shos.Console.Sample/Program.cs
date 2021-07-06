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

        static void Main()
        {
            new System.Action[] { 英数字記号のみの場合, 所謂全角半角混じりの場合, クラス利用の場合 }
            .ForEach(test =>
            {
                test();
                System.Console.WriteLine();
            });
        }
    }
}
