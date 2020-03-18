using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RKLib.ExportData;

namespace ToolsOrganizer
{
    public partial class Main : Form
    {
        Utilities ut = new Utilities();
        Dictionary<string, Tool> lsTools;
        private int id;  //to generate new file id
        private string created { get; set; }
        private string updated { get; set; }
        private string imagepath { get; set; }     //for tool picture path
        private string tempimagepath { get; set; }
        private string toolurl { get; set; }    //hold selected tool url for click view
        private string selectedKey { get; set; }   //for selected customer on listview
        private string ToolsFile = Utilities.ToolsFile; //@"" + Utilities.Tools + "\\tools" + Utilities.Ext;
        public Main()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = ut.ProductCopyright;
            Utilities.setDefaultPicture(Utilities.DefaultPicture);

            ShowTools();
            getToolID();
            ExportToExcel();

        }

        private Dictionary<string, Tool> getTools
        {
            get
            {
                Dictionary<string, Tool> ls = new Dictionary<string, Tool>();
                BinaryFormatter bfmTools = new BinaryFormatter();

                if (File.Exists(ToolsFile))
                {
                    FileStream stmCustomers = new FileStream(ToolsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    try
                    {
                        // Retrieve the list of receipts from file
                        ls = (Dictionary<string, Tool>)
                        bfmTools.Deserialize(stmCustomers);
                    }
                    finally
                    {
                        stmCustomers.Close();
                    }

                }
                return ls;
            }
        }

        private void getToolID()
        {
            //create new id
            Tool tool = new Tool();
            lsTools = new Dictionary<string, Tool>();
            lsTools = getTools;

            //BinaryFormatter bfmTools = new BinaryFormatter();
            //if (File.Exists(ToolsFile))
            //{
            //    FileStream stmCustomers = new FileStream(ToolsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            //    try
            //    {
            //        // Retrieve the list of receipts from file
            //        lsTools = (Dictionary<string, Tool>)
            //        bfmTools.Deserialize(stmCustomers);
            //    }
            //    finally
            //    {
            //        stmCustomers.Close();
            //    }

            //}

            if (lsTools.Count > 0)
            {
                int i = 1;
                foreach (KeyValuePair<string, Tool> kvp in lsTools)
                {
                    if (i == lsTools.Count)
                    {
                        string dKey = kvp.Key;
                        id = Convert.ToInt32(dKey) + 1;

                    }
                    i++;

                }
            }
            else
            {
                id = 1;
            }
            //get default image to avoid compiler warning
            imagepath = Utilities.DefaultPicture;
            lblMsg.Text = "Create new or select exiting to edit...";

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
                objExport.ExportDetails(Utilities.ToolsTable, iColumns, Export.ExportFormat.Excel, fpath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, Utilities.MsgBoxHead);
            }

        }

        internal void ShowTools()
        {
            Reset();
            lsTools = new Dictionary<string, Tool>();
            lsTools = getTools;

            if (lsTools.Count == 0)
                return;
            else
            {               
                lvwTools.Items.Clear();
                int i = 1;

                //Sort by name
                foreach (KeyValuePair<string, Tool> kvp in lsTools.OrderBy(x => x.Value.m_name))
                {
                    ListViewItem lviTool = new ListViewItem(kvp.Key);

                    Tool tool = kvp.Value;
                    string cid = kvp.Key;
                    lviTool.SubItems.Add(tool.m_name);                   

                    if (i % 2 == 0)
                    {
                        lviTool.BackColor = Color.Navy;
                        lviTool.ForeColor = Color.White;
                    }
                    else
                    {
                        lviTool.BackColor = Color.Black;
                        lviTool.ForeColor = Color.White;
                    }

                    lvwTools.Items.Add(lviTool);

                    i++;
                }

                //var dictval = from x in lsTools
                //              where x.Key.Contains("3")
                //              select x;
                ////MessageBox.Show(dictval.FirstOrDefault<string, Tool>().Value, "Ayitech");

                ExportToExcel(); //will do later

            }

        }

        //IEnumerable<Tool> SortedAccounts = null; //Get Sorted List
        //private void LoadTools()
        //{
        //    lsTools = new Dictionary<string, Tool>();
        //    lsTools = getTools;
        //    if(lsTools.Count > 0)
        //    {
        //        ////Sort Account
        //        //SortedAccounts = from a in lsTools
        //        //                 orderby a.Value ascending
        //        //                 select a;

        //        lbxAccounts.Items.Clear();
        //        lbxAccounts.Items.Insert(0, "Click to refresh for new");
        //        lbxAccounts.DataSource = new BindingSource(lsTools, null);
        //        lbxAccounts.DisplayMember = "Value";
        //        lbxAccounts.ValueMember = "Key";
        //        //lbxAccounts.DataBindings();
        //        //int i = 0;
        //        //foreach (KeyValuePair<string, Tool> kvp in lsTools)
        //        //{
                   
        //        //    ListViewItem lviTool = new ListViewItem(kvp.Key);

        //        //    Tool tool = kvp.Value;
        //        //    string cid = kvp.Key;
        //        //    lviTool.SubItems.Add(tool.m_name);

        //        //    if (i % 2 == 0)
        //        //    {
        //        //        lviTool.BackColor = Color.Navy;
        //        //        lviTool.ForeColor = Color.White;
        //        //    }
        //        //    else
        //        //    {
        //        //        lviTool.BackColor = Color.Black;
        //        //        lviTool.ForeColor = Color.White;
        //        //    }

        //        //    lvwTools.Items.Add(lviTool);

        //        //    i++;
        //        //}
        //    }
        //}

        #region Validate
        //validate save
        internal bool valSave()
        {
            string msg = "";
            if (txtName.Text == "")
            {
                msg += " Tool name is required!";
                //MessageBox.Show("Tool name is required!", Utilities.MsgBoxHead);
                //return false;
            }
            if (txtUrl.Text == "")
            {
                msg += "\n Tool url is required!";
                //MessageBox.Show("Tool url is required!", Utilities.MsgBoxHead);
                //return false;
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

        private bool valDuplicateInsert(string newname)
        {
            Tool tool = new Tool();
            lsTools = new Dictionary<string, Tool>();

            BinaryFormatter bfmTools = new BinaryFormatter();
            string strFilename = ToolsFile; //@"" + Utilities.Customers + "\\customers" + Utilities.Ext;

            if (File.Exists(strFilename))
            {
                FileStream stmTools = new FileStream(strFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    // Retrieve the list of receipts from file
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
                    tool = kvp.Value;
                    if (tool.m_name.ToLower() == newname.ToLower())
                    {
                        MessageBox.Show("Name already exist! \n" +
                            "Enter a different name.", Utilities.MsgBoxHead);
                        return true; 
                    }

                }
            }
            return false;
        }

        private bool valDuplicateUpdate(string tid, string tname)
        {
            Tool tool = new Tool();
            lsTools = new Dictionary<string, Tool>();

            BinaryFormatter bfmTools = new BinaryFormatter();
            string strFilename = ToolsFile; //@"" + Utilities.Customers + "\\customers" + Utilities.Ext;

            if (File.Exists(strFilename))
            {
                FileStream stmTools = new FileStream(strFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    // Retrieve the list of receipts from file
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
                    string id = kvp.Key;
                    tool = kvp.Value;
                    if (((tool.m_name.ToLower() == tname.ToLower())) && (id != tid))
                    {
                        MessageBox.Show("Name already exist! \n" +
                            "Enter a different name.", Utilities.MsgBoxHead);
                        return true;
                    }

                }
            }
            return false;
        }
        #endregion

        //save 
        internal void SaveTool()
        {
            //this uses number id as the key
            //this will also update
            //if no picture selected use default to prevent warning
            //then update image later

            if (!valSave())
                return;
            else
            {
                Directory.CreateDirectory(@"" + Utilities.Tools);
                Directory.CreateDirectory(@"" + Utilities.ToolsImages);
                Tool tool = new Tool();

                try
                {                   
                    tool.m_name = txtName.Text;
                    tool.m_url = txtUrl.Text;
                    //tool.m_created = DateTime.Now.ToString();
                    tool.m_updated = DateTime.Now.ToString();

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
                            picnewname = @"" + Utilities.ToolsImages + "\\" + txtName.Text + Utilities.RandomNumber(1, 100) + flePicture.Extension;
                            flePicture.CopyTo(picnewname);
                        }

                    }
                    else
                    {
                        picnewname = @"" + Utilities.ToolsImages + "\\" + txtName.Text + flePicture.Extension;
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
                    tool.m_id = id.ToString();

                    // If is already exist, do update
                    if (lsTools.ContainsKey(tool.m_id) == true)
                    {
                        // Simply update its value
                        //check for name duplication
                        if (valDuplicateUpdate(tool.m_id, txtName.Text)) 
                            return;
                        else
                        {
                            tool.m_created = created;
                            lsTools[tool.m_id] = tool;
                        }                            

                        //lsTools[tool.m_id] = tool; //just update for now will validate dup later
                    }
                    else
                    {
                        // If not exist, do insert
                        //check for name duplication
                        if (valDuplicateInsert(txtName.Text))
                            return;
                        else
                        {
                            tool.m_created = DateTime.Now.ToString();
                            lsTools.Add(tool.m_id, tool);
                        }
                            
                        //lsTools.Add(tool.m_id, tool); //just insert for now will validate dup later
                    }

                    FileStream bcrStream = new FileStream(ToolsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BinaryFormatter bcrBinary = new BinaryFormatter();

                    bcrBinary.Serialize(bcrStream, lsTools);
                    bcrStream.Close();

                    ExportToExcel();

                    ShowTools();
                    //MessageBox.Show("Details has been saved.", Utilities.MsgBoxHead);

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message + " \n" + ex.InnerException, Utilities.MsgBoxHead);
                }

            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveTool();
        }

        //reset form
        internal void Reset()
        {
            txtName.Text = "";
            txtUrl.Text = "";
            lblMsg.Text = "";
            toolurl = "";
            txtSearch.Text = "";
            pbxCustomer.Image = null;
            btnView.Visible = false;
            btnDelete.Visible = false;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Reset();
            getToolID();
        }

        private void lvwTools_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Reset();
            Tool tool = new Tool();
            string itm = e.Item.Text;

            foreach (KeyValuePair<string, Tool> kvp in lsTools)
            {
                selectedKey = kvp.Key;

                if (selectedKey == itm)
                {

                    try
                    {
                        tool.m_id = selectedKey;
                        id = Convert.ToInt32(selectedKey);
                        
                        tool = kvp.Value;
                        created = tool.m_created;
                        updated = tool.m_updated;
                        txtName.Text = tool.m_name;
                        txtUrl.Text = tool.m_url;
                        toolurl = tool.m_url;
                        imagepath = tool.m_picture;
                        tempimagepath = tool.m_picture;
                        if (imagepath.Length > 0 || imagepath != "")
                        {
                            pbxCustomer.Image = Image.FromFile(imagepath);
                        }

                        btnView.Visible = true;
                        btnDelete.Visible = true;

                        lblMsg.Text = "Id:" + id + " Last update:" + updated;

                        break;
                    }
                    catch (SystemException ex)
                    {
                        MessageBox.Show(ex.Message, Utilities.MsgBoxHead);
                    }
                }

            }

        }

        private void btnPicture_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                imagepath = dlgOpenFile.FileName;
                pbxCustomer.Image = Image.FromFile(imagepath);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to delete selected tool?", 
                Utilities.MsgBoxHead, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                //Do something
                if (lsTools.ContainsKey(selectedKey) == true)
                {
                    lsTools.Remove(selectedKey);
                    FileStream bcrStream = new FileStream(ToolsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BinaryFormatter bcrBinary = new BinaryFormatter();

                    bcrBinary.Serialize(bcrStream, lsTools);
                    bcrStream.Close();
                    ShowTools();

                    //MessageBox.Show("This will delete: " + selectedKey, Utilities.MsgBoxHead);
                }
                else
                {
                    MessageBox.Show("Tool not found!", Utilities.MsgBoxHead);
                }
            }
            else
            {
                return;
            }
            
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(toolurl);
            }
            catch(System.Exception ex)
            {
                MessageBox.Show("Something is wrong: \n" + ex.Message,Utilities.MsgBoxHead);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("Coming SOON!", Utilities.MsgBoxHead);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming SOON!", Utilities.MsgBoxHead);
        }

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
            
            //MessageBox.Show("Coming SOON!", Utilities.MsgBoxHead);
        }
    }
}
