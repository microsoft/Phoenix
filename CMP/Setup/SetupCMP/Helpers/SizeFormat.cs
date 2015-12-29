//+------------------------------------------------------------------
//
//  Description:  Input and localized output formats for various sizes used by UI.
//
//-------------------------------------------------------------------

using System;
using System.Threading;
using System.Globalization;

namespace CMP.Setup.Helpers
{
    public enum SizeFormatType
    {
        Size, // uses labels like KB, MB & GB
        DataRate // uses labels like KBps, MBps and GBps
    }

    /// <summary>
    /// Base class for handling conversions for user input and display of bytes.
    /// </summary>
    public class SizeFormat
    {
        public const Int64 KILOBYTE_FACTOR = 1024;
        public const Int64 MEGABYTE_FACTOR = 1024 * KILOBYTE_FACTOR;
        public const Int64 GIGABYTE_FACTOR = 1024 * MEGABYTE_FACTOR;
        public const string FORMAT_CHAR = "n";
        public const NumberStyles NumberStyle = NumberStyles.AllowThousands;
        public const NumberStyles DecimalStyle = NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint;

        /// <summary>
        /// When rounding 1.005 the following occurs:
        /// 1.005 * 100 -> 1.0049999 . 5 -> 1.0099999.  We need this fudge factor to address this situation
        /// </summary>
        internal const double DoubleRoundingFudgeFactor = 0.000001F;

        protected Int64 byteFactor;
        protected NumberStyles style;
        protected string label;

        protected readonly string KiloSizeLabel;
        protected readonly string MegaSizeLabel;
        protected readonly string GigaSizeLabel;

        /// <summary>
        /// Create a size format based on the size passed in
        /// <list type="bullet">
        /// <item>Size >=     1024 * 1024 * 1024 (1 GB) -> GigaByte Format</item>
        /// <item>else                             -> MegaByte Format</item>
        /// </list>
        /// </summary>
        /// <param name="sizeInBytes">size to base format on</param>
        /// <param name="type">Type to be used for labels, e.g. KB for size, KBps for data rate</param>
        public SizeFormat(Int64 sizeInBytes, SizeFormatType type)
        {
            switch (type)
            {
                case SizeFormatType.DataRate:
                    this.KiloSizeLabel = "KBps";
                    this.MegaSizeLabel = "MBps";
                    this.GigaSizeLabel = "GBps";
                    break;
                case SizeFormatType.Size:
                default:
                    this.KiloSizeLabel = "KB";
                    this.MegaSizeLabel = "MB";
                    this.GigaSizeLabel = "GB";
                    break;
            }

            if (sizeInBytes >= GIGABYTE_FACTOR)
            {
                this.byteFactor = GIGABYTE_FACTOR;
                this.style = DecimalStyle;
                this.label = this.GigaSizeLabel;
            }
            else if (sizeInBytes >= MEGABYTE_FACTOR)
            {
                this.byteFactor = MEGABYTE_FACTOR;
                this.style = DecimalStyle;
                this.label = this.MegaSizeLabel;
            }
            else
            {
                this.byteFactor = KILOBYTE_FACTOR;
                this.style = NumberStyle;
                this.label = this.KiloSizeLabel;
            }
        }

        public SizeFormat(Int64 sizeInBytes)
            : this(sizeInBytes, SizeFormatType.Size)
        {

        }

        /// <summary>
        /// Round the size in bytes according to the derived format
        /// </summary>
        /// <param name="size">data size in bytes</param>
        /// <returns>rounded size in bytes</returns>
        virtual protected Double Round(double size)
        {
            if (this.style == NumberStyle)
            {
                // Round to nearest unit
                return Math.Floor(size + .5 + DoubleRoundingFudgeFactor);
            }
            else
            {
                // Round to nearest hundreth of unit
                return Math.Floor((size * 100) + .5 + DoubleRoundingFudgeFactor) / 100;
            }
        }

        /// <summary>
        /// Round the size in bytes according to the derived format
        /// </summary>
        /// <param name="size">data size in bytes</param>
        /// <returns>rounded size in bytes</returns>
        public Int64 Round(Int64 sizeInBytes)
        {
            return (Int64)(this.Round((double)sizeInBytes / this.byteFactor) * this.byteFactor);
        }

        private delegate string SizeFormatter(Int64 sizeInBytes);

        /// <summary>
        /// Format the size based upon the derived types format and factor
        /// </summary>
        /// <param name="size">size to format</param>
        /// <returns>string representation based on unit type</returns>
        public string FormatSize(Int64 sizeInBytes)
        {
            if (sizeInBytes <= 0)
            {
                return "0";
            }
            else
            {
                return this.FormatSignedSize(sizeInBytes);
            }
        }

        /// <summary>
        /// Format the size, but allow negatives
        /// Separate this out from FormatSize as most sizes should not be negative
        /// </summary>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        public string FormatSignedSize(Int64 sizeInBytes)
        {
            double size = this.Round((double)sizeInBytes / this.byteFactor);
            bool roundZeroUp = (size == 0) && (sizeInBytes > 0);
            if (this.style == NumberStyle)
            {
                System.Globalization.NumberFormatInfo numberFormatInfo = (System.Globalization.NumberFormatInfo)
                    Thread.CurrentThread.CurrentCulture.NumberFormat.Clone();
                numberFormatInfo.NumberDecimalDigits = 0;
                return (roundZeroUp ? 1 : size).ToString(FORMAT_CHAR, numberFormatInfo);
            }
            else
            {
                return (roundZeroUp ? (.01) : size).ToString(FORMAT_CHAR);
            }
        }

        /// <summary>
        /// Tries to parse the string into an Int64 based
        /// upon the rules of the unit.
        /// </summary>
        /// <param name="size">user input string to parse number from</param>
        /// <returns>integer size from input string</returns>
        /// <exception cref="FormatException">Invalid text</exception>
        /// <exception cref="OverflowException">Input was too large for Int64</exception>
        public Int64 Parse(string sizeString)
        {
            if (sizeString.Length == 0)
            {
                return 0;
            }

            double initialSize = Double.Parse(sizeString, this.style);

            // Round the input size as a double to minimize the value loss (underflow) in casting
            Int64 size = checked((Int64)(this.Round(initialSize) * this.byteFactor));

            if (size < 0)
            {
                throw new OverflowException();
            }

            return size;
        }

        /// <summary>
        /// Display the size with the label next to it, like "30 GB"
        /// </summary>
        /// <param name="sizeInBytes">Size to display, in bytes</param>
        /// <returns></returns>
        public string FormatSizeWithLabel(Int64 sizeInBytes)
        {
            return this.FormatSizeWithLabel(sizeInBytes, new SizeFormatter(this.FormatSize));
        }

        /// <summary>
        /// Display the signed size with the label next to it, "30 GB" or "-30GB".
        /// </summary>
        /// <param name="sizeInBytes">Size to display, in bytes</param>
        /// <returns></returns>
        public string FormatSignedSizeWithLabel(Int64 sizeInBytes)
        {
            return this.FormatSizeWithLabel(sizeInBytes, new SizeFormatter(this.FormatSignedSize));
        }

        private string FormatSizeWithLabel(Int64 sizeInBytes, SizeFormatter sizeFormatter)
        {
            if (this.byteFactor == GIGABYTE_FACTOR)
            {
                return string.Format("{0} {1}", sizeFormatter(sizeInBytes), this.GigaSizeLabel);
            }
            else if (this.byteFactor == MEGABYTE_FACTOR)
            {
                return string.Format("{0} {1}", sizeFormatter(sizeInBytes), this.MegaSizeLabel);
            }
            else
            {
                return string.Format("{0} {1}", sizeFormatter(sizeInBytes), this.KiloSizeLabel);
            }
        }

        /// <summary>
        /// Display the size with the label next to it, like "30 GB"
        /// </summary>
        /// <param name="sizeInBytes">Size to display, in bytes</param>
        /// <param name="stringForInvalidSize">String to use if the size is less than or equal to 0 </param>
        /// <returns>string with the size and the label for the format, like "30 GB"</returns>
        public string FormatSizeWithLabel(Int64 sizeInBytes, string stringForInvalidSize)
        {
            if (sizeInBytes <= 0)
            {
                return stringForInvalidSize;
            }

            return this.FormatSizeWithLabel(sizeInBytes);
        }

        /// <summary>
        /// Override ToString to return the derived label of this format
        /// </summary>
        /// <returns>string label {TB,GB, or MB}</returns>
        public override string ToString()
        {
            return this.label;
        }

        /// <summary>
        /// Static version of format size with label, if the sizeformat doesn't need to be reused
        /// </summary>
        /// <param name="sizeInBytes">size to format</param>
        /// <returns></returns>
        public static string Format(Int64 sizeInBytes)
        {
            return new SizeFormat(sizeInBytes).FormatSizeWithLabel(sizeInBytes);
        }

        /// <summary>
        /// Static version of format size with label, if the sizeformat doesn't need to be reused
        /// </summary>
        /// <param name="sizeInBytes">size to format</param>
        /// <returns></returns>
        public static string Format(Int64 sizeInBytes, SizeFormatType type)
        {
            return new SizeFormat(sizeInBytes, type).FormatSizeWithLabel(sizeInBytes);
        }

        /// <summary>
        /// Format the size with label, using the units provided
        /// </summary>
        /// <param name="sizeInBytes"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string FormatByUnit(Int64 sizeInBytes, Int64 unit)
        {
            return new SizeFormat(unit).FormatSizeWithLabel(sizeInBytes);
        }

        /// <summary>
        /// This common helper function rounds up the result of a division.
        /// Its most common use in this class is to round up VM capacity needs such
        /// as disk space when KB needs to be converted to MB
        /// </summary>
        /// <param name="dividend">The number that will be divided</param>
        /// <param name="divisor">The number to divide by</param>
        /// <returns></returns>
        public static ulong ScaleDownAndRoundUp(ulong dividend, ulong divisor)
        {
            ulong roundedResult = ulong.MaxValue;

            if (divisor != 0)
            {
                roundedResult = dividend / divisor;

                if (((dividend % divisor) != 0) && roundedResult != ulong.MaxValue)
                {
                    // If the division has remainders, round up by one
                    roundedResult++;
                }
            }

            return roundedResult;
        }
    }
}
