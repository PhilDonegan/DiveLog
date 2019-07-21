using DiveLog.DTO.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.GUI.Models
{
    public class LogSearcherModel
    {
        public DiveType? DiveType { get; set; }

        public decimal TargetDepth { get; set; }

        public short TargetDepthRange { get; set; }

        public short TargetDiveTime { get; set; }

        public short TargetDiveTimeRange { get; set; }
    }
}
