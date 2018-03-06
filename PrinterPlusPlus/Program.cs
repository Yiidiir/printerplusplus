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
using System.Windows.Forms;

namespace PrinterPlusPlus
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var silentMode = false;
            var noMonitor = false;
            var key = string.Empty;
            var fileName = string.Empty;

            //-silent -nomonitor -key Elfa -file C:\PrinterPlusPlus\Temp\Elfa_PH-D-W7-1_rsaturnino_20120622_133136_22.ps
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-silent":
                        silentMode = true;
                        break;
                    case "-nomonitor":
                        noMonitor = true;
                        break;
                    case "-key":
                        i += 1;
                        key = args[i];
                        break;
                    case "-file":
                        i += 1;
                        fileName = args[i];
                        break;

                    default:
                        break;
                }
            }

            if (LicenseHelper.Validate() == true)
                Application.Run(new frmMain(silentMode, noMonitor, key, fileName));
            else
                Application.Run(new frmRegistration());
        }
    }
}
