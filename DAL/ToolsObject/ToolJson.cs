using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.ToolsObject
{
    public class ToolJson : ToolTableColumn, ICRUD<Tool>
    {
        string file_path = ""; //Init.ToolFileName.FullFilePath(); //Utilities.PersonJsonFile.FullFilePath(); //Use "PersonJsonFile" to include file name to "FullFilePath"
        string error_folder = Init.ErrorDataFolder;
        //string file_path = bll_Utilities.PersonJsonFilePath;
        public ToolJson(string tool_file_name)
        {
            file_path = tool_file_name.FullFilePath();
        }
        
        public bool Create(Tool obj)
        {
            try
            {
                List<Tool> tool = ToolJsonProcessor.getJsonDataObject(file_path);

                int currentId = 1;

                if (tool != null)
                {
                    currentId = tool.OrderByDescending(x => x.m_id).First().m_id + 1;
                }

                //if (people.Count > 0)
                //{
                //    currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
                //}

                obj.m_id = currentId;

                string json_basedata = ToolJsonProcessor.getJsonDataString(file_path);
                string s = ToolJsonProcessor.AddObjectsToJson(json_basedata, obj, file_path);

                return true;
            }
            catch (System.Exception ex)
            {
                string error = "Create person json file exceptio: " + ex.Message;
                ErrorHandling.WriteError(error, error_folder);
                return false;
            }
        }

        public Tool Retrieve(string key)
        {
            var person = (from s in RetrieveAll() where key == s.m_id.ToString() select s).FirstOrDefault();
            return person;
        }

        public List<Tool> RetrieveAll()
        {
            return ToolJsonProcessor.getJsonDataObject(file_path);
        }

        public bool Update(Tool obj, string key)
        {
            int id = Int32.Parse(key);
            if (ToolJsonProcessor.UpdateRow(file_path, id, obj))
                return true;
            else
                return false;
        }

        public bool Delete(string key)
        {
            int id = Int32.Parse(key);
            if (ToolJsonProcessor.DeleteRow(file_path, id))
                return true;
            else
                return false;
        }
    }
}
