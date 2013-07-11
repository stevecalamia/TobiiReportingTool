using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiReportingTool
{
    public class TsvLine : System.Dynamic.DynamicObject
    {
        string[] _lineContent;
        List<string> _headers;

        public TsvLine(string line, List<string> headers)
        {
            _lineContent = line.Split(new char[] { '\t' });
            _headers = headers;
        }

        public string GetValue(string key)
        {
            string val = "";
            int index = _headers.IndexOf(key);
            if (index >= 0 && index < _lineContent.Length)
                val = _lineContent[index];

            return val;
        }

        public int getKeyIndex(string key)
        {
            return _headers.IndexOf(key);
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = null;

            int index = _headers.IndexOf(binder.Name);
            if (index >= 0 && index < _lineContent.Length)
            {
                result = _lineContent[index];
                return true;
            }

            return false;
        }
    }
}
