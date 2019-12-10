using KRB.Core.Toolset.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRB.Core.Toolset.Primitive
{
   public static class PrimitiveCopyHelper
   {
      /// <summary>
      /// A primitive Clone implementation that scrapes public fields and properties and copies them
      /// to a new object. Will not target private fields/properties.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      public static T PrimitiveClone<T>(this T ťhis, bool copyPrivate = false)
      {
         var that = ťhis.NewInstance();
         that.PrimitiveCopyFrom(ťhis, copyPrivate);
         return that;
      }

      /// <summary>
      /// A primitive inverse CopyTo implementation that scrapes public fields and properties and
      /// copies them to the target object. Will not target private fields/properties.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <param name="that">
      /// </param>
      public static void PrimitiveCopyFrom<T>(this T ťhis, T that, bool copyPrivate = false)
      {
         CopyHelper.CopyProperties(that, ťhis, copyPrivate);
         CopyHelper.CopyFields(that, ťhis, copyPrivate);
      }

      /// <summary>
      /// A primitive inverse CopyTo implementation that scrapes public fields and properties and
      /// copies them to the target object of a different type.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <param name="that">
      /// </param>
      public static void PrimitiveCopySimilarFrom<TSource, TTarget>(this TSource ťhis, TTarget that, bool copyPrivate = false)
      {
         that.CopyPropertiesTo(ťhis, copyPrivate);
         that.CopyFieldsTo(ťhis, copyPrivate);
      }

      /// <summary>
      /// A primitive inverse CopyTo implementation that scrapes public fields and properties and
      /// copies them to the target object of a different type. Allows the user to define a mapping
      /// function to dictate how one object should map to another. E.g. append a character or
      /// prefix to every property name.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <param name="that">
      /// </param>
      /// <param name="mappingFunction">
      /// </param>
      public static void PrimitiveCopySimilarFrom<TSource, TTarget>(this TSource ťhis, TTarget that, Func<string, string, bool> mappingFunction, bool copyPrivate = false)
      {
         that.CopyPropertiesTo(ťhis, mappingFunction, copyPrivate);
         that.CopyFieldsTo(ťhis, mappingFunction, copyPrivate);
      }

      /// <summary>
      /// A primitive CopyTo implementation that scrapes matching public fields and properties and
      /// copies them to the target object of a different type.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <param name="that">
      /// </param>
      public static void PrimitiveCopySimilarTo<TSource, TTarget>(this TSource ťhis, TTarget that, bool copyPrivate = false)
      {
         ťhis.CopyPropertiesTo(that, copyPrivate);
         ťhis.CopyFieldsTo(that, copyPrivate);
      }

      /// <summary>
      /// A primitive CopyTo implementation that scrapes matching public fields and properties and
      /// copies them to the target object of a different type. Allows the user to define a mapping
      /// function to dictate how one object should map to another. E.g. append a character or
      /// prefix to every property name.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <param name="that">
      /// </param>
      /// <param name="mappingFunction">
      /// </param>
      public static void PrimitiveCopySimilarTo<TSource, TTarget>(this TSource ťhis, TTarget that, Func<string, string, bool> mappingFunction, bool copyPrivate = false)
      {
         ťhis.CopyPropertiesTo(that, mappingFunction, copyPrivate);
         ťhis.CopyFieldsTo(that, mappingFunction, copyPrivate);
      }

      /// <summary>
      /// A primitive CopyTo implementation that scrapes public fields and properties and copies
      /// them to the target object. Will not target private fields/properties.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <param name="that">
      /// </param>
      public static void PrimitiveCopyTo<T>(this T ťhis, T that, bool copyPrivate = false)
      {
         CopyHelper.CopyProperties(ťhis, that, copyPrivate);
         CopyHelper.CopyFields(ťhis, that, copyPrivate);
      }

      public static bool PrimitiveEquals<T>(this T ťhis, T that, bool comparePrivate = false)
      {
         return CopyHelper.EqualsProperties(ťhis, that, comparePrivate) && CopyHelper.EqualsFields(ťhis, that, comparePrivate);
      }
   }
}
