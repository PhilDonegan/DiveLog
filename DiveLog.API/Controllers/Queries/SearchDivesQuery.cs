using DiveLog.DTO.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.API.Controllers.Queries
{
    public class SearchDivesQuery
    {
        [FromQuery]
        public DiveType? DiveType { get; set; }

        [FromQuery]
        public decimal TargetDepth { get; set; }

        [FromQuery]
        public short TargetDepthRange { get; set; }

        [FromQuery]
        public short TargetDiveLength { get; set; }

        [FromQuery]
        public short TargetDiveLengthRange { get; set; }
    }
}
