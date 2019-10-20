using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.API.Helpers
{
	public interface IDiveLogStatHelper
	{
		Tuple<int, int> CalculateBottomTime(DAL.Models.LogEntry logEntry);
	}
}
