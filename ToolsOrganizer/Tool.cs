using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsOrganizer
{
    [Serializable]
    public class Tool
    {
        //public string m_id;
        //public string m_name;
        //public string m_url;
        //public string m_picture;
        //public string m_created;
        //public string m_updated;

        public string m_id { get; set; }
        public string m_name { get; set; }
        public string m_url { get; set; }
        public string m_picture { get; set; }
        public string m_created { get; set; }
        public string m_updated { get; set; }

        public Tool()
        {
            m_id = "";
            m_name = "";
            m_url = "";
            m_picture = "";
            m_created = "";
            m_updated = "";
        }

        public Tool(string id, string name, string url, string picture, string created, string updated)
        {
            m_id = id;
            m_name = name;
            m_url = url;
            m_picture = picture;
            m_created = created;
            m_updated = updated;
        }

        public Tool(Tool t)
        {
            m_id = t.m_id;
            m_name = t.m_name;
            m_url = t.m_url;
            m_picture = t.m_picture;
            m_created = t.m_created;
            m_updated = t.m_updated;
        }

    }
}
