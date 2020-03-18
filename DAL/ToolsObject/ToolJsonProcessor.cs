using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DAL.ToolsObject
{
    public static class ToolJsonProcessor
    {
        public static string AppDataFolder = Init.AppDataFolder;
        public static string ErrorDataFolder = Init.ErrorDataFolder;

        //private static string fileFolder = "filePath";
        public static string FullFilePath(this string fileName)
        {
            string fileDir = AppDataFolder; //HostingEnvironment.MapPath(ConfigurationManager.AppSettings[fileFolder].ToString()); //Ref System.Web
            //string fileDir = ConfigurationManager.AppSettings[fileFolder].ToString(); //Local Drive
            //Create file directory if it does not exist
            DirectoryInfo dir = new DirectoryInfo(fileDir);
            if (!dir.Exists)
            {
                dir.Create();
            }

            string file_path = fileDir + "\\" + fileName;
            //string file_path = ConfigurationManager.AppSettings[fileFolder] + "\\" + fileName;
            if (!File.Exists(file_path))
            {
                using (var jsonFile = File.Create(file_path))
                {
                    // interact with myFile here, it will be disposed automatically                   
                }

                //Create Initial/Seed JSON file
                WriteJsonData(ToolLogic.SeedTool, file_path);

            }

            return file_path;
            //return (ConfigurationManager.AppSettings[fileFolder] + "\\" + fileName);
            //return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        }

        #region :Create
        //Write from object
        /// <summary>
        /// Save List objects to file
        /// </summary>
        /// <param name="ls">List objects</param>
        /// <param name="file_path">Full path to the file location</param>
        public static void WriteJsonData(List<Tool> ls, string file_path)
        {
            try
            {
                // Pass the "person list" object for conversion object to JSON string  
                //string jsondata = new JavaScriptSerializer().Serialize(ls); //System.Web.Script.Serialization
                string jsondata = JsonConvert.SerializeObject(ls, Formatting.Indented); //Ref: Extension - Json.NET
                File.WriteAllText(file_path, jsondata);
            }
            catch (System.Exception ex)
            {
                string error = String.Format("WriteJsonData(ls, file_path) {0}", ex.Message);
                ErrorHandling.WriteError(error, ErrorDataFolder);
            }

        }

        //Re-write the file after manipulation
        /// <summary>
        /// Save string format of JSON data
        /// </summary>
        /// <param name="file_path">Full path to the file location</param>
        /// <param name="json_data">JSON data in string format</param>
        public static void WriteJsonData(string file_path, string json_data)
        {
            try
            {
                File.WriteAllText(file_path, json_data);
            }
            catch (System.Exception ex)
            {
                string error = String.Format("WriteJsonData(file_path, json_data) exception: {0}", ex.Message);
                ErrorHandling.WriteError(error, ErrorDataFolder);
            }

        }

        //Append person new person object and return updated data
        /// <summary>
        /// Add new person object to the list 
        /// </summary>
        /// <param name="json_basedata">JSON data in string format</param>
        /// <param name="obj">New person object</param>
        /// <param name="file_path">Full path to the file location</param>
        /// <returns>Returns string formatted JSON data</returns>
        public static string AddObjectsToJson(string json_basedata, Tool obj, string file_path)
        {
            //Add to the list
            List<Tool> ls = JsonConvert.DeserializeObject<List<Tool>>(json_basedata);
            ls.Add(obj);

            //Write to file
            WriteJsonData(ls, file_path);

            //return JsonConvert.SerializeObject(ls);
            return JsonConvert.SerializeObject(ls, Formatting.Indented);
        }

        /* Ref: https://stackoverflow.com/questions/33081102/json-add-new-object-to-existing-json-file-c-sharp
        //You could create a method:
        public string AddObjectsToJson<T>(string json, List<T> objects)
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(json);
            list.Add(objects);
            return JsonConvert.SerializeObject(list);
        }
        //Then use it like this:
        string baseJson = "[{\"id\":\"123\",\"name\":\"carl\"}]";
        List<Person> personsToAdd = new List<Person>() { new Person(1234, "carl2") };

        string updatedJson = AddObjectsToJson(baseJson, personsToAdd);
        
        */
        #endregion

        #region :Read
        ////Seed 
        ///// <summary>
        ///// Initialize the JSON file
        ///// </summary>
        //public static List<bel_Person> SeedPersonFile
        //{
        //    get
        //    {
        //        List<bel_Person> ls = new List<bel_Person>();
        //        ls.Add(new bel_Person()
        //        {
        //            Id = 0,
        //            FirstName = "SeedFirst",
        //            LastName = "SeedLast",
        //            EmailAddress = "seed@email.com",
        //            CellphoneNumber = "132-000-9800"
        //        });
        //        return ls;
        //    }
        //}

        //Get json data in string format
        /// <summary>
        /// Read all rows in the the file
        /// </summary>
        /// <param name="file_path">Full path of the file location</param>
        /// <returns>Returns data in string format</returns>
        public static string getJsonDataString(string file_path)
        {
            string jData = "";
            using (StreamReader r = new StreamReader(file_path))
            {
                jData = r.ReadToEnd();
            }

            return jData;
        }

        //Get json data in person list object
        /// <summary>
        /// Read all rows in JSON file
        /// </summary>
        /// <param name="file_path">Full path to the file</param>
        /// <returns>Returns List object of the file</returns>
        public static List<Tool> getJsonDataObject(string file_path)
        {
            List<Tool> persons = new List<Tool>();
            using (StreamReader r = new StreamReader(file_path))
            {
                string jData = r.ReadToEnd();
                persons = JsonConvert.DeserializeObject<List<Tool>>(jData);
            }

            return persons;
        }

        //FIND ITEM
        /// <summary>
        /// Find a record in the JSON file
        /// </summary>
        /// <param name="json_data">JSON data in string format</param>
        /// <param name="id">Record id</param>
        /// <returns>Returns string data</returns>
        public static string FindItem(string json_data, int id)
        {
            JArray json_data_arr = JArray.Parse(json_data);

            //var item_id = jo["report"]["Id"].ToString(); //Ex: if json data has name called: report

            //foreach (var company in json_data_arr.Where(obj => obj["ID"].Value<int>() == id))
            //{
            //    //company["companyname"] = !string.IsNullOrEmpty(companyName) ? companyName : "";
            //}

            //This will get the row data of the id
            string id_coln = "Id";
            var companyToDeleted = json_data_arr.FirstOrDefault(obj => obj[id_coln].Value<int>() == id);
            if (companyToDeleted != null)
            {
                string row_data = "Row: \n" + companyToDeleted;
                string row_id = "Row ID: " + companyToDeleted[id_coln].ToString();
                return "\nFOUND \n" + row_id + "\n\n" + row_data;
            }
            else
            {
                return "\nNOT FOUND";
            }

        }
        #endregion

        #region :Update
        //Update Method 2 (Prefer)
        public static bool UpdateRow(string file_path, int id, Tool p)
        {
            List<Tool> ls = getJsonDataObject(file_path);
            var itemindex = ls.FindIndex(y => y.m_id == id);
            var item = ls.ElementAt(itemindex);
            if (item != null)
            {
                ls.RemoveAt(itemindex); //Delete then insert again after update

                item.m_name = p.m_name;
                item.m_url = p.m_url;
                item.m_notes = p.m_notes;
                item.m_picture = p.m_picture;
                item.m_created = p.m_created;
                item.m_updated = p.m_updated;
                //item.FullName = p.FullName;

                ls.Insert(itemindex, item); //Insert updated row

                string update_json_data = JsonConvert.SerializeObject(ls, Formatting.Indented);
                WriteJsonData(file_path, update_json_data);

                return true;
            }
            else
            {
                return false;
            }

        }

        //Update Method 1 (Not in-use)
        public static bool UpdateRow(string json_data, int id, string json_file_path)
        {
            //string json = File.ReadAllText(json_file_path);
            //dynamic jsonObj = JsonConvert.DeserializeObject(json);
            //jsonObj["Bots"][0]["Password"] = "new password";
            //string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            //File.WriteAllText("settings.json", output);

            /*
            var itemIndex = listObject.FindIndex(x => x == SomeSpecialCondition());
            var item = listObject.ElementAt(itemIndex);
            listObject.RemoveAt(itemIndex);
            item.SomePropYouWantToChange = "yourNewValue";
            listObject.Insert(itemIndex, item);
            */

            List<Tool> ls = JsonConvert.DeserializeObject<List<Tool>>(json_data);

            var itemindex = ls.FindIndex(y => y.m_id == id);
            var item = ls.ElementAt(itemindex);
            if (item != null)
            {
                ls.RemoveAt(itemindex);

                item.m_name = "Test name";
                item.m_url = "Test url";

                string fn = item.m_name;
                string ln = item.m_url;

                ls.Insert(itemindex, item);

                string update_json_data = JsonConvert.SerializeObject(ls, Formatting.Indented);
                WriteJsonData(json_file_path, update_json_data);

                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region :Delete
        //Delete Row
        /// <summary>
        /// Delete record by id
        /// </summary>
        /// <param name="file_path">Full path to file location</param>
        /// <param name="id">Record id</param>
        /// <returns>Returns true if deleted</returns>
        public static bool DeleteRow(string file_path, int id)
        {
            List<Tool> ls = getJsonDataObject(file_path);
            var getRow = ls.FirstOrDefault(x => x.m_id == id);
            if (getRow != null)
            {
                ls.Remove(getRow);
                string update_json_data = JsonConvert.SerializeObject(ls, Formatting.Indented);
                WriteJsonData(file_path, update_json_data);
                return true;
            }
            else
            {
                return false;
            }

            //var jObject = JObject.Parse(file_path); //make an object of the original data file
            //JArray json_data_arr = JArray.Parse(file_path); //make an array of the original data file

            //var companyToDeleted = json_data_arr.FirstOrDefault(obj => obj["ID"].Value<int>() == id);
            //if (companyToDeleted != null)
            //{
            //    json_data_arr.Remove(companyToDeleted);
            //    string output = JsonConvert.SerializeObject(jObject, Formatting.Indented); //make updated file
            //    File.WriteAllText(file_path, output); //merge updated with the original file
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

        }
        #endregion

    }
}
