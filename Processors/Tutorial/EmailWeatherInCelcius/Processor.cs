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
using System.Net.Mail;
using PrinterPlusPlusSDK;

namespace EmailWeatherInCelcius
{
    public class Processor : PrinterPlusPlusSDK.IProcessor
    {
        public PrinterPlusPlusSDK.ProcessResult Process(string key, string psFilename)
        {
            //Convert PS to Text
            var txtFilename = System.IO.Path.GetTempFileName();
            ConvertPsToTxt(psFilename, txtFilename);

            //Process the converted Text File
            var extractedValue = ProcessTextFile(txtFilename);

            //Ask user for recipeint's email
            var recipients = Microsoft.VisualBasic.Interaction.InputBox("Enter email address of recipient.");

            //Send email if user entered an email address
            if (!string.IsNullOrWhiteSpace(recipients))
            {
                SendEmail(extractedValue, recipients);
            }

            return new ProcessResult();
        }

        public static string ConvertPsToTxt(string psFilename, string txtFilename)
        {
            var retVal = string.Empty;
            var errorMessage = string.Empty;
            var command = "C:\\ps2txt\\ps2txt.exe";
            var args = string.Format("-nolayout \"{0}\" \"{1}\"", psFilename, txtFilename);
            retVal = Shell.ExecuteShellCommand(command, args, ref errorMessage);
            return retVal;
        }

        public ExtractedValue ProcessTextFile(string txtFilename)
        {
            var values = new ExtractedValue();      //Create the extracted values placeholders
            var reachedMarker = false;
            //Read the text file
            using (System.IO.StreamReader sr = System.IO.File.OpenText(txtFilename))
            {
                while (sr.Peek() > -1)
                {
                    var currentLine = sr.ReadLine().Trim();

                    //Skip whitespaces
                    if (string.IsNullOrWhiteSpace(currentLine))
                        continue;

                    //Checked if we've reached the marker to begin extraction
                    if (reachedMarker == true)
                    {
                        //Get Title value
                        if (string.IsNullOrWhiteSpace(values.Title))
                        {
                            values.Title = currentLine;
                            continue;
                        }
                        //Skip Right Now value
                        if (currentLine.ToLower() == "right now")
                            continue;
                        //Get UpdatedDateTime value
                        if (string.IsNullOrWhiteSpace(values.UpdatedDateTime))
                        {
                            values.UpdatedDateTime = currentLine;
                            continue;
                        }
                        //Get TemperatureFahrenheit and convert value to Celcius
                        if (values.TemperatureFahrenheit == 0)
                        {
                            values.TemperatureFahrenheit = Convert.ToDouble(currentLine.Substring(0, currentLine.Length - 2));
                            values.TemperatureCelcius = ((values.TemperatureFahrenheit - 32) / 1.8);
                            continue;
                        }
                        //Get Forecast value
                        if (string.IsNullOrWhiteSpace(values.Forecast))
                        {
                            values.Forecast = currentLine;
                            break;
                        }
                    }
                    //Mark farming so we can begin extracting data
                    if (currentLine.ToLower() == "farming")
                        reachedMarker = true;
                }
            }
            return values;      //Return the extracted values
        }

        public class ExtractedValue
        {
            public string Title { get; set; }
            public string UpdatedDateTime { get; set; }
            public double TemperatureFahrenheit { get; set; }
            public double TemperatureCelcius { get; set; }
            public string Forecast { get; set; }
        }

        public void SendEmail(ExtractedValue values, string recipient)
        {
            try
            {
                var smtpHost = "mail.nightshade.arvixe.com";
                var sender = "info@printerplusplus.com";
                SmtpClient client = new SmtpClient(smtpHost, 26);

                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("rowell@opxsoftinc.com", "banana");

                MailAddress from = new MailAddress(sender);
                MailAddress to = new MailAddress(recipient);
                MailMessage message = new MailMessage(from, to);
                message.Body = values.Title;
                message.Body += Environment.NewLine;
                message.Body += values.UpdatedDateTime;
                message.Body += Environment.NewLine;
                message.Body += values.TemperatureCelcius;
                message.Body += Environment.NewLine;
                message.Body += values.Forecast;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = "Printer++ EmailWeatherInCelcius";
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                client.Send(message);
                message.Dispose();
            }
            catch (Exception ex)
            {
                //Error occured while sending email. Add code to handle error.
            }
        }
    }
}
