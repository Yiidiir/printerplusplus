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
using System.Xml;

namespace PrinterPlusPlusSDK
{
    public static class ConfigHelper
    {
        public static void AddProcessor(string processorName, string assemblyQualifiedName)
        {
            XmlDocument doc = loadConfigDocument();
            XmlNode node = doc.SelectSingleNode("//processors");

            if (node == null)
                throw new InvalidOperationException("processors section not found in config file.");
            try
            {
                XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", processorName));
                if (elem != null)
                {
                    elem.SetAttribute("value", assemblyQualifiedName);
                }
                else
                {
                    elem = doc.CreateElement("add");
                    elem.SetAttribute("key", processorName);
                    elem.SetAttribute("value", assemblyQualifiedName);
                    node.AppendChild(elem);
                }
                doc.Save(getConfigFilePath());
                SetDefaultProcessor(processorName);
            }
            catch
            {
                throw;
            }
        }

        public static void SetDefaultProcessor(string processorName)
        {
            XmlDocument doc = loadConfigDocument();
            XmlNode node = doc.SelectSingleNode("//appSettings");
            var key = "DefaultProcessor";
            if (node == null)
                throw new InvalidOperationException("appSettings section not found in config file.");
            try
            {
                XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", key));
                if (elem != null)
                {
                    elem.SetAttribute("value", processorName);
                }
                else
                {
                    elem = doc.CreateElement("add");
                    elem.SetAttribute("key", key);
                    elem.SetAttribute("value", processorName);
                    node.AppendChild(elem);
                }
                doc.Save(getConfigFilePath());
            }
            catch
            {
                throw;
            }
        }
        private static XmlDocument loadConfigDocument()
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(getConfigFilePath());
                return doc;
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }
        }
        private static string getConfigFilePath()
        {
            return @"C:\PrinterPlusPlus\PrinterPlusPlus.exe.config";
        }

        //public static void SetDefaultProcessor(string processorName)
        //{
        //    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    config.AppSettings.Settings["DefaultProcessor"].Value = processorName;
        //    config.Save(ConfigurationSaveMode.Modified);
        //    //ConfigurationManager.RefreshSection(sectionName);
        //}
    }
}
