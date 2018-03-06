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
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;

namespace PrinterPlusPlusSetupCustomAction
{
    public class SpoolerHelper
    {

        #region PInvoke Codes
        #region Printer Monitor
        //API for Adding Print Monitors
        //http://msdn.microsoft.com/en-us/library/windows/desktop/dd183341(v=vs.85).aspx
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 AddMonitor(String pName, UInt32 Level, ref MONITOR_INFO_2 pMonitors);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITOR_INFO_2
        {
            public string pName;
            public string pEnvironment;
            public string pDLLName;
        }
        #endregion
        #region Printer Port
        private const int MAX_PORTNAME_LEN = 64;
        private const int MAX_NETWORKNAME_LEN = 49;
        private const int MAX_SNMP_COMMUNITY_STR_LEN = 33;
        private const int MAX_QUEUENAME_LEN = 33;
        private const int MAX_IPADDR_STR_LEN = 16;
        private const int RESERVED_BYTE_ARRAY_SIZE = 540;

        private enum PrinterAccess
        {
            ServerAdmin = 0x01,
            ServerEnum = 0x02,
            PrinterAdmin = 0x04,
            PrinterUse = 0x08,
            JobAdmin = 0x10,
            JobRead = 0x20,
            StandardRightsRequired = 0x000f0000,
            PrinterAllAccess = (StandardRightsRequired | PrinterAdmin | PrinterUse)
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PrinterDefaults
        {
            public IntPtr pDataType;
            public IntPtr pDevMode;
            public PrinterAccess DesiredAccess;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct PortData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PORTNAME_LEN)]
            public string sztPortName;
        }

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool OpenPrinter(string printerName, out IntPtr phPrinter, ref PrinterDefaults printerDefaults);
        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr phPrinter);
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool XcvDataW(IntPtr hXcv, string pszDataName, IntPtr pInputData, UInt32 cbInputData, out IntPtr pOutputData, UInt32 cbOutputData, out UInt32 pcbOutputNeeded, out UInt32 pdwStatus);

        #endregion
        #region Printer Driver
        //API for Adding Printer Driver
        //http://msdn.microsoft.com/en-us/library/windows/desktop/dd183346(v=vs.85).aspx
        //http://pinvoke.net/default.aspx/winspool.DRIVER_INFO_2
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 AddPrinterDriver(String pName, UInt32 Level, ref DRIVER_INFO_3 pDriverInfo);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DRIVER_INFO_3
        {
            public uint cVersion;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pEnvironment;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverPath;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDataFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pConfigFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pHelpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDependentFiles;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pMonitorName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDefaultDataType;
        }
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool GetPrinterDriverDirectory(StringBuilder pName, StringBuilder pEnv, int Level, [Out] StringBuilder outPath, int bufferSize, ref int Bytes);
        #endregion
        #region Printer
        //API for Adding Printer
        //http://msdn.microsoft.com/en-us/library/windows/desktop/dd183343(v=vs.85).aspx
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 AddPrinter(string pName, uint Level, [In] ref PRINTER_INFO_2 pPrinter);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PRINTER_INFO_2
        {
            public string pServerName;
            public string pPrinterName;
            public string pShareName;
            public string pPortName;
            public string pDriverName;
            public string pComment;
            public string pLocation;
            public IntPtr pDevMode;
            public string pSepFile;
            public string pPrintProcessor;
            public string pDatatype;
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }
        #endregion
        #endregion

        public GenericResult AddPrinterMonitor(string monitorName)
        {
            GenericResult retVal = new GenericResult("AddPrinterMonitor");
            MONITOR_INFO_2 mi2 = new MONITOR_INFO_2();

            mi2.pName = monitorName;
            mi2.pEnvironment = null;
            mi2.pDLLName = "mfilemon.dll";

            try
            {
                if (AddMonitor(null, 2, ref mi2) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }

        public GenericResult AddPrinterPort(string portName, string portType)
        {
            GenericResult retVal = new GenericResult("AddPrinterPort");
            IntPtr printerHandle;
            PrinterDefaults defaults = new PrinterDefaults { DesiredAccess = PrinterAccess.ServerAdmin };
            try
            {
                if (!OpenPrinter(",XcvMonitor " + portType, out printerHandle, ref defaults))
                    throw new Exception("Could not open printer for the monitor port " + portType + "!");
                try
                {
                    PortData portData = new PortData { sztPortName = portName };
                    uint size = (uint)Marshal.SizeOf(portData);
                    IntPtr pointer = Marshal.AllocHGlobal((int)size);
                    Marshal.StructureToPtr(portData, pointer, true);
                    IntPtr outputData;
                    UInt32 outputNeeded, status;
                    try
                    {
                        if (!XcvDataW(printerHandle, "AddPort", pointer, size, out outputData, 0, out outputNeeded, out status))
                            retVal.Message = status.ToString();
                    }
                    catch (Exception ex)
                    {
                        retVal.Exception = ex;
                        retVal.Message = retVal.Exception.Message;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pointer);
                    }
                }
                catch (Exception ex)
                {
                    retVal.Exception = ex;
                    retVal.Message = retVal.Exception.Message;
                }

                finally
                {
                    ClosePrinter(printerHandle);
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }

        public GenericResult GetPrinterDirectory()
        {
            GenericResult retVal = new GenericResult("GetPrinterDirectory");
            StringBuilder str = new StringBuilder(1024);
            int i = 0;
            GetPrinterDriverDirectory(null, null, 1, str, 1024, ref i);
            try
            {
                GetPrinterDriverDirectory(null, null, 1, str, 1024, ref i);
                retVal.Success = true;
                retVal.Message = str.ToString();
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            return retVal;
        }

        public GenericResult AddPrinterDriver(string driverName, string driverPath, string dataPath, string configPath, string helpPath)
        {
            GenericResult retVal = new GenericResult("AddPrinterDriver");
            DRIVER_INFO_3 di = new DRIVER_INFO_3();
            di.cVersion = 3;
            di.pName = driverName;
            di.pEnvironment = null;
            di.pDriverPath = driverPath;
            di.pDataFile = dataPath;
            di.pConfigFile = configPath;
            di.pHelpFile = helpPath;
            di.pDependentFiles = "";
            di.pMonitorName = null;
            di.pDefaultDataType = "RAW";

            try
            {
                if (AddPrinterDriver(null, 3, ref di) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }

        public GenericResult AddPrinter(string printerName, string portName, string driverName)
        {
            GenericResult retVal = new GenericResult("AddPrinter");
            PRINTER_INFO_2 pi = new PRINTER_INFO_2();

            pi.pServerName = null;
            pi.pPrinterName = printerName;
            pi.pShareName = "";
            pi.pPortName = portName;
            pi.pDriverName = driverName;    // "Apple Color LW 12/660 PS";
            pi.pComment = "PrinterPlusPlus";
            pi.pLocation = "";
            pi.pDevMode = new IntPtr(0);
            pi.pSepFile = "";
            pi.pPrintProcessor = "WinPrint";
            pi.pDatatype = "RAW";
            pi.pParameters = "";
            pi.pSecurityDescriptor = new IntPtr(0);

            try
            {
                if (AddPrinter(null, 2, ref pi) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }

        public GenericResult ConfigureVirtualPort(string monitorName, string portName, string key)
        {
            GenericResult retVal = new GenericResult("ConfigureVirtualPort");
            try
            {
                string appPath = @"C:\PrinterPlusPlus";
                string outputPath = string.Format(@"{0}\Temp", appPath);
                string filePattern = "%r_%c_%u_%Y%m%d_%H%n%s_%j.ps";
                string userCommand = string.Empty;
                var execPath = string.Empty;

                string keyName = string.Format(@"SYSTEM\CurrentControlSet\Control\Print\Monitors\{0}\{1}", monitorName, portName);
                Registry.LocalMachine.CreateSubKey(keyName);
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(keyName, true);
                regKey.SetValue("OutputPath", outputPath, RegistryValueKind.String);
                regKey.SetValue("FilePattern", filePattern, RegistryValueKind.String);
                regKey.SetValue("Overwrite", 0, RegistryValueKind.DWord);
                regKey.SetValue("UserCommand", userCommand, RegistryValueKind.String);
                regKey.SetValue("ExecPath", execPath, RegistryValueKind.String);
                regKey.SetValue("WaitTermination", 0, RegistryValueKind.DWord);
                regKey.SetValue("PipeData", 0, RegistryValueKind.DWord);
                regKey.Close();
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }

            return retVal;
        }

        public GenericResult RestartSpoolService()
        {
            GenericResult retVal = new GenericResult("RestartSpoolService");
            try
            {
                ServiceController sc = new ServiceController("Spooler");
                if (sc.Status != ServiceControllerStatus.Stopped || sc.Status != ServiceControllerStatus.StopPending)
                    sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                sc.Start();
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            return retVal;
        }

        public GenericResult AddVPrinter(string printerName, string key)
        {
            GenericResult retVal = new GenericResult("AddVPrinter");
            try
            {
                string monitorName = "PrinterPlusPlus";
                string portName = string.Format("{0}:", printerName);
                string driverName = "PrinterPlusPlus";

                string driverFileName = "PSCRIPT5.DLL";
                string dataFileName = "PRINTERPLUSPLUS.PPD";
                string configFileName = "PS5UI.DLL";
                string helpFileName = "PSCRIPT.HLP";

                string driverPath = @"C:\WINDOWS\system32\spool\drivers\w32x86\PSCRIPT5.DLL";
                string dataPath = @"C:\WINDOWS\system32\spool\drivers\w32x86\VPRINTER.PPD";
                string configPath = @"C:\WINDOWS\system32\spool\drivers\w32x86\PS5UI.DLL";
                string helpPath = @"C:\WINDOWS\system32\spool\drivers\w32x86\PSCRIPT.HLP";

                //0 - Set Printer Driver Path and Files
                LogHelper.Log("Setting Driver Path and Files.");
                GenericResult printerDriverPath = GetPrinterDirectory();
                if (printerDriverPath.Success == true)
                {
                    driverPath = string.Format("{0}\\{1}", printerDriverPath.Message, driverFileName);
                    dataPath = string.Format("{0}\\{1}", printerDriverPath.Message, dataFileName);
                    configPath = string.Format("{0}\\{1}", printerDriverPath.Message, configFileName);
                    helpPath = string.Format("{0}\\{1}", printerDriverPath.Message, helpFileName);
                }

                //1 - Add Printer Monitor
                LogHelper.Log("Adding Printer Monitor.");
                GenericResult printerMonitorResult = AddPrinterMonitor(monitorName);
                if (printerMonitorResult.Success == false)
                {
                    if (printerMonitorResult.Message.ToLower() != "the specified print monitor has already been installed")
                        throw printerMonitorResult.Exception;
                }

                //2 - Add Printer Port
                LogHelper.Log("Adding Printer Port.");
                GenericResult printerPortResult = AddPrinterPort(portName, monitorName);
                if (printerPortResult.Success == false)
                    throw printerPortResult.Exception;

                //3 - Add Printer Driver
                LogHelper.Log("Adding Printer Driver.");
                GenericResult printerDriverResult = AddPrinterDriver(driverName, driverPath, dataPath, configPath, helpPath);
                if (printerDriverResult.Success == false)
                    throw printerDriverResult.Exception;

                //4 - Add Printer
                LogHelper.Log("Adding Printer");
                GenericResult printerResult = AddPrinter(printerName, portName, driverName);
                if (printerResult.Success == false)
                    throw printerResult.Exception;

                //5 - Configure Virtual Port
                LogHelper.Log("Configuring Virtual Port");
                GenericResult configResult = ConfigureVirtualPort(monitorName, portName, key);
                if (configResult.Success == false)
                    throw configResult.Exception;

                //6 - Restart Spool Service
                LogHelper.Log("Restarting Spool Service");
                GenericResult restartSpoolResult = RestartSpoolService();
                if (restartSpoolResult.Success == false)
                    throw restartSpoolResult.Exception;

                LogHelper.Log("AddVPrinter Success");
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
                LogHelper.Log(string.Format("Exception: {0}", ex.Message));
            }

            return retVal;
        }


        public class GenericResult
        {
            public GenericResult(string method)
            {
                Success = false;
                Message = string.Empty;
                Exception = null;
                _method = method;
            }
            public bool Success { get; set; }
            public string Message { get; set; }
            public Exception Exception { get; set; }
            private string _method;
            public string Method
            {
                get { return _method; }
            }
        }
    }
}
