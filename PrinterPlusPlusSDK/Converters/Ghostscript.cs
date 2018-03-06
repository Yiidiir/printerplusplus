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
namespace PrinterPlusPlusSDK.Converters
{
    public static class Ghostscript
    {
        public static string PSToTxt(string psFilename)
        {
            var retVal = string.Empty;
            var errorMessage = string.Empty;

            var command = "C:\\PrinterPlusPlus\\Converters\\gs\\gswin32c";
            var args = string.Format("-q -dNODISPLAY -P- -dSAFER -dDELAYBIND -dWRITESYSTEMDICT -dSIMPLE \"c:\\PrinterPlusPlus\\Converters\\gs\\ps2ascii.ps\" \"{0}\" -c quit", psFilename);
            retVal = Shell.ExecuteShellCommand(command, args, ref errorMessage);
            return retVal;
        }

        public static string PSToPdf(string psFilename, string pdfFilename)
        {
            var retVal = string.Empty;
            var errorMessage = string.Empty;

            var command = "C:\\PrinterPlusPlus\\Converters\\gs\\gswin32c";
            var args = string.Format("-q -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -sOutputFile=\"{1}\" -c save pop -f \"{0}\"", psFilename, pdfFilename);
            retVal = Shell.ExecuteShellCommand(command, args, ref errorMessage);
            return retVal;
        }

    }
}
