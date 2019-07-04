using System;
using System.Globalization;
using System.Text;

namespace KRB.Core.Toolset
{
   /// <summary>
   /// Pinched from System.Windows.Media.Color.
   /// </summary>
   public struct Color : IFormattable, IEquatable<Color>
   {
      private const string c_scRgbFormat = "R";
      private readonly float[] nativeColorValue;
      private bool isFromScRgb;
      private MILColorF scRgbColor;
      private MILColor sRgbColor;

      private static byte ScRgbTosRgb(float val)
      {
         if (val <= 0.0)
            return 0;
         if (val <= 0.0031308)
            return (byte)(byte.MaxValue * (double)val * 12.9200000762939 + 0.5);
         if (val < 1.0)
            return (byte)(byte.MaxValue * (1.05499994754791 * Math.Pow(val, 5.0 / 12.0) - 0.0549999997019768) + 0.5);
         return byte.MaxValue;
      }

      private static float SRgbToScRgb(byte bval)
      {
         var num = bval / (float)byte.MaxValue;
         if (num <= 0.0)
            return 0.0f;
         if (num <= 0.04045)
            return num / 12.92f;
         if (num < 1.0)
            return (float)Math.Pow((num + 0.055) / 1.055, 2.4);
         return 1f;
      }

      private struct MILColor
      {
         public byte a;
         public byte b;
         public byte g;
         public byte r;
      }

      private struct MILColorF
      {
         public float a;
         public float b;
         public float g;
         public float r;
      }

      internal static Color FromUInt32(uint argb)
      {
         var color = new Color();
         color.sRgbColor.a = (byte)((argb & 4278190080U) >> 24);
         color.sRgbColor.r = (byte)((argb & 16711680U) >> 16);
         color.sRgbColor.g = (byte)((argb & 65280U) >> 8);
         color.sRgbColor.b = (byte)(argb & byte.MaxValue);
         color.scRgbColor.a = color.sRgbColor.a / (float)byte.MaxValue;
         color.scRgbColor.r = SRgbToScRgb(color.sRgbColor.r);
         color.scRgbColor.g = SRgbToScRgb(color.sRgbColor.g);
         color.scRgbColor.b = SRgbToScRgb(color.sRgbColor.b);
         color.isFromScRgb = false;
         return color;
      }

      internal static char GetNumericListSeparator(IFormatProvider provider)
      {
         var ch = ',';
         var instance = NumberFormatInfo.GetInstance(provider);
         if (instance.NumberDecimalSeparator.Length > 0 && ch == instance.NumberDecimalSeparator[0])
            ch = ';';
         return ch;
      }

      internal string ConvertToString(string format, IFormatProvider provider)
      {
         var stringBuilder = new StringBuilder();
         if (format == null)
         {
            stringBuilder.AppendFormat(provider, "#{0:X2}", sRgbColor.a);
            stringBuilder.AppendFormat(provider, "{0:X2}", sRgbColor.r);
            stringBuilder.AppendFormat(provider, "{0:X2}", sRgbColor.g);
            stringBuilder.AppendFormat(provider, "{0:X2}", sRgbColor.b);
         }
         else
         {
            char numericListSeparator = GetNumericListSeparator(provider);
            stringBuilder.AppendFormat(provider, "sc#{1:" + format + "}{0} {2:" + format + "}{0} {3:" + format + "}{0} {4:" + format + "}", numericListSeparator, scRgbColor.a, scRgbColor.r, scRgbColor.g, scRgbColor.b);
         }
         return stringBuilder.ToString();
      }

      /// <summary>
      /// Gets or sets the sRGB alpha channel value of the color.
      /// </summary>
      /// <returns>
      /// The sRGB alpha channel value of the color, a value between 0 and 255.
      /// </returns>
      public byte A
      {
         get => sRgbColor.a;

         set
         {
            scRgbColor.a = value / (float)byte.MaxValue;
            sRgbColor.a = value;
         }
      }

      /// <summary>
      /// Gets or sets the sRGB blue channel value of the color.
      /// </summary>
      /// <returns>
      /// The sRGB blue channel value of the color structure, a value between 0 and 255.
      /// </returns>
      public byte B
      {
         get => sRgbColor.b;

         set
         {
            scRgbColor.b = SRgbToScRgb(value);
            sRgbColor.b = value;
         }
      }

      /// <summary>
      /// Gets or sets the sRGB green channel value of the color.
      /// </summary>
      /// <returns>
      /// The sRGB green channel value of the color structure, a value between 0 and 255.
      /// </returns>
      public byte G
      {
         get => sRgbColor.g;

         set
         {
            scRgbColor.g = SRgbToScRgb(value);
            sRgbColor.g = value;
         }
      }

      /// <summary>
      /// Gets or sets the sRGB red channel value of the color.
      /// </summary>
      /// <returns>
      /// The sRGB red channel value of the color structure, a value between 0 and 255.
      /// </returns>
      public byte R
      {
         get => sRgbColor.r;

         set
         {
            scRgbColor.r = SRgbToScRgb(value);
            sRgbColor.r = value;
         }
      }

      /// <summary>
      /// Gets or sets the ScRGB alpha channel value of the color.
      /// </summary>
      /// <returns>
      /// The ScRGB alpha channel value of the color structure, a value between 0 and 1.
      /// </returns>
      public float ScA
      {
         get => scRgbColor.a;

         set
         {
            scRgbColor.a = value;
            if (value < 0.0)
               sRgbColor.a = 0;
            else if (value > 1.0)
               sRgbColor.a = byte.MaxValue;
            else
               sRgbColor.a = (byte)(value * (double)byte.MaxValue);
         }
      }

      /// <summary>
      /// Gets or sets the ScRGB blue channel value of the color.
      /// </summary>
      /// <returns>
      /// The ScRGB red channel value of the color structure, a value between 0 and 1.
      /// </returns>
      public float ScB
      {
         get => scRgbColor.b;

         set
         {
            scRgbColor.b = value;
            sRgbColor.b = ScRgbTosRgb(value);
         }
      }

      /// <summary>
      /// Gets or sets the ScRGB green channel value of the color.
      /// </summary>
      /// <returns>
      /// The ScRGB green channel value of the color structure, a value between 0 and 1.
      /// </returns>
      public float ScG
      {
         get => scRgbColor.g;

         set
         {
            scRgbColor.g = value;
            sRgbColor.g = ScRgbTosRgb(value);
         }
      }

      /// <summary>
      /// Gets or sets the ScRGB red channel value of the color.
      /// </summary>
      /// <returns>
      /// The ScRGB red channel value of the color structure, a value between 0 and 1.
      /// </returns>
      public float ScR
      {
         get => scRgbColor.r;

         set
         {
            scRgbColor.r = value;
            sRgbColor.r = ScRgbTosRgb(value);
         }
      }

      /// <summary>
      /// Adds two color structures.
      /// </summary>
      /// <returns>
      /// A new color structure whose color values are the results of the addition operation.
      /// </returns>
      /// <param name="color1">
      /// The first color structure to add.
      /// </param>
      /// <param name="color2">
      /// The second color structure to add.
      /// </param>
      public static Color Add(Color color1, Color color2)
      {
         return color1 + color2;
      }

      /// <summary>
      /// Tests whether two color structures are identical.
      /// </summary>
      /// <returns>
      /// true if <paramref name="color1" /> and <paramref name="color2" /> are exactly identical;
      /// otherwise, false.
      /// </returns>
      /// <param name="color1">
      /// The first color structure to compare.
      /// </param>
      /// <param name="color2">
      /// The second color structure to compare.
      /// </param>
      public static bool Equals(Color color1, Color color2)
      {
         return color1 == color2;
      }

      /// <summary>
      /// Creates a new color structure by using the specified sRGB alpha channel and color channel values.
      /// </summary>
      /// <returns>
      /// A color structure with the specified values.
      /// </returns>
      /// <param name="a">
      /// The alpha channel, <see cref="P:System.Windows.Media.Color.A" />, of the new color.
      /// </param>
      /// <param name="r">
      /// The red channel, <see cref="P:System.Windows.Media.Color.R" />, of the new color.
      /// </param>
      /// <param name="g">
      /// The green channel, <see cref="P:System.Windows.Media.Color.G" />, of the new color.
      /// </param>
      /// <param name="b">
      /// The blue channel, <see cref="P:System.Windows.Media.Color.B" />, of the new color.
      /// </param>
      public static Color FromArgb(byte a, byte r, byte g, byte b)
      {
         var color = new Color();
         color.scRgbColor.a = a / (float)byte.MaxValue;
         color.scRgbColor.r = SRgbToScRgb(r);
         color.scRgbColor.g = SRgbToScRgb(g);
         color.scRgbColor.b = SRgbToScRgb(b);
         color.sRgbColor.a = a;
         color.sRgbColor.r = ScRgbTosRgb(color.scRgbColor.r);
         color.sRgbColor.g = ScRgbTosRgb(color.scRgbColor.g);
         color.sRgbColor.b = ScRgbTosRgb(color.scRgbColor.b);
         color.isFromScRgb = false;
         return color;
      }

      /// <summary>
      /// Creates a new color structure by using the specified sRGB color channel values.
      /// </summary>
      /// <returns>
      /// A color structure with the specified values and an alpha channel value of 255.
      /// </returns>
      /// <param name="r">
      /// The sRGB red channel, <see cref="P:System.Windows.Media.Color.R" />, of the new color.
      /// </param>
      /// <param name="g">
      /// The sRGB green channel, <see cref="P:System.Windows.Media.Color.G" />, of the new color.
      /// </param>
      /// <param name="b">
      /// The sRGB blue channel, <see cref="P:System.Windows.Media.Color.B" />, of the new color.
      /// </param>
      public static Color FromRgb(byte r, byte g, byte b)
      {
         return FromArgb(byte.MaxValue, r, g, b);
      }

      /// <summary>
      /// Creates a new color structure by using the specified ScRGB alpha channel and color channel values.
      /// </summary>
      /// <returns>
      /// A color structure with the specified values.
      /// </returns>
      /// <param name="a">
      /// The ScRGB alpha channel, <see cref="P:System.Windows.Media.Color.ScA" />, of the new color.
      /// </param>
      /// <param name="r">
      /// The ScRGB red channel, <see cref="P:System.Windows.Media.Color.ScR" />, of the new color.
      /// </param>
      /// <param name="g">
      /// The ScRGB green channel, <see cref="P:System.Windows.Media.Color.ScG" />, of the new color.
      /// </param>
      /// <param name="b">
      /// The ScRGB blue channel, <see cref="P:System.Windows.Media.Color.ScB" />, of the new color.
      /// </param>
      public static Color FromScRgb(float a, float r, float g, float b)
      {
         var color = new Color();
         color.scRgbColor.r = r;
         color.scRgbColor.g = g;
         color.scRgbColor.b = b;
         color.scRgbColor.a = a;
         if (a < 0.0)
            a = 0.0f;
         else if (a > 1.0)
            a = 1f;
         color.sRgbColor.a = (byte)(a * (double)byte.MaxValue + 0.5);
         color.sRgbColor.r = ScRgbTosRgb(color.scRgbColor.r);
         color.sRgbColor.g = ScRgbTosRgb(color.scRgbColor.g);
         color.sRgbColor.b = ScRgbTosRgb(color.scRgbColor.b);
         color.isFromScRgb = true;
         return color;
      }

      /// <summary>
      /// Multiplies the alpha, red, blue, and green channels of the specified color structure by the
      /// specified value.
      /// </summary>
      /// <returns>
      /// A new color structure whose color values are the results of the multiplication operation.
      /// </returns>
      /// <param name="color">
      /// The color to be multiplied.
      /// </param>
      /// <param name="coefficient">
      /// The value to multiply by.
      /// </param>
      public static Color Multiply(Color color, float coefficient)
      {
         return color * coefficient;
      }

      /// <summary>
      /// Subtracts a color structure from a color structure.
      /// </summary>
      /// <returns>
      /// A new color structure whose color values are the results of the subtraction operation.
      /// </returns>
      /// <param name="color1">
      /// The color structure to be subtracted from.
      /// </param>
      /// <param name="color2">
      /// The color structure to subtract from <paramref name="color1" />.
      /// </param>
      public static Color operator -(Color color1, Color color2)
      {
         return FromScRgb(color1.scRgbColor.a - color2.scRgbColor.a, color1.scRgbColor.r - color2.scRgbColor.r, color1.scRgbColor.g - color2.scRgbColor.g, color1.scRgbColor.b - color2.scRgbColor.b);
      }

      /// <summary>
      /// Tests whether two color structures are not identical.
      /// </summary>
      /// <returns>
      /// true if <paramref name="color1" /> and <paramref name="color2" /> are not equal; otherwise, false.
      /// </returns>
      /// <param name="color1">
      /// The first color structure to compare.
      /// </param>
      /// <param name="color2">
      /// The second color structure to compare.
      /// </param>
      public static bool operator !=(Color color1, Color color2)
      {
         return !(color1 == color2);
      }

      /// <summary>
      /// Multiplies the alpha, red, blue, and green channels of the specified color structure by the
      /// specified value.
      /// </summary>
      /// <returns>
      /// A new color structure whose color values are the results of the multiplication operation.
      /// </returns>
      /// <param name="color">
      /// The color to be multiplied.
      /// </param>
      /// <param name="coefficient">
      /// The value to multiply by.
      /// </param>
      public static Color operator *(Color color, float coefficient)
      {
         return FromScRgb(color.scRgbColor.a * coefficient, color.scRgbColor.r * coefficient, color.scRgbColor.g * coefficient, color.scRgbColor.b * coefficient);
      }

      /// <summary>
      /// Adds two color structures.
      /// </summary>
      /// <returns>
      /// A new color structure whose color values are the results of the addition operation.
      /// </returns>
      /// <param name="color1">
      /// The first color structure to add.
      /// </param>
      /// <param name="color2">
      /// The second color structure to add.
      /// </param>
      public static Color operator +(Color color1, Color color2)
      {
         return FromScRgb(color1.scRgbColor.a + color2.scRgbColor.a, color1.scRgbColor.r + color2.scRgbColor.r, color1.scRgbColor.g + color2.scRgbColor.g, color1.scRgbColor.b + color2.scRgbColor.b);
      }

      /// <summary>
      /// Tests whether two color structures are identical.
      /// </summary>
      /// <returns>
      /// true if <paramref name="color1" /> and <paramref name="color2" /> are exactly identical;
      /// otherwise, false.
      /// </returns>
      /// <param name="color1">
      /// The first color structure to compare.
      /// </param>
      /// <param name="color2">
      /// The second color structure to compare.
      /// </param>
      public static bool operator ==(Color color1, Color color2)
      {
         return Math.Abs(color1.scRgbColor.r - (double)color2.scRgbColor.r) < float.Epsilon
            && Math.Abs(color1.scRgbColor.g - (double)color2.scRgbColor.g) < float.Epsilon
            && (Math.Abs(color1.scRgbColor.b - (double)color2.scRgbColor.b) < float.Epsilon
            && Math.Abs(color1.scRgbColor.a - (double)color2.scRgbColor.a) < float.Epsilon);
      }

      /// <summary>
      /// Subtracts a color structure from a color structure.
      /// </summary>
      /// <returns>
      /// A new color structure whose color values are the results of the subtraction operation.
      /// </returns>
      /// <param name="color1">
      /// The color structure to be subtracted from.
      /// </param>
      /// <param name="color2">
      /// The color structure to subtract from <paramref name="color1" />.
      /// </param>
      public static Color Subtract(Color color1, Color color2)
      {
         return color1 - color2;
      }

      /// <summary>
      /// Sets the ScRGB channels of the color to within the gamut of 0 to 1, if they are outside
      /// that range.
      /// </summary>
      public void Clamp()
      {
         scRgbColor.r = scRgbColor.r < 0.0 ? 0.0f : scRgbColor.r > 1.0 ? 1f : scRgbColor.r;
         scRgbColor.g = scRgbColor.g < 0.0 ? 0.0f : scRgbColor.g > 1.0 ? 1f : scRgbColor.g;
         scRgbColor.b = scRgbColor.b < 0.0 ? 0.0f : scRgbColor.b > 1.0 ? 1f : scRgbColor.b;
         scRgbColor.a = scRgbColor.a < 0.0 ? 0.0f : scRgbColor.a > 1.0 ? 1f : scRgbColor.a;
         sRgbColor.a = (byte)(scRgbColor.a * (double)byte.MaxValue);
         sRgbColor.r = ScRgbTosRgb(scRgbColor.r);
         sRgbColor.g = ScRgbTosRgb(scRgbColor.g);
         sRgbColor.b = ScRgbTosRgb(scRgbColor.b);
      }

      /// <summary>
      /// Tests whether the specified color structure is identical to this color.
      /// </summary>
      /// <returns>
      /// true if the specified color structure is identical to the current color structure;
      /// otherwise, false.
      /// </returns>
      /// <param name="color">
      /// The color structure to compare to the current color structure.
      /// </param>
      public bool Equals(Color color)
      {
         return this == color;
      }

      /// <summary>
      /// Tests whether the specified object is a color structure and is equivalent to this color.
      /// </summary>
      /// <returns>
      /// true if the specified object is a color structure and is identical to the current color
      /// structure; otherwise, false.
      /// </returns>
      /// <param name="obj">
      /// The object to compare to this color structure.
      /// </param>
      public override bool Equals(object obj)
      {
         if (obj is Color)
            return this == (Color)obj;
         return false;
      }

      /// <summary>
      /// Gets a hash code for this color structure.
      /// </summary>
      /// <returns>
      /// A hash code for this color structure.
      /// </returns>
      public override int GetHashCode()
      {
         return scRgbColor.GetHashCode();
      }

      /// <summary>
      /// Creates a string representation of the color using the sRGB channels.
      /// </summary>
      /// <returns>
      /// The string representation of the color. The default implementation represents the
      /// <see cref="T:System.Byte" /> values in hex form, prefixes with the # character, and starts
      /// with the alpha channel. For example, the
      /// <see cref="M:System.Windows.Media.Color.ToString" /> value for
      /// <see cref="P:System.Windows.Media.Colors.AliceBlue" /> is #FFF0F8FF.
      /// </returns>
      public override string ToString()
      {
         return ConvertToString(isFromScRgb ? "R" : null, null);
      }

      /// <summary>
      /// Creates a string representation of the color by using the sRGB channels and the specified
      /// format provider.
      /// </summary>
      /// <returns>
      /// The string representation of the color.
      /// </returns>
      /// <param name="provider">
      /// Culture-specific formatting information.
      /// </param>
      public string ToString(IFormatProvider provider)
      {
         return ConvertToString(isFromScRgb ? "R" : null, provider);
      }

      string IFormattable.ToString(string format, IFormatProvider provider)
      {
         return ConvertToString(format, provider);
      }
   }
}
