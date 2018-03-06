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
using System.IO;
using System.Security.Permissions;
using PrinterPlusPlusSDK;

namespace PrinterPlusPlus
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class IOMonitorHelper
    {
        private FileSystemWatcher fsw = null;

        public void StartMonitor(string path)
        {
            if (System.IO.Directory.Exists(path))
            {
                fsw = new FileSystemWatcher(path, "*.ps");
                fsw.NotifyFilter = System.IO.NotifyFilters.DirectoryName;

                fsw.NotifyFilter = fsw.NotifyFilter | System.IO.NotifyFilters.FileName;
                fsw.NotifyFilter = fsw.NotifyFilter | System.IO.NotifyFilters.Attributes;

                fsw.Created += new FileSystemEventHandler(fsw_eventHandler);
                //fsw.Changed += new FileSystemEventHandler(fsw_eventHandler);
                //fsw.Deleted += new FileSystemEventHandler(fsw_eventHandler);
                //fsw.Renamed += new RenamedEventHandler(fsw_eventHandler);

                try
                {
                    fsw.EnableRaisingEvents = true;
                }
                catch (ArgumentException ex)
                {
                    IO.Log(ex.Message);
                    throw;
                }
            }
            else
            {
                IO.Log(string.Format("Unable to monitor folder: {0}. Folder does not exist.", path));
            }
        }

        public void StopMonitor()
        {
            if (fsw != null)
            {
                fsw.EnableRaisingEvents = false;
                fsw.Dispose();
            }
        }

        private void fsw_eventHandler(object sender, System.IO.FileSystemEventArgs e)
        {
            var sleepTimeout = 100;
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    while (IO.IsFileLocked(e.FullPath) == true)
                    {
                        IO.Log("Locked...Sleeping...");
                        System.Threading.Thread.Sleep(sleepTimeout);
                    }
                    IO.Log(string.Format("Created: {0}", e.FullPath));
                    ProcessorHelper.Process(e.FullPath);
                    break;
                //case WatcherChangeTypes.Changed:
                //    IO.Log(string.Format("Changed: {0}", e.FullPath));
                //    break;
                //case WatcherChangeTypes.Deleted:
                //    IO.Log(string.Format("Deleted: {0}", e.FullPath));
                //    break;
                //case WatcherChangeTypes.Renamed:
                //    IO.Log(string.Format("Renamed: {0}", e.FullPath));
                //    break;
                default: // Another action
                    break;
            }
        }

        
    }
}
