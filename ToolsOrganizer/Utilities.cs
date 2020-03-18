using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ToolsOrganizer
{
    public class Utilities
    {
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

        //paths
        private static string AppBaseDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
        private static string DataFolder = AppBaseDir + ConfigurationManager.AppSettings["DataFolder"].ToString(); // ..\Resources\Data
        public static string DefaultPicture = AppBaseDir + ConfigurationManager.AppSettings["DefaultPic"].ToString();
        public static string Tools = DataFolder + @"\Tools"; //Tools Folder
        public static string ToolsImages = Tools + @"\Images"; //Tools Images Folder
        public static string ToolsFile = Tools + @"\tools" + Ext;
        public static string ToolsExcelFile = Tools + @"\Tools.xls"; //Tools Execel file
        public static string MsgBoxHead = ConfigurationManager.AppSettings["MsgBoxHead"].ToString();

        //file extension
        public static string Ext = ".inf";

        //get default image
        public static void setDefaultPicture(string defaultpic)
        {
            //create location directory
            Directory.CreateDirectory(@"" + Utilities.Tools);

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

        //Export Customers Data to Excel
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

                Dictionary<string, Tool> lsTools = new Dictionary<string, Tool>();
                BinaryFormatter bfmTools = new BinaryFormatter();

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

    }
}
