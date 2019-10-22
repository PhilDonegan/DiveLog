using DiveLog.DTO;
using DiveLog.DTO.Types;
using System;
using System.Collections.Generic;

namespace DiveLog.GUI.Models
{
	public class LogComparisonModel
	{
		public DiveType DiveType { get; set; }

		public int Depth { get; set; }

		public int Time { get; set; }

		public List<ComparisonMetricDTO> AvailableComparisons { get; set; }
	}
}
