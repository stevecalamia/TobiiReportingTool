using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TobiiReportingTool
{
    public class SettingsItem
    {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public dynamic value;
    }
}
