using System;
using System.Collections.Generic;
using System.Text;

namespace DiveLog.DTO
{
	[Serializable]
	public class ComparisonMetricDTO
	{
		public ComparisonMetricDTO(int depth, int time, int total)
		{
			Depth = depth;
			Time = time;
			Total = total;
		}

		public int Depth { get; }

		public int Time { get; }

		public int Total { get; }
	}
}
