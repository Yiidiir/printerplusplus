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
    public partial class frmMain : Form
    {
        private bool closedFromMenu = false;
        IOMonitorHelper iomh = null;

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(bool silentMode, bool noMonitor, string key, string fileName)
        {
            try
            {
                IO.Log(string.Format("Starting Printer++: ({0},{1},{2},{3})", silentMode, noMonitor, key, fileName));
                InitializeComponent();
                iomh = new IOMonitorHelper();

                if (silentMode == true)
                {
                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                }

                if (!noMonitor)
                    iomh.StartMonitor("C:\\PrinterPlusPlus\\Temp");

                if (!string.IsNullOrWhiteSpace(key))
                    txtKey.Text = key;
                else
                    txtKey.Text = ConfigurationManager.AppSettings["DefaultProcessor"];


                if (!string.IsNullOrWhiteSpace(fileName))
                    txtFileName.Text = fileName;

                Process(key, fileName);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Process(txtKey.Text, txtFileName.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtFileName.Text = fDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Process(string key, string fileName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(fileName))
                {
                    IO.Log(string.Format("Processing: {0}. {1}", key, fileName));
                    ProcessorHelper.Process(key, fileName);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }

        }

        #region Form
        private void ShowError(Exception ex)
        {
            //IO.Log(ex.Message);
            MessageBox.Show(ex.Message, "Unhandled Exception");
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
                closedFromMenu = true;

            if (closedFromMenu == false)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
                e.Cancel = true;
            }
            else
            {
                if (iomh != null)
                    iomh.StopMonitor();
            }
        }
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            mnuTray_ShowHide_Click(sender, e);
        }
        #endregion

        #region mnuTray
        private void mnuTray_Exit_Click(object sender, EventArgs e)
        {
            closedFromMenu = true;
            this.Close();
        }
        private void mnuTray_About_Click(object sender, EventArgs e)
        {

        }
        private void mnuTray_ShowHide_Click(object sender, EventArgs e)
        {
            //this.Visible = !this.Visible;
            //if (!this.ShowInTaskbar)
            //    this.ShowInTaskbar = true;
            this.ShowInTaskbar = !this.ShowInTaskbar;

            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Minimized;
        }
        #endregion

    }
}
