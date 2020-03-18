using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.ToolsObject;

namespace ConsoleAppUI
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //PersonLogic pLogic = new PersonLogic(Utilities.DataFolder, Utilities.PersonsJsonFile, Utilities.ErrorFolder);           
            //LoadPersons(pLogic);

            //Console.WriteLine("\n List after insert");
            //CreatePerson(pLogic);
            //LoadPersons(pLogic);

            //
            Init init = new Init(Utilities.DataFolder, Utilities.ErrorFolder);
            ToolLogic logic = new ToolLogic(Utilities.ToolJsonFileName);
            Console.WriteLine("\n Initial List of Records:");
            LoadRecords(logic);
            Console.WriteLine("\n Get a record:");
            ShowRecord(logic, 0);

            //Console.WriteLine("\n List after insert");
            //CreateRecord(logic); //Tested Good
            //UpdateRecord(logic, 2); //Tested Good
            //DeleteRecord(logic, 0); //Tested Good
            //LoadRecords(logic);

            Console.Write("\n\n Press any key to exit: ");
            Console.ReadKey();
        }

        #region :Process Tool
        static void LoadRecords(ToolLogic logic)
        {   
            List<Tool> ls = logic.Records;
            foreach(Tool t in ls)
            {
                Console.WriteLine("ID:{0} Name:{1} Created:{2} Updated:{3}", t.m_id, t.m_name,t.m_created,t.m_updated);
            }
        }

        static void ShowRecord(ToolLogic logic, int id)
        {
            var rec = logic.Records.FirstOrDefault(x => x.m_id == id).m_created;
            if(rec != null)
            {
                Console.WriteLine(" id:{0} Created:{1}",id, rec);
            }
            else
            {
                Console.WriteLine(" No data was returned!");
            }
        }

        static void CreateRecord(ToolLogic logic)
        {
            Tool t = new Tool()
            {
                m_name = "Johan",
                m_url = "http://ayitech.com",
                m_notes = "This is a test...",
                m_picture = @"C:\Users\charl\Pictures\resources-1400x642.jpg",
                m_created = DateTime.Now.ToString(),
                m_updated = DateTime.Now.ToString()
            };

            logic.CreateRecord(t);
        }

        static void UpdateRecord(ToolLogic logic, int id)
        {
            //get created date by id
            var rec = logic.Records.FirstOrDefault(x => x.m_id == id).m_created;
            string created = DateTime.Now.ToString();
            if(rec != null)
            {
                created = rec;
            }

            Tool t = new Tool()
            {
                m_id = id,
                m_name = "Sample Name",
                m_url = "http://ayitech.com",
                m_notes = "This is a test...",
                m_picture = @"C:\Users\charl\Pictures\resources-1400x642.jpg",
                m_created = created,
                m_updated = DateTime.Now.ToString()
            };
            logic.UpdateRecord(t, id.ToString());
        }

        static void DeleteRecord(ToolLogic logic, int id)
        {
            logic.DeleteRecord(id.ToString());
        }
        #endregion

    }
}
