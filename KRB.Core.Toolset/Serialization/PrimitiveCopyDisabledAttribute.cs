using System;
using System.Linq;
using System.Reflection;

namespace KRB.Core.Toolset.Serialization
{
   public static class PrimitiveCopyDisabledAttributeExtensions
   {
      public static bool CopyDisabled(this FieldInfo fi)
      {
         return fi.GetCustomAttributes(typeof(PrimitiveCopyDisabledAttribute), true).Any();
      }

      public static bool CopyDisabled(this PropertyInfo pi)
      {
         return pi.GetCustomAttributes(typeof(PrimitiveCopyDisabledAttribute), true).Any();
      }
   }

   [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
   public class PrimitiveCopyDisabledAttribute : Attribute
   {
   }
}
