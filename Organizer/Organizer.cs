using DAL;
using DAL.ToolsObject;
using Organizer.Models;
using RKLib.ExportData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Organizer
{
    public partial class Organizer : Form
    {
        #region :Delarations
        Utilities ut = new Utilities();
        List<Tool> lsTools = null; //Get unsorted list
        IEnumerable<Tool> lsSortedTools = null; //Get Sorted List  
        ToolLogic logic;
        private int tool_id { get; set; }
        private string created { get; set; }
        private string updated { get; set; }
        private string imagepath { get; set; }     //for tool picture path
        private string tempimagepath { get; set; }
        private string toolurl { get; set; }    //hold selected tool url for click view
        #endregion

        public Organizer()
        {
            InitializeComponent();

            toolStripStatusLabel1.Text = ut.ProductCopyright;

            //Set 
            DirectoryInfo dirData = new DirectoryInfo(Utilities.ToolsImagesFolder);
            if (!dirData.Exists)
            {
                dirData.Create();
            }

            Utilities.setDefaultPicture(Utilities.DefaultPicture);

            Reset();
        }

        #region :Methods
        private void ShowTools()
        {
            if(lsTools.Count > 0)
            {
                //Sort the list
                lsSortedTools = from t in lsTools
                                orderby t.m_name ascending
                                select t;
                lbxTools.Items.Clear();
                lbxTools.Items.Insert(0, "Click this to add new");
                int i = 0;
                foreach (Tool s in lsSortedTools)
                {
                    lbxTools.Items.Add(s.m_name);
                    i++;
                }

                lblMsg.Text = "Found: " + i;
            }

            ExportToExcel();
        }

        private void getTool(int id)
        {
            Tool tool = lsTools.FirstOrDefault(x => x.m_id == id);
            
            if (tool != null)
            {
                tool_id = tool.m_id;
                txtName.Text = tool.m_name;
                txtUrl.Text = tool.m_url;
                toolurl = tool.m_url;
                txtNotes.Text = tool.m_notes;
                if(!string.IsNullOrEmpty(tool.m_picture))
                {
                    pbxPicture.Image = Image.FromFile(tool.m_picture);
                }
                
                created = tool.m_created;
                lblMsg.Text = String.Format("Id:{0}, Created:{1}", tool.m_id, tool.m_created);
                btnDelete.Visible = true;
                btnView.Visible = true;
            }
            else
            {
                MessageBox.Show("No data was returned!", Utilities.MsgBoxHead);
            }
        }

        private void SaveRecord(ToolLogic logic)
        {
            
            if(!valSave())
            {
                return;
            }
            else
            {
                DirectoryInfo dirData = new DirectoryInfo(Utilities.ToolsFolder);
                if (!dirData.Exists)
                {
                    dirData.Create();
                }

                DirectoryInfo dirImages = new DirectoryInfo(Utilities.ToolsImagesFolder);
                if (!dirImages.Exists)
                {
                    dirImages.Create();
                }

                Tool tool = new Tool();
                try
                {
                    tool.m_id = tool_id;
                    tool.m_name = txtName.Text.Trim();
                    tool.m_url = txtUrl.Text.Trim();
                    tool.m_notes = txtNotes.Text.Trim();
                    tool.m_updated = DateTime.Now.ToString();

                    //Set Picture
                    string picnewname = "";
                    tempimagepath = "";
                    FileInfo flePicture = new FileInfo(imagepath);
                    if (tempimagepath.Length > 0)
                    {
                        if (tempimagepath == imagepath)
                        {
                            picnewname = tempimagepath;
                        }
                        else
                        {
                            picnewname = @"" + Utilities.ToolsImagesFolder + "\\" + txtName.Text + Utilities.RandomNumber(1, 100) + flePicture.Extension;
                            flePicture.CopyTo(picnewname);
                        }

                    }
                    else
                    {
                        picnewname = @"" + Utilities.ToolsImagesFolder + "\\" + txtName.Text + flePicture.Extension;
                        //if picture exist don't copy
                        if (!File.Exists(picnewname))
                        {
                            flePicture.CopyTo(picnewname);
                        }
                        else
                        {
                            picnewname = imagepath;
                        }
                    }

                    tool.m_picture = picnewname;
                    //tool.m_id = id.ToString();

                    //If already exist do update else Insert
                    var record = lsTools.FirstOrDefault(x => x.m_id == tool_id);
                    if(record != null)
                    {
                        //Update
                        //Validate Duplicate

                        tool.m_created = created;
                        logic.UpdateRecord(tool, tool_id.ToString());

                    }
                    else
                    {
                        //Insert
                        //Validate Duplicate
                        tool.m_created = DateTime.Now.ToString();
                        logic.CreateRecord(tool);
                    }

                    ExportToExcel();

                    Reset();

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message + " \n" + ex.InnerException, Utilities.MsgBoxHead);
                }

            }
        }

        private void DeleteTool(ToolLogic logic, int id)
        {
            logic.DeleteRecord(id.ToString());

            Reset();
        }

        private void Reset()
        {
            //Set 
            Init init = new Init(Utilities.ToolsFolder, Utilities.ErrorFolder);
            logic = new ToolLogic(Utilities.ToolsDataFileName);
            lsTools = logic.Records;

            imagepath = Utilities.DefaultPicture;

            ResetToolFields();

            ShowTools();
        }

        private void ResetToolFields()
        {
            tool_id = 0;
            txtName.Text = "";
            txtUrl.Text = "";
            txtNotes.Text = "";
            lblMsg.Text = "";
            toolurl = "";
            txtSearch.Text = "";
            pbxPicture.Image = null;
            btnView.Visible = false;
            btnDelete.Visible = false;
        }

        // List To Table - Usage ex:
        // DataTable dt = cl_DataConversion.ToDataTable(cl_ReadData.getStudents);
        // or
        // List<cl_Student> ls_stu = cl_ReadData.getStudents;
        // DataTable dt = cl_DataConversion.ToDataTable(ls_stu);
        internal DataTable ToolsDataRecords
        {
            get
            {
                DataTable dt = new DataTable();
                dt = DataConvertor.ToDataTable(lsTools);
                return dt;
            }
            
        }

        private List<Tool> getToolsLike(string tool_name)
        {
            List<Tool> lsToolsLike = new List<Tool>();
            var tools = from ls in lsTools
                        where ls.m_name.ToLower().Contains(tool_name.ToLower())
                        select ls;
            foreach(Tool t in tools)
            {
                lsToolsLike.Add(t);
            }

            return lsToolsLike;
        }

        private void ExportToExcel()
        {
            try
            {
                string fpath = Utilities.ToolsExcelFile;

                // Specify the columns in the list to export
                int[] iColumns = { 0, 1, 2, 3, 4, 5 };

                // Export the details of specified columns to CSV
                RKLib.ExportData.Export objExport = new Export("Win");
                objExport.ExportDetails(ToolsDataRecords, iColumns, Export.ExportFormat.Excel, fpath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, Utilities.MsgBoxHead);
            }

        }
        #endregion

        #region :Validations
        internal bool valSave()
        {
            string msg = "";
            if (txtName.Text == "")
            {
                msg += " Tool name is required!";
            }
            if (txtUrl.Text == "")
            {
                msg += "\n Tool url is required!";
            }

            if (msg != "")
            {
                MessageBox.Show(msg, Utilities.MsgBoxHead);
                return false;
            }
            else
            {
                return true;
            }

        }
        #endregion

        #region :Click Events
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text.Length > 0)
            {
                lsTools = getToolsLike(txtSearch.Text.Trim());
            }
            else
            {
                lsTools = logic.Records;
            }

            ShowTools();
        }
        private void lbxTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lbxTools.SelectedIndex < 1)
            {
                Reset();
            }
            else if(lbxTools.SelectedIndex > 0)
            {
                //ResetToolFields(); //This is causing the search unable to find selected item

                int counter = 1;
                int tool_id = 0;
                foreach(Tool s in lsSortedTools)
                {
                    if(lbxTools.SelectedIndex == counter)
                    {
                        tool_id = s.m_id;
                        break;
                    }
                    counter++;
                }

                if(tool_id > 0)
                {
                    getTool(tool_id);
                }
                else
                {
                    MessageBox.Show("No record was returned!", Utilities.MsgBoxHead);
                }
            }

        }
        private void btnCSVExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            List<Tool> ls = logic.Records;
            DataTable dt = DataConvertor.ToDataTable(ls);
            //DataSet ds = bal_DataConvertor.dtToDataSet(dt);

            dlg.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            dlg.Title = "Export in CSV format";

            //decide whether we need to check file exists
            //dlg.CheckFileExists = true;

            //this is the default behaviour
            dlg.CheckPathExists = true;

            //If InitialDirectory is not specified, the default path is My Documents
            //dlg.InitialDirectory = Application.StartupPath;

            dlg.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (dlg.FileName != "")

            //alternative if you prefer this
            //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
            //&& dlg.FileName.Length > 0)
            {
                string saveAs_file_path = dlg.FileName;

                //First Approach
                // Specify the column list to export
                int[] iColumns = { 0, 1, 2, 3, 4, 5, 6};

                // Export the details of specified columns to CSV
                RKLib.ExportData.Export objExport = new Export("Win");
                objExport.ExportDetails(dt, iColumns, Export.ExportFormat.CSV, saveAs_file_path);

                //Second Approach
                /*
                StreamWriter streamWriter = new StreamWriter(saveAs_file_path);
                streamWriter.Write("User Accounts Record. File created on: " + DateTime.Now.ToString());
                //Note streamWriter.NewLine is same as "\r\n"
                streamWriter.Write(streamWriter.NewLine);
                streamWriter.Write("\r\n");
                //streamWriter.Write("Date, Title, Memo\r\n");    //Header
                streamWriter.Write(bal_DataConvertor.ListToCSVFile(ls));   //Content                   
                streamWriter.Close();
                */

                MessageBox.Show("CSV file created successfully.", Utilities.MsgBoxHead, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Please select save location", Utilities.MsgBoxHead, MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand);
            }
        }
        private void btnXMLExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            List<Tool> ls = logic.Records;
            DataTable dt = DataConvertor.ToDataTable(ls);
            DataSet ds = DataConvertor.dtToDataSet(dt);

            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.Title = "Export in XML format";

            //decide whether we need to check file exists
            //dlg.CheckFileExists = true;

            //this is the default behaviour
            dlg.CheckPathExists = true;

            //If InitialDirectory is not specified, the default path is My Documents
            //dlg.InitialDirectory = Application.StartupPath;

            dlg.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (dlg.FileName != "")

            //alternative if you prefer this
            //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
            //&& dlg.FileName.Length > 0)
            {
                string saveAs_file_path = dlg.FileName;

                //First Approach
                ds.WriteXml(saveAs_file_path);

                //Second Approach
                /*
                StreamWriter streamWriter = new StreamWriter(saveAs_file_path);
                streamWriter.Write("User Accounts Record. File created on: " + DateTime.Now.ToString());
                //Note streamWriter.NewLine is same as "\r\n"
                streamWriter.Write(streamWriter.NewLine);
                streamWriter.Write("\r\n");
                //streamWriter.Write("Date, Title, Memo\r\n");    //Header
                streamWriter.Write(bal_DataConvertor.ListToXML(ls));   //Content                   
                streamWriter.Close();
                */

                MessageBox.Show("XML file created successfully.", Utilities.MsgBoxHead, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select save location", Utilities.MsgBoxHead, MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand);
            }
        }
        /* This has ben replaced with XML Export and CSV Export methods.
        private void btnExport_Click(object sender, EventArgs e)
        {
            //Open the excel file
            try
            {
                Process.Start(Utilities.ToolsExcelFile);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Something is wrong: \n" + ex.Message, Utilities.MsgBoxHead);
            }
        }
        */

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveRecord(logic);
        }

        private void btnPicture_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                imagepath = dlgOpenFile.FileName;
                pbxPicture.Image = Image.FromFile(imagepath);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(toolurl);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: \n" + ex.Message, Utilities.MsgBoxHead);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to delete selected tool?",
                Utilities.MsgBoxHead, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                //Do something
                DeleteTool(logic, tool_id);
            }
            else
            {
                return;
            }
        }
        #endregion
        
    }
}
