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
using System;
namespace PrinterPlusPlus
{
    public static class LicenseHelper
    {
        public static string GetKey()
        {
            //return CryptographyHelper.Encrypt(GetMachineGuid());
            return GetKey(GetMachineGuid());
        }

        public static string GetKey(string machineId)
        {
            return CryptographyHelper.Encrypt(machineId);
        }

        public static string CleanRegKey(string value)
        {
            return value;
        }

        public static bool Register(string regKeyInput)
        {
            var retVal = false;
            var regKey = GetKey();
            if (regKey.Equals(regKeyInput, StringComparison.OrdinalIgnoreCase))
            {
                ConfigHelper.AddSetting("RegKey", regKey);
                retVal = true;
            }
            return retVal;
        }

        public static bool Validate()
        {
            //var regKey = ConfigHelper.ReadSetting("RegKey");
            //var regKeyCalculated = CryptographyHelper.Encrypt(GetMachineGuid());
            //var retVal = false;
            //if (!string.IsNullOrWhiteSpace(regKey))
            //    retVal = (regKey.Equals(regKeyCalculated, StringComparison.OrdinalIgnoreCase));
            //return retVal;
            return true;
        }

        public static string GetMachineGuid()
        {
            var retVal = string.Empty;
            string regPath = @"SOFTWARE\Microsoft\Cryptography";
            string regKey = "MachineGuid";
            string value32 = RegistryHelper.ReadRegistry32(regPath, regKey);
            string value64 = RegistryHelper.ReadRegistry64(regPath, regKey);

            if (!string.IsNullOrWhiteSpace(value32))
                retVal = value32;
            else if (!string.IsNullOrWhiteSpace(value64))
                retVal = value64;

            return retVal;
        }
    }
}
