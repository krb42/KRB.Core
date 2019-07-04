using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace KRB.Core.Toolset.Extensions
{
   public enum TypeFilterConstraints
   {
      Default,
      Strict,
      Derived
   }

   public static class TypeExtensions
   {
      private static IEnumerable<TResult> OfTypeDerivedIterator<TResult>(IEnumerable source)
      {
         foreach (var obj in source)
         {
            if (obj.Is<TResult>(TypeFilterConstraints.Derived)) yield return (TResult)obj;
         }
      }

      private static IEnumerable<TResult> OfTypeStrictIterator<TResult>(IEnumerable source)
      {
         foreach (var obj in source)
         {
            if (obj.Is<TResult>(TypeFilterConstraints.Strict)) yield return (TResult)obj;
         }
      }

      public static IEnumerable<T> GetCustomAttributes<T>(this PropertyInfo pi, bool inherit) where T : Attribute
      {
         return pi.GetCustomAttributes(typeof(T), inherit).OfType<T>();
      }

      public static bool Is<T>(this object o)
      {
         return o is T;
      }

      public static bool Is(this object o, Type t)
      {
         return o.IsInstanceOfType(t);
      }

      public static bool Is(this Type t1, Type t2)
      {
         return t2.GetTypeInfo().IsAssignableFrom(t1);
      }

      public static bool Is<T>(this object o, TypeFilterConstraints constraint)
      {
         switch (constraint)
         {
            case TypeFilterConstraints.Default:
               return o is T;

            case TypeFilterConstraints.Strict:
               return o.GetType() == typeof(T);

            case TypeFilterConstraints.Derived:
               return !(o is T) && o.GetType() == typeof(T);

            default:
               throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
         }
      }

      public static bool Is(this object o, Type t, TypeFilterConstraints constraint)
      {
         switch (constraint)
         {
            case TypeFilterConstraints.Default:
               return o.GetType().IsInstanceOfType(t);

            case TypeFilterConstraints.Strict:
               return o.GetType() == t;

            case TypeFilterConstraints.Derived:
               return o.GetType() != t && o.IsInstanceOfType(t);

            default:
               throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
         }
      }

      public static bool Is(this Type t1, Type t2, TypeFilterConstraints constraint)
      {
         switch (constraint)
         {
            case TypeFilterConstraints.Default:
               return t2.GetTypeInfo().IsAssignableFrom(t1.GetTypeInfo());

            case TypeFilterConstraints.Strict:
               return t1 == t2;

            case TypeFilterConstraints.Derived:
               return t1 != t2 && t2.GetTypeInfo().IsAssignableFrom(t1.GetTypeInfo());

            default:
               throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
         }
      }

      // Not always bundled with .NET, so equivalent retrieved from: https://stackoverflow.com/a/24712250
      public static bool IsInstanceOfType(this Type type1, Type type2)
      {
         return type1 != null && type2 != null && type2.GetTypeInfo().IsAssignableFrom(type1);
      }

      // Not always bundled with .NET, so equivalent retrieved from: https://stackoverflow.com/a/24712250
      public static bool IsInstanceOfType<T>(this Type type)
      {
         return type != null && type.GetTypeInfo().IsAssignableFrom(typeof(T));
      }

      // Not always bundled with .NET, so equivalent retrieved from: https://stackoverflow.com/a/24712250
      public static bool IsInstanceOfType(this object obj, Type type)
      {
         return obj != null && type != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType());
      }

      // Not always bundled with .NET, so equivalent retrieved from: https://stackoverflow.com/a/24712250
      public static bool IsInstanceOfType<T>(this object obj)
      {
         return obj != null && typeof(T).GetTypeInfo().IsAssignableFrom(obj.GetType());
      }

      public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source, TypeFilterConstraints constraint)
      {
         if (source == null) throw new NullReferenceException(nameof(source));
         switch (constraint)
         {
            case TypeFilterConstraints.Default:
               return source.OfType<TResult>();

            case TypeFilterConstraints.Strict:
               return OfTypeStrictIterator<TResult>(source);

            case TypeFilterConstraints.Derived:
               return OfTypeDerivedIterator<TResult>(source);

            default:
               throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
         }
      }

      /// <summary>
      /// Returns an object of a specified type
      /// </summary>
      public static T To<T>(this string obj)
      {
         if (typeof(T) == typeof(Guid) || typeof(T) == typeof(TimeSpan))
         {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj);
         }

         return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
      }

      /// <summary>
      /// Returns an object of a specified type
      /// </summary>
      public static T ToOrDefault<T>(this string obj, T defaultValue)
      {
         T result;
         try
         {
            result = To<T>(obj);
         }
         catch (FormatException)
         {
            result = defaultValue != null ? defaultValue : default(T);
         }

         return result;
      }

      /// <summary>
      /// Returns an object of a specified type
      /// </summary>
      public static T ToOrDefault<T>(this string obj)
      {
         T result;
         try
         {
            result = To<T>(obj);
         }
         catch (Exception)
         {
            result = default(T);
         }

         return result;
      }

      public static bool TypeMatch(this IEnumerable<Type> types, Type type)
      {
         return types.Any(type.Is);
      }

      public static bool TypeMatch(this IEnumerable<Type> types, Type type, TypeFilterConstraints constraint)
      {
         return types.Any(t => type.Is(t, constraint));
      }
   }
}
