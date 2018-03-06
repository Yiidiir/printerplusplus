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
using System.IO;

namespace PrinterPlusPlusSDK
{
    public static class IO
    {
        public static string ReadFile(string filename)
        {
            var retVal = string.Empty;
            using (var sr = new StreamReader(filename))
            {
                retVal = sr.ReadToEnd();
            }
            return retVal;
        }

        public static void WriteFile(string filename, string value)
        {
            using (var sw = new StreamWriter(filename, false))
            {
                sw.Write(value);
                sw.Flush();
            }
        }

        public static void Delete(string filename)
        {
            File.Delete(filename);
        }

        //public static void Print(string filename)
        //{
        //    StartProcess(filename, "print");
        //}

        //public static void StartProcess(string filename)
        //{
        //    StartProcess(filename, string.Empty);
        //}

        //public static void StartProcess(string filename, string startInfoVerb)
        //{
        //    Process p = new Process();
        //    p.StartInfo.FileName = filename;
        //    p.StartInfo.UseShellExecute = true;
        //    if (!string.IsNullOrWhiteSpace(startInfoVerb))
        //        p.StartInfo.Verb = startInfoVerb;
        //    p.EnableRaisingEvents = true;
        //    p.Exited += new System.EventHandler(printJob_Exited);
        //    p.Start();
            
        //}

        //static void printJob_Exited(object sender, System.EventArgs e)
        //{
        //    eventHandled = true;
        //}

        public static void Log(string msg)
        {
            var debugMode = ConfigurationManager.AppSettings["DebugMode"].ToBool();
            if (debugMode == true)
            {
                var appDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var fileName = string.Format("{0}\\PrinterPlusPlus.log", appDir);
                using (var sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(string.Format("{0} - {1}\n", DateTime.Now, msg));
                    sw.Flush();
                }
            }
        }

        public static bool IsFileLocked(string filename)
        {
            FileStream stream = null;
            FileInfo file = new FileInfo(filename);
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
