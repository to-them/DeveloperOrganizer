using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Organizer
{
    public class Utilities
    {
        #region :Directory Paths
        private static string AppBaseDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName; //Project Directory
        public static string DataFolder = AppBaseDir + ConfigAppSettingLookup("DataFolder"); // \Data
        public static string ErrorFolder = DataFolder + ConfigAppSettingLookup("ErrorFolder");
        public static string DefaultPicture = DataFolder + ConfigAppSettingLookup("DefaultPic"); // \Data\pic.jpg
        #endregion

        #region :Tool Data Paths
        public static string ToolsFolder = DataFolder + @"\Tools"; // \Data\Tools       
        public static string ToolsImagesFolder = ToolsFolder + @"\Images"; // \Data\Tools\Images
        public static string ToolsDataFileName = ConfigAppSettingLookup("ToolsDataFileName"); // tools.json (This will be merged with the folder in the code)
        public static string ToolsExcelFile = ToolsFolder + @"\" + ConfigAppSettingLookup("ToolsExcelFileName"); // \Data\Tools\tools.xls
        #endregion

        #region :Other
        ////file extension
        //public static string datafileExt = ".json";
        //public static string excelfileExt = ".xls";

        //Message Box Header
        public static string MsgBoxHead = ConfigAppSettingLookup("MsgBoxHead");
        #endregion

        #region :Methods
        //Read from Assembly Information
        public string ProductCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        //App_Settings lookup
        /// <summary>
        /// Get App_Settings value specified in the config file
        /// </summary>
        /// <param name="key">App_Settings key</param>
        /// <returns>Returns the value</returns>
        public static string ConfigAppSettingLookup(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        //get default image
        public static void setDefaultPicture(string defaultpic)
        {
            //create location directory
            Directory.CreateDirectory(@"" + Utilities.ToolsFolder);

            //get installed default picture 
            FileInfo flePicture = new FileInfo(DefaultPicture);

            //if picture don't exist copy it
            //from installed location 
            if (!File.Exists(defaultpic))
            {
                flePicture.CopyTo(defaultpic);
            }
        }

        //generate random number each time is been called
        //start at min and increment each call by to the max
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        #endregion

        //Export Customers Data to Excel
        /*
        public static DataTable ToolsTable
        {
            get
            {
                //initilise table 
                DataTable tbl = new DataTable();
                DataColumn tc = null;
                DataRow tr = null;

                //create table columns
                tc = new DataColumn("ID", Type.GetType("System.String"));
                tbl.Columns.Add(tc);
                tc = new DataColumn("Name", Type.GetType("System.String"));
                tbl.Columns.Add(tc);
                tc = new DataColumn("URL", Type.GetType("System.String"));
                tbl.Columns.Add(tc);
                tc = new DataColumn("Created", Type.GetType("System.String"));
                tbl.Columns.Add(tc);
                tc = new DataColumn("Updated", Type.GetType("System.String"));
                tbl.Columns.Add(tc);
                tc = new DataColumn("Picture", Type.GetType("System.String"));
                tbl.Columns.Add(tc);

                string strFilename = ToolsFile; //@"" + Customers + "\\customers" + Ext;

                if (File.Exists(strFilename))
                {
                    FileStream stmTools = new FileStream(strFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    try
                    {
                        // Retrieve the list of customers from file
                        lsTools = (Dictionary<string, Tool>)
                        bfmTools.Deserialize(stmTools);
                    }
                    finally
                    {
                        stmTools.Close();
                    }
                }
                if (lsTools.Count > 0)
                {
                    foreach (KeyValuePair<string, Tool> kvp in lsTools)
                    {
                        Tool tool = kvp.Value;
                        string cid = kvp.Key;
                        tr = tbl.NewRow();
                        tr["ID"] = tool.m_id;
                        tr["Name"] = tool.m_name;
                        tr["URL"] = tool.m_url;
                        tr["Created"] = tool.m_created;
                        tr["Updated"] = tool.m_updated;
                        tr["Picture"] = tool.m_picture;
                        tbl.Rows.Add(tr);
                    }
                }
                return tbl;
            }
        }
        */

    }
}
