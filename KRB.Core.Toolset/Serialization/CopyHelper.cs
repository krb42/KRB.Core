using KRB.Core.Toolset.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KRB.Core.Toolset.Serialization
{
   public static class CopyHelper
   {
      private static BindingFlags ResolveFlags(bool includePublic, bool includePrivate)
      {
         var flags = BindingFlags.Default;
         if (includePublic) flags |= BindingFlags.Public;
         if (includePrivate) flags |= BindingFlags.NonPublic;
         flags = flags | BindingFlags.Instance | BindingFlags.Static;
         return flags;
      }

      /// <summary>
      /// Copies all fields from one object of a certain type to another object of the same type.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="source">
      /// </param>
      /// <param name="target">
      /// </param>
      public static void CopyFields<T>(T source, T target, bool copyPrivate = false)
      {
         var flags = ResolveFlags(true, copyPrivate);
         var fields = source.GetType().GetFields(flags).Where(fi => !fi.CopyDisabled());
         foreach (var fi in fields) fi.SetValue(target, fi.GetValue(source));
      }

      /// <summary>
      /// Copies all readable and writeable fields from one object of a certain type to another
      /// object of a different type where they happen to match a provided predicate and in field
      /// type. This allows you to map methods from one type onto another type in the manner you
      /// choose. Use wisely.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="source">
      /// </param>
      /// <param name="dest">
      /// </param>
      /// <param name="copyPrivate">
      /// </param>
      public static void CopyFieldsTo<TSource, TTarget>(this TSource source, TTarget dest, bool copyPrivate = false)
      {
         var flags = ResolveFlags(true, copyPrivate);
         var fieldsSource = typeof(TSource).GetFields(flags);
         var fieldsTarget = typeof(TTarget).GetFields(flags);

         foreach (var destfield in fieldsTarget)
         {
            var sourceprops = fieldsSource.Where((p) => p.Name == destfield.Name && destfield.FieldType.IsInstanceOfType(p));
            foreach (var sourceprop in sourceprops) destfield.SetValue(dest, sourceprop.GetValue(source));
         }
      }

      /// <summary>
      /// Copies all readable and writeable fields from one object of a certain type to another
      /// object of a different type where they happen to match a provided predicate and in field
      /// type. This allows you to map methods from one type onto another type in the manner you
      /// choose. Use wisely.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="source">
      /// </param>
      /// <param name="dest">
      /// </param>
      /// <param name="comparisonFunction">
      /// </param>
      /// <param name="copyPrivate">
      /// </param>
      public static void CopyFieldsTo<TSource, TTarget>(this TSource source, TTarget dest, Func<string, string, bool> comparisonFunction, bool copyPrivate = false)
      {
         var flags = ResolveFlags(true, copyPrivate);
         var fieldsSource = typeof(TSource).GetFields(flags);
         var fieldsTarget = typeof(TTarget).GetFields(flags);

         foreach (var destfield in fieldsTarget)
         {
            //var sourceprops = fieldsSource.Where((p) => comparisonFunction(p.Name, destfield.Name) && destfield.FieldType.IsInstanceOfType(p));
            var sourceprops = fieldsSource.Where((p) => comparisonFunction(p.Name, destfield.Name) && destfield.FieldType.Is(p.FieldType));
            foreach (var sourcefield in sourceprops) destfield.SetValue(dest, sourcefield.GetValue(source));
         }
      }

      /// <summary>
      /// Copies data into the current object from the provided object. Copies fields and properties.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <param name="that">
      /// </param>
      public static void CopyFrom<T>(this T ťhis, T that) where T : ICopyable<T>
      {
         that.CopyTo(ťhis);
      }

      /// <summary>
      /// Copies all readable and writeable properties from one object of a certain type to another
      /// object of the same type.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="source">
      /// </param>
      /// <param name="target">
      /// </param>
      public static void CopyProperties<T>(T source, T target, bool copyPrivate = false)
      {
         var flags = ResolveFlags(true, copyPrivate);
         var properties = source.GetType().GetProperties(flags).Where(p => p.CanRead && p.CanWrite && !p.CopyDisabled());
         foreach (var pi in properties) pi.SetValue(target, pi.GetValue(source, null), null);
      }

      /// <summary>
      /// Copies all readable and writeable properties from one object of a certain type to another
      /// object of a different type where they happen to match in name and property type.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="source">
      /// </param>
      /// <param name="dest">
      /// </param>
      /// <param name="copyPrivate">
      /// </param>
      public static void CopyPropertiesTo<TSource, TTarget>(this TSource source, TTarget dest, bool copyPrivate = false)
      {
         var flags = ResolveFlags(true, copyPrivate);
         var propertiesSource = typeof(TSource).GetProperties(flags).Where(prop1 => prop1.CanRead && !prop1.CopyDisabled());
         var propertiesTarget = typeof(TTarget).GetProperties(flags).Where(prop2 => prop2.CanWrite && !prop2.CopyDisabled());

         foreach (var destprop in propertiesTarget)
         {
            //var sourceprops = propertiesSource.Where((p) => string.Equals(p.Name, destprop.Name) && destprop.PropertyType.IsInstanceOfType(p));
            var sourceprops = propertiesSource.Where((p) => string.Equals(p.Name, destprop.Name) && destprop.PropertyType.Is(p.PropertyType));
            foreach (var sourceprop in sourceprops) destprop.SetValue(dest, sourceprop.GetValue(source, null), null);
         }
      }

      /// <summary>
      /// Copies all readable and writeable properties from one object of a certain type to another
      /// object of a different type where they happen to match a provided predicate and in property
      /// type. This allows you to map methods from one type onto another type in the manner you
      /// choose. Use wisely.
      /// </summary>
      /// <typeparam name="TSource">
      /// </typeparam>
      /// <typeparam name="TTarget">
      /// </typeparam>
      /// <param name="source">
      /// </param>
      /// <param name="dest">
      /// </param>
      /// <param name="comparisonFunction">
      /// </param>
      /// <param name="copyPrivate">
      /// </param>
      public static void CopyPropertiesTo<TSource, TTarget>(this TSource source, TTarget dest, Func<string, string, bool> comparisonFunction, bool copyPrivate = false)
      {
         var flags = ResolveFlags(true, copyPrivate);
         var propertiesSource = typeof(TSource).GetProperties(flags).Where(prop1 => prop1.CanRead);
         var propertiesTarget = typeof(TTarget).GetProperties(flags).Where(prop2 => prop2.CanWrite);

         foreach (var destprop in propertiesTarget)
         {
            var sourceprops = propertiesSource.Where((p) => comparisonFunction(p.Name, destprop.Name) && destprop.PropertyType.IsInstanceOfType(p));
            foreach (var sourceprop in sourceprops) destprop.SetValue(dest, sourceprop.GetValue(source, null), null);
         }
      }

      public static bool EqualsFields<T>(this T source, T target, bool comparePrivate = false)
      {
         var flags = ResolveFlags(true, comparePrivate);
         var fields = source.GetType().GetFields(flags);
         return fields.All(fi =>
         {
            var sourceValue = fi.GetValue(source);
            var targetValue = fi.GetValue(target);
            if (sourceValue == null && targetValue == null) return true;
            else if ((sourceValue == null && targetValue != null) || (sourceValue != null && targetValue == null)) return false;
            else return sourceValue.Equals(targetValue);
         });
      }

      public static bool EqualsProperties<T>(this T source, T target, bool comparePrivate = false)
      {
         var flags = ResolveFlags(true, comparePrivate);
         var properties = source.GetType().GetProperties(flags);
         return properties.All(pi =>
         {
            var sourceValue = pi.GetValue(source, null);
            var targetValue = pi.GetValue(target, null);
            if (sourceValue == null && targetValue == null) return true;
            else if ((sourceValue == null && targetValue != null) || (sourceValue != null && targetValue == null)) return false;
            else return sourceValue.Equals(targetValue);
         });
      }

      /// <summary>
      /// Creates a new object of the same type as a provided one. Only use this to make things
      /// you're allowed to.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <returns>
      /// </returns>
      public static T NewInstance<T>(this T ťhis)
      {
         return (T)Activator.CreateInstance(typeof(T));
      }

      /// <summary>
      /// Creates a new object of a provided type. Only use this to make things you're allowed to.
      /// </summary>
      /// <typeparam name="T">
      /// </typeparam>
      /// <param name="ťhis">
      /// </param>
      /// <returns>
      /// </returns>
      public static T NewInstance<T>(this Type ťhis)
      {
         return (T)Activator.CreateInstance(ťhis);
      }
   }
}
