using KRB.Core.Toolset.Serialization;

namespace KRB.Core.Toolset.Primitive
{
   public static class PrimitiveCopyableExtensions
   {
      public static void CopyFrom<T>(this T ťhis, T that, bool copyPrivate = false) where T : PrimitiveCopyable
      {
         CopyHelper.CopyProperties(that, ťhis, copyPrivate);
         CopyHelper.CopyFields(that, ťhis, copyPrivate);
      }

      public static void CopyTo<T>(this T ťhis, T that, bool copyPrivate = false) where T : PrimitiveCopyable
      {
         CopyHelper.CopyProperties(ťhis, that, copyPrivate);
         CopyHelper.CopyFields(ťhis, that, copyPrivate);
      }
   }

   /// <summary>
   /// Not an empty class. Allows the user to use an extension method on things that subclass this.
   /// </summary>
   public abstract class PrimitiveCopyable
   {
   }
}
