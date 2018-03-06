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
    public partial class frmRegistration : Form
    {
        public frmRegistration()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtRegistrationKey.Text))
            {
                //if (txtRegistrationKey.Text == "generateNEWkeyPrinter+Plus")
                //    txtRegistrationKey.Text = LicenseHelper.GetKey();
                //else
                //{
                    if (LicenseHelper.Register(txtRegistrationKey.Text) == true)
                    {
                        MessageBox.Show("Thank you for registering Printer++.\nPlease restart the Printer++ to apply the changes.", "Printer++");
                        //this.Hide();
                        //var frm = new frmMain(false, false, string.Empty, string.Empty);
                        //frm.ShowDialog();
                        this.Close();
                    }
                    else
                        MessageBox.Show("Invalid Registration Key.", "Printer++");
                //}
            }
            else
            {
                MessageBox.Show("Registration Key is empty.", "Printer++");
            }
        }

        private void frmRegistration_Load(object sender, EventArgs e)
        {
            txtProductId.Text = LicenseHelper.GetMachineGuid();
        }
    }
}
