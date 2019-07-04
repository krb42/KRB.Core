using KRB.Core.Toolset.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRB.Core.Toolset.Test
{
   [TestClass]
   public class ToolsetIntExtensionTests
   {
      [TestMethod]
      public void CoreToolset_TestIntExtensionCantorPairing()
      {
         var val1 = 18;
         var val2 = 9;
         var val3 = 1;
         var val4 = 89;

         var result1 = val1.CantorPair(val2);
         var result2 = val2.CantorPair(val1);
         var result3 = val3.CantorPair(val4);
         Assert.IsTrue(result1 != result2);
         Assert.IsTrue(result1 != result3);
         Assert.IsTrue(result2 != result3);
      }
   }
}
