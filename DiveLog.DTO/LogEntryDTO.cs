using DiveLog.DTO.Types;
using System;
using System.Collections.Generic;

namespace DiveLog.DTO
{
    [Serializable]
    public class LogEntryDTO
    {
        public string ExternalId { get; set; }

        public DateTime DiveDate { get; set; }

        public DiveType DiveType { get; set; }

        public List<DataPointDTO> DataPoints { get; set; }

        public DiveOutcome Outcome { get; set; }

        public decimal MaxDepth { get; set; }

        public TimeSpan DiveLength { get; set; }

        public decimal FractionO2 { get; set; }

        public decimal FractionHe { get; set; }

		public int? AverageBottomDepth { get; set; }

		public int? BottomTime { get; set; }
	}
}
