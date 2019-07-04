using KRB.Core.Toolset.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace KRB.Core.Toolset.Test
{
   public class Bar : Foo
   {
   }

   public class Foo
   {
   }

   public class Tapioca : Bar
   {
   }

   [TestClass]
   public class ToolsetMiscellaneousTests
   {
      [TestMethod]
      public void CoreToolset_ForEachTest()
      {
         var list = new List<string>
            {
                "test",
                "test1",
                "test2",
                "test3"
            };
         list.ForEach((item, index) => Console.WriteLine(index + " - " + item));
      }

      [TestMethod]
      public void CoreToolset_IsTest()
      {
         var foo = new Foo();
         var bar = new Bar();
         var tapioca = new Tapioca();
         var objis1 = foo.Is(typeof(Tapioca));   // false
         var objis2 = bar.Is(typeof(Tapioca));   // false
         var objis3 = tapioca.Is(typeof(Tapioca));   // true
         var objis4 = foo.Is(typeof(Foo));   // true
         var objis5 = foo.Is(typeof(Bar));   // false
         var objis6 = foo.Is(typeof(Tapioca));   // false
         var objis7 = bar.Is(typeof(Foo));   // true
         var objis8 = bar.Is(typeof(Bar));   // true
         var objis9 = bar.Is(typeof(Tapioca));   // false
         var objis10 = tapioca.Is(typeof(Foo));   // true
         var objis11 = tapioca.Is(typeof(Bar));   // true
         var objis12 = tapioca.Is(typeof(Tapioca));   // true
      }

      [TestMethod]
      [Ignore]
      public void CoreToolset_TypeMatchTest()
      {
         var types = new[]
         {
                typeof(short),
                typeof(bool),
                typeof(string)
            };

         var match = types.TypeMatch(typeof(Timer));
         var match1 = types.TypeMatch("sofa".GetType());

         var moreTypes = new[]
         {
                typeof(Foo),
                typeof(float)
            };

         var match2 = moreTypes.TypeMatch(typeof(Tapioca));
         var match3 = moreTypes.TypeMatch(typeof(Tapioca), TypeFilterConstraints.Strict);

         Assert.AreEqual(match && match2, true);
      }

      [TestMethod]
      public void ToType_ConvertBool_ShouldConvert()
      {
         var obj = "true";
         var result = obj.To<bool>();
         Assert.IsTrue(result);
      }

      [TestMethod]
      [ExpectedException(typeof(FormatException))]
      public void ToType_ConvertBool_ShouldThrow()
      {
         var obj = TestingConstants.InvalidStrings.First();
         var result = obj.To<bool>();
         Assert.IsTrue(result);
      }

      [TestMethod]
      public void ToType_ConvertEmptyStringToBool_ShouldReturnDefault()
      {
         var obj = "";
         var result = obj.ToOrDefault<bool>();
         Assert.IsFalse(result);
      }

      [TestMethod]
      public void ToType_ConvertEmptyStringToBool_ShouldReturnTrueDefault()
      {
         var obj = "";
         var result = obj.ToOrDefault<bool>(true);
         Assert.IsTrue(result);
      }

      [TestMethod]
      public void ToType_ConvertInt_ShouldConvert()
      {
         var obj = "5";
         var result = obj.To<int>();
         Assert.AreEqual(5, result);
      }

      [TestMethod]
      public void ToType_convertIntFromEmptyString_ShouldReturnProvidedDefault()
      {
         double defaultDuration = 12;
         var setting = "".ToOrDefault(defaultDuration);
         Assert.AreEqual(12, setting);
      }

      [TestMethod]
      public void ToType_ConvertTimeSpace_ShouldConvert()
      {
         var obj = "06:00:00";
         var result = obj.To<TimeSpan>();
         Assert.AreEqual(new TimeSpan(6, 0, 0), result);
      }
   }
}
