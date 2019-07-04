using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace KRB.Core.Toolset.Extensions
{
   public static class GenericExtensions
   {
      private static DateTime Trim(this DateTime date, long roundTicks)
      {
         var trimmedDateTime = new DateTime(date.Ticks - date.Ticks % roundTicks);
         return trimmedDateTime;
      }

      public static List<T> EnqueueInList<T>(this T obj)
      {
         return new List<T> { obj };
      }

      /// <summary>
      /// Produces the set difference of two sequences by using the default equality comparer to
      /// compare values.
      /// </summary>
      /// <returns>
      /// A sequence that contains the set difference of the elements of two sequences.
      /// </returns>
      /// <param name="first">
      /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that are not
      /// also in <paramref name="second" /> will be returned.
      /// </param>
      /// <param name="second">
      /// An element that also occurs in the first sequence will cause those elements to be removed
      /// from the returned sequence.
      /// </param>
      /// <typeparam name="TSource">
      /// The type of the elements of the input sequences.
      /// </typeparam>
      /// <exception cref="T:System.ArgumentNullException">
      /// <paramref name="first" /> or <paramref name="second" /> is null.
      /// </exception>
      public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, TSource second)
      {
         return first.Except(second.EnqueueInList());
      }

      /// <summary>
      /// Produces the set difference of two sequences by using the specified
      /// <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.
      /// </summary>
      /// <returns>
      /// A sequence that contains the set difference of the elements of two sequences.
      /// </returns>
      /// <param name="first">
      /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that are not
      /// also in <paramref name="second" /> will be returned.
      /// </param>
      /// <param name="second">
      /// An element that also occurs in the first sequence will cause those elements to be removed
      /// from the returned sequence.
      /// </param>
      /// <param name="comparer">
      /// An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.
      /// </param>
      /// <typeparam name="TSource">
      /// The type of the elements of the input sequences.
      /// </typeparam>
      /// <exception cref="T:System.ArgumentNullException">
      /// <paramref name="first" /> or <paramref name="second" /> is null.
      /// </exception>
      public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, TSource second, IEqualityComparer<TSource> comparer)
      {
         return first.Except(second.EnqueueInList(), comparer);
      }

      [DebuggerStepThrough]
      public static void ForEach<T>(this IEnumerable<T> e, Action ac)
      {
         if (!e.IsNullOrEmpty()) for (var index = 0; index < e.Count(); index++) ac();
      }

      [DebuggerStepThrough]
      public static void ForEach<T>(this IEnumerable<T> e, Action<T> ac)
      {
         if (!e.IsNullOrEmpty()) foreach (var item in e) ac(item);
      }

      public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> selector)
      {
         if (source.IsNullOrEmpty()) return;
         var index = -1;
         foreach (var source1 in source)
         {
            checked { ++index; }
            selector(source1, index);
         }
      }

      public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
      {
         var type = value.GetType();
         var name = Enum.GetName(type, value);
         return type.GetRuntimeField(name)
             .GetCustomAttributes(false)
             .OfType<TAttribute>()
             .SingleOrDefault();
      }

      public static bool IsDefault<T>(this T value) where T : struct
      {
         return value.Equals(default(T));
      }

      public static bool IsDigit(this char c)
      {
         return char.IsDigit(c);
      }

      public static bool IsLetter(this char c)
      {
         return char.IsLetter(c);
      }

      public static bool IsLetterOrDigit(this char c)
      {
         return char.IsLetterOrDigit(c);
      }

      public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
      {
         return e == null || !e.Any();
      }

      public static T ThrowIfNull<T>(this T source, string name = null)
      {
         if (source == null)
            throw new ArgumentNullException(name);
         return source;
      }

      /// <summary>
      /// From Minetec.Mantle
      /// </summary>
      /// <param name="input">
      /// </param>
      /// <returns>
      /// </returns>
      public static DateTime? ToDateTime(this string input)
      {
         if (!DateTime.TryParse(input, out DateTime result))
         {
            return null;
         }

         return result;
      }

      /// <summary>
      /// From Minetec.Mantle
      /// </summary>
      /// <param name="input">
      /// </param>
      /// <returns>
      /// </returns>
      public static int ToInt32(this string input)
      {
         if (!int.TryParse(input, out int result))
         {
            return -1;
         }

         return result;
      }

      public static DateTime TrimToMinutes(this DateTime date)
      {
         var trimToMinutes = date.Trim(TimeSpan.TicksPerMinute);
         return trimToMinutes;
      }

      public static DateTime TrimToSeconds(this DateTime date)
      {
         var trimToSeconds = date.Trim(TimeSpan.TicksPerSecond);
         return trimToSeconds;
      }
   }
}
