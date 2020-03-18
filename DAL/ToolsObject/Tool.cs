using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.ToolsObject
{
    public class Tool
    {
        public int m_id { get; set; }
        public string m_name { get; set; }
        public string m_url { get; set; }
        public string m_picture { get; set; }
        public string m_created { get; set; }
        public string m_updated { get; set; }
        public string m_notes { get; set; }
    }
}
