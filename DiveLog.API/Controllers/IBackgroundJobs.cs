using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.API.Controllers
{
	public interface IBackgroundJobs
	{
		void SaveLogs(List<DAL.Models.LogEntry> logEntries);
	}
}
