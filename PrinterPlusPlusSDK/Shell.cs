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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
namespace PrinterPlusPlusSDK
{
    public class Shell
    {
        private const int SLEEP_AMOUNT = 100;
        private Process p;
        private int elapsedTime;
        private bool eventHandled;
        public void StartProcess(string filename, string startInfoArgs, string startInfoVerb)
        {
            p = new Process();
            p.StartInfo.FileName = filename;
            if (!string.IsNullOrWhiteSpace(startInfoArgs))
                p.StartInfo.Arguments = startInfoArgs;

            if (!string.IsNullOrWhiteSpace(startInfoVerb))
                p.StartInfo.Verb = startInfoVerb;

            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(p_Exited);
            p.Start();

            // Wait for Exited event, but not more than 30 seconds.
            while (!eventHandled)
            {
                elapsedTime += SLEEP_AMOUNT;
                if (elapsedTime > 30000)
                {
                    break;
                }
                Thread.Sleep(SLEEP_AMOUNT);
            }
        }
        // Handle Exited event and display process information.
        private void p_Exited(object sender, System.EventArgs e)
        {
            eventHandled = true;
        }


        public static string ExecuteShellCommand(string fileToExecute, string args, ref string errorMessage)
        {
            //http://www.digitalcoding.com/Code-Snippets/C-Sharp/C-Code-Snippet-Execute-Shell-Commands-from-Net.html
            var retVal = string.Empty;
            try
            {
                using (var process = new Process())
                {
                    string cmdProcess = string.Format(System.Globalization.CultureInfo.InvariantCulture, @"{0}\cmd.exe", new object[] { Environment.SystemDirectory });
                    string arguments = string.Format(System.Globalization.CultureInfo.InvariantCulture, "/C {0}", new object[] { fileToExecute });
                    if (args != null && args.Length > 0)
                        arguments += string.Format(System.Globalization.CultureInfo.InvariantCulture, " {0}", new object[] { args, System.Globalization.CultureInfo.InvariantCulture });

                    ProcessStartInfo _ProcessStartInfo = new ProcessStartInfo(cmdProcess, arguments);
                    _ProcessStartInfo.CreateNoWindow = true;
                    _ProcessStartInfo.UseShellExecute = false;
                    _ProcessStartInfo.RedirectStandardOutput = true;
                    _ProcessStartInfo.RedirectStandardInput = true;
                    _ProcessStartInfo.RedirectStandardError = true;
                    process.StartInfo = _ProcessStartInfo;
                    process.Start();
                    errorMessage = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    retVal = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        IO.Log(string.Format("ExecuteShellCommand Error: {0}", errorMessage));
                }
            }
            catch (Win32Exception _Win32Exception)
            {
                IO.Log(string.Format("Win32 Exception caught in process: {0}", _Win32Exception.ToString()));
            }
            catch (Exception _Exception)
            {
                IO.Log(string.Format("Exception caught in process: {0}", _Exception.ToString()));
            }
            return retVal;
        }
    }
}
