using System;
using System.Linq;

namespace KRB.Core.Toolset.Test
{
   public static class TestingConstants
   {
      public static char[] InvalidCharacters =
      {
            '<', '>', '"', '\'', '`', '&', '^', '%', ':', ';', '\\', '/', '|', '?'
        };

      public static DateTime[] InvalidDateTimes = {
            new DateTime(0001, 01, 01, 0, 0, 0), // too low
            DateTime.Now.AddYears(1) // too high
        };

      public static string[] InvalidStrings = new[]
      {
            string.Concat(Enumerable.Repeat("a", 257)),   // length
            string.Concat(Enumerable.Repeat("b", 1000)),   // length
            string.Concat(Enumerable.Repeat("c", 10000)),   // length
            string.Empty,   // empty input
            "\a\b\f\n\r\t\v",    // special characters
            "¦©®°҂֍֎؎؏۞۩۽۾߶৺୰௳௴௵௶௷௸௺౿൏൹༁༂༃༓༕༖༗༚༛༜༝༞༟༴༶༸྾྿࿀࿁࿂࿃࿄࿅࿇࿈࿉࿊࿋࿌࿎࿏࿕࿖࿗࿘႞႟᎐᎑᎒᎓᎔᎕᎖᎗᎘᎙᥀᧞᧟᧠᧡᧢᧣᧤᧥᧦᧧᧨᧩᧪᧫᧬᧭᧮᧯᧰᧱᧲᧳᧴",   // symbols
            "使語施遺案政暮態青見暮循吐警第住江特夜横。当告出社創集就氷野京不柏使感。刊性全康訪受有聞日企維能投。用亡川仕救月言害見審氏川京",   // Chinese
            "太ゃびか換載せ必字せだん位病潟ッのも辺端基ラヲ建元ぐは写講ぴ掲構属ンつぎ国席さゆ国肪そうわン辺成ヲイオル止単航ツシ影年マレモ尚6職ユカ転宿句せ。",  // Japanese
            "SELECT * FROM *"   // sql
        }.Concat(InvalidCharacters.Select(c => c.ToString())).ToArray();

      public static DateTime ValidDate = new DateTime(1986, 5, 22, 6, 45, 12);
      public static string ValidString = "Test String";
   }
}
