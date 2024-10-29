using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    public class Report
    {
        public List<string> Header { get; set; }
        public List<List<string>> Lines { get; set; }

        public IList<string> GetLines()
        {
            var lines = new List<string>() { string.Join(',', Header)};
            lines.AddRange(Lines.Select(l => string.Join(',', l)));
            return lines;
        }

        public string GetString()
        {
            return string.Join("\r\n", GetLines());
        }
    }
}
