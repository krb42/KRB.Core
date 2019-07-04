namespace KRB.Core.Toolset.Extensions
{
   public static class IntExtensions
   {
      /// <summary>
      /// Unique marriage of two ints https://en.wikipedia.org/wiki/Cantor_pairing_function
      /// </summary>
      /// <param name="value">
      /// </param>
      /// <param name="partner">
      /// </param>
      /// <returns>
      /// </returns>
      public static int CantorPair(this int value, int partner)
      {
         return (((value + partner) * (value + partner + 1)) / 2) + partner;
      }
   }
}
