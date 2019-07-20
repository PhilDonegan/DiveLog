using DiveLog.DTO.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.API.Controllers.Queries
{
    public class SearchDivesQuery
    {
        public DiveType? DiveType { get; set; }

        public decimal TargetDepth { get; set; }

        public short TargetDepthRange { get; set; }

        public TimeSpan TargetDiveLength { get; set; }

        public TimeSpan TargetDiveLengthRange { get; set; }
    }
}
