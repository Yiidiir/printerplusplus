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
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace PrinterPlusPlusSetupCustomAction
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            LogHelper.Log("Install Started.");
            LogHelper.Log("Keys:");
            foreach (var key in Context.Parameters.Keys)
            {
                LogHelper.Log(string.Format("{0}: {1}", key.ToString(), Context.Parameters[key.ToString()].ToString()));
            }
            //if (Context.Parameters.ContainsKey("PRINTERNAME") && !string.IsNullOrWhiteSpace(Context.Parameters["PRINTERNAME"]) && Context.Parameters.ContainsKey("PROCESSOR") && !string.IsNullOrWhiteSpace(Context.Parameters["PROCESSOR"]))
            string printerName = string.Empty;
            if (Context.Parameters.ContainsKey("PRINTERNAME") && !string.IsNullOrWhiteSpace(Context.Parameters["PRINTERNAME"]))
                printerName = Context.Parameters["PRINTERNAME"].ToString();
            else if (Context.Parameters.ContainsKey("NAME") && !string.IsNullOrWhiteSpace(Context.Parameters["NAME"]))
                printerName = Context.Parameters["NAME"].ToString();

            if (printerName != string.Empty)
            //if (Context.Parameters.ContainsKey("PRINTERNAME") && !string.IsNullOrWhiteSpace(Context.Parameters["PRINTERNAME"]))
            {
                //string printerName = Context.Parameters["PRINTERNAME"].ToString();
                //string key = Context.Parameters["PROCESSOR"].ToString();
                //string serial = Context.Parameters["SERIALNUMBER"].ToString();

                LogHelper.Log(string.Format("PrinterName: {0}", printerName));
                //LogHelper.Log(string.Format("Processor: {0}", key));
                //LogHelper.Log(string.Format("Serial: {0}", serial));
                try
                {
                    SpoolerHelper sh = new SpoolerHelper();
                    SpoolerHelper.GenericResult result = sh.AddVPrinter(printerName, printerName);
                    if (result.Success == false)
                    {
                        LogError(result.Method, result.Message, result.Exception);
                        throw new InstallException(string.Format("Source: {0}\nMessage: {1}", result.Method, result.Message), result.Exception);
                    }
                    AutorunHelper.AddToStartup();
                }
                catch (Exception ex)
                {
                    LogError("AddVPrinter", ex.Message, ex);
                }
            }
            else
            {
                LogHelper.Log("Incomplete Parameters.");
                throw new InstallException("Incomplete Parameters.");
            }

            LogHelper.Log("Install Finished.");
        }

        private static void LogError(string exceptionSource, string message, Exception innerException)
        {
            string eventMessage = string.Format("Source: {0}\nMessage: {1}\nInnerException: {2}", exceptionSource, message, innerException);
            LogHelper.Log(eventMessage);
        }
    }
}
