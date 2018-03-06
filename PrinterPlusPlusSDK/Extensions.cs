/*
Printer++ Virtual Printer Processor
Copyright (C) 2012 - Printer++

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

namespace PrinterPlusPlusSDK
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    [DebuggerStepThrough]
    public static partial class Extensions
    {
        /// <summary>
        /// Extension method for String DataType.
        /// Converts String to Int.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>Int datatype of the string after conversion.Zero(0) if conversion failed.</returns>
        public static int ToInt(this String value)
        {
            int retVal = 0;
            if (!string.IsNullOrEmpty(value))
                int.TryParse(value, out retVal);
            return retVal;
        }

        /// <summary>
        /// Extension method for Object DataType.
        /// Converts Object to Double.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>Double datatype of the string after conversion.Zero(0) if conversion failed.</returns>
        public static double ToDouble(this Object value)
        {
            Double retVal = 0;
            if (value != null && value.ToString() != string.Empty)
                double.TryParse(value.ToString(), out retVal);
            return retVal;
        }

        /// <summary>
        /// Extension method for String DataType.
        /// Converts String to Bool.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>Bool datatype of the string after conversion.False if conversion failed.</returns>
        public static bool ToBool(this String value)
        {
            //bool retVal = false;
            //if (!string.IsNullOrEmpty(value))
            //{
            //    bool.TryParse(value, out retVal);
            //    if (retVal == false && value.ToInt() == 1)
            //        retVal = true;
            //}
            //return retVal;
            return ToBool(value, false);
        }
        /// <summary>
        /// Extension method for String DataType.
        /// Converts String to Bool.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <param name="trueIfHasValue">Boolean flag to set the result to true if the input object has value regardless of the content.</param>
        /// <returns>Bool datatype of the string after conversion.False if conversion failed.</returns>
        public static bool ToBool(this String value, bool trueIfHasValue)
        {
            bool retVal = false;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (trueIfHasValue == false)
                {
                    bool.TryParse(value, out retVal);
                    if (retVal == false && value.ToInt() == 1)
                        retVal = true;
                }
                else
                {
                    if (value.Trim() != string.Empty)
                        retVal = true;
                }
            }
            return retVal;
        }

        public static bool IsNumeric(this String value)
        {
            int retVal;
            return int.TryParse(value, out retVal);
        }

        public static  string CleanSpaces(this String value, string replaceWith)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            return regex.Replace(value.Trim(), replaceWith);
        }

        public static  string CleanGSComments(this String value)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"%%\[.*?\]%%", options);
            return regex.Replace(value.Trim(), string.Empty);
        }
        
    }
}
