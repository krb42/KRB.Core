using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRB.Core.Toolset.Primitive;
using KRB.Core.Toolset.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRB.Core.Toolset.Test
{
   public static class OverrideExtensions
   {
      public static bool EqualsProperties<T>(this T source, T target)
      {
         var properties = source.GetType().GetProperties();
         return properties.All(pi => pi.GetValue(source, null).Equals(pi.GetValue(target, null)));
      }
   }

   [TestClass]
   public class ToolsetSerialisationTests
   {
      private class IntHaver
      {
         public int Value { get; set; }
      }

      internal class Bar : Foo
      {
         public double D { get; set; } = 200.00;

         public Foo E { get; set; } = null;

         public float F { get; set; } = 300.00f;
      }

      internal class Foo : PrimitiveCopyable
      {
         public string A { get; set; } = "A";

         public string B { get; set; } = "B";

         public int C { get; set; } = 100;
      }

      internal class FooNotImplemented : Foo
      {
         [PrimitiveCopyDisabled()]
         public string D { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

         public string E { get; set; } = "EE";
      }

      [TestMethod]
      public void CoreToolset_TestPrimitiveCopyFrom()
      {
         var foo = new FooNotImplemented { A = "apple", B = "bismuth", C = 2 };
         var bar = foo.PrimitiveClone();

         Assert.AreEqual(bar.A, foo.A);
         Assert.AreEqual(bar.B, foo.B);
         Assert.AreEqual(bar.C, foo.C);
         Assert.AreEqual(bar.E, foo.E);
      }

      [TestMethod]
      public void CoreToolset_TestPrimitiveEquals()
      {
         var a = new IntHaver { Value = 2 };
         var b = new IntHaver { Value = 2 };

         Assert.IsTrue(a.EqualsProperties(b));
      }

      [TestMethod]
      public void CoreToolset_TestPrimitivePropertyClone()
      {
         var foo = new Foo { A = "apple", B = "bismuth", C = 2 };
         var bar = foo.PrimitiveClone();

         Assert.AreEqual(bar.A, foo.A);
         Assert.AreEqual(bar.B, foo.B);
         Assert.AreEqual(bar.C, foo.C);
      }

      [TestMethod]
      public void CoreToolset_TestPrimitivePropertyCopy()
      {
         var foo = new Foo { A = "apple", B = "bismuth", C = 2 };
         var bar = new Bar();

         foo.CopyTo(bar);
         Assert.AreEqual(bar.A, foo.A);
         Assert.AreEqual(bar.B, foo.B);
         Assert.AreEqual(bar.C, foo.C);
      }

      [TestMethod]
      public void CoreToolset_TestPrimitivePropertyEquals()
      {
         var foo = new Foo { A = "apple", B = "bismuth", C = 2 };
         var bar = foo.PrimitiveClone();

         var equals = foo.PrimitiveEquals(bar);
         Assert.AreEqual(equals, true);
      }
   }
}
