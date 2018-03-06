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
using Microsoft.Win32;
namespace PrinterPlusPlus
{
    public static class RegistryHelper
    {
        public static string ReadRegistry32(string path, string key)
        {
            return ReadRegistry(path, key, RegistryView.Registry32);
        }

        public static string ReadRegistry64(string path, string key)
        {
            return ReadRegistry(path, key, RegistryView.Registry64);
        }

        public static string ReadRegistry(string path, string key, RegistryView view)
        {
            var retVal = string.Empty;
            RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, view);
            localKey = localKey.OpenSubKey(path);
            if (localKey != null)
            {
                retVal = (string)localKey.GetValue(key);
            }
            return retVal;
        }
    }
}
