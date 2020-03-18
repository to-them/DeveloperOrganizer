using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppUI
{
    public class Utilities
    {
        private static string AppBaseDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
        public static string DataFolder = AppBaseDir + AppKeyLookup("DataFolder");
        public static string ErrorFolder = DataFolder + AppKeyLookup("ErrorFolder");
        public static string PersonsJsonFile = AppKeyLookup("PersonsJsonFile");
        public static string ToolJsonFileName = AppKeyLookup("ToolJsonFileName");

        //App_Settings lookup
        /// <summary>
        /// Get App_Settings value specified in the config file
        /// </summary>
        /// <param name="key">App_Settings key</param>
        /// <returns>Returns the value</returns>
        public static string AppKeyLookup(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
