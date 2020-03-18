using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.ToolsObject
{
    public class ToolLogic
    {
        ICRUD<Tool> t;
        public ToolLogic(string tool_file_name)
        {
            t = new ToolJson(tool_file_name);
        }

        #region :Create
        public bool CreateRecord(Tool model)
        {
            if (t.Create(model))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region :Read
        public List<Tool> Records
        {
            get
            {
                List<Tool> ls = new List<Tool>();
                ls = t.RetrieveAll();
                if (ls == null || ls.Count() < 1)
                {
                    ls = SeedTool;
                }
                return ls;

                //return p.RetrieveAll();
            }
        }

        public Tool Record(string key)
        {
            return t.Retrieve(key);
        }
        #endregion

        #region :Update
        public bool UpdateRecord(Tool model, string key)
        {
            if (t.Update(model, key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region :Delete
        public bool DeleteRecord(string key)
        {
            if (t.Delete(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region :Seed Default Data
        /// <summary>
        /// Initialize the JSON file
        /// </summary>
        public static List<Tool> SeedTool
        {
            get
            {
                List<Tool> ls = new List<Tool>();
                ls.Add(new Tool()
                {
                    m_id = 1,
                    m_name = "Seed Name",
                    m_notes = "Seed Notes",
                    m_url = "http://google.com",
                    m_picture = "",
                    m_created = DateTime.Now.ToString(),
                    m_updated = DateTime.Now.ToString()
                });
                return ls;
            }
        }
        #endregion
    }
}
