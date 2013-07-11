using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiReportingTool
{
    public class Participant
    {
        public string Name { get; set; }
        public float ValidityPercentage { get; set; }
        public bool isValid { get; set; }
    }
}
