using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.GUI.Models
{
    public class LogSearcherResultsModel
    {
        public LogSearcherResultsModel()
        {
            DiveProfiles = new List<DiveProfile>();
        }

        public List<DiveProfile> DiveProfiles { get; set; }
    }
}
