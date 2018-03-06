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
using System.Configuration;
using System.Windows.Forms;
using PrinterPlusPlusSDK;

namespace PrinterPlusPlus
{
    public class ProcessorHelper
    {
        public static void Process(string psFilename)
        {
            //%r_%c_%u_%Y%m%d_%H%n%s_%j.ps  (Elfa_PH-D-W7-1_rsaturnino_20120622_131716_20.ps)
            //%r = Printer name
            //%c = ComputerName
            //%u = UserName
            //%Y%m%d = Date(YYYYMMDD)
            //%H%n%s = Time (HHMMSS) (24hours format)
            //%j = Print job id
            var filename = System.IO.Path.GetFileNameWithoutExtension(psFilename);
            var arrFileName = filename.Split('_');
            if (arrFileName.Length > 0)
            {
                var key = arrFileName[0];
                Process(key, psFilename);
            }
        }

        public static void Process(string key, string psFilename)
        {
            try
            {
                //Licensing.Register("639f793535871a3ebe494522bd1254af");
                var isRegistered = LicenseHelper.Validate();
                if (isRegistered == true)
                {
                    IO.Log(string.Format("Process: {0}, {1}", key, psFilename));
                    ProcessResult retVal = null;
                    IProcessor dependencyProcessor = GetDependencyProcessor(key);
                    if (dependencyProcessor != null)
                    {
                        IO.Log("Initializing Processor");
                        Processor processor = new Processor(dependencyProcessor);
                        IO.Log("Executing Process");
                        retVal = processor.Process(key, psFilename);
                        IO.Log("Process Complete");
                        if (retVal.HasException == true)
                        {
                            MessageBox.Show(retVal.Exception.Message, "Printer++");
                            IO.Log(string.Format("Exception: {0}", retVal.Exception.Message));
                        }
                    }
                    else
                    {
                        var msg = string.Format("Processor not found for: {0}", key);
                        MessageBox.Show(msg, "Printer++");
                        IO.Log(msg);
                    }
                }
                else
                {
                    var frm = new frmRegistration();
                    frm.ShowDialog();
                    //MessageBox.Show("Printer++ is not registered.", "Printer++ Registration");
                }
            }
            catch (Exception ex)
            {
                IO.Log(ex.Message);
            }
        }

        private static IProcessor GetDependencyProcessor(string key)
        {
            //Use typeof(Processor).AssemblyQualifiedName to get the qualified name of the object and add to web.config
            IProcessor retVal = null;
            if (!string.IsNullOrWhiteSpace(key))
            {
                string classToCreate = GetProcessorQualifiedName(key);
                if (!string.IsNullOrWhiteSpace(classToCreate))
                {
                    Type type = System.Type.GetType(classToCreate);
                    if (type != null)
                    {
                        retVal = (IProcessor)Activator.CreateInstance(type);
                    }
                }
            }
            return retVal;
        }

        private static string GetProcessorQualifiedName(string processor)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrWhiteSpace(processor))
            {
                var section = (System.Collections.Specialized.NameValueCollection)ConfigurationManager.GetSection("processors");
                if (section != null)
                    retVal = section[processor];
            }
            return retVal;
        }
    }
}
