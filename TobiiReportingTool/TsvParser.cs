using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiReportingTool
{
    public class TsvParser : IEnumerable
    {
        List<string> _headers;
        string[] _lines;
        bool _header;

        public TsvParser(string tsvContent, bool hasHeader = true)
        {
            _header = hasHeader;
            _lines = tsvContent.Split('\n');
            if (_lines.Length > 0)
                _headers = _lines[0].Split(new char[] { '\t' }).ToList();
        }

        public TsvParser(string[] tsvContentLines, bool hasHeader = true)
        {
            _header = hasHeader;
            _lines = tsvContentLines;
            if (_lines.Length > 0)
                _headers = _lines[0].Split(new char[] { '\t' }).ToList();
        }

        public IEnumerator GetEnumerator()
        {
            bool header = _header;
            foreach (var line in _lines)
                if (header)
                    header = false;
                else
                    yield return new TsvLine(line, _headers);
        }
        public int GetKeyIndex(string key)
        {
            return _headers.IndexOf(key);
        }
    }
}
