using DiveLog.API.Helpers;
using DiveLog.DAL;
using DiveLog.DAL.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.API.Controllers
{
	public class BackgroundJobs : IBackgroundJobs
	{
		private readonly DiveLogContext _context;

		public BackgroundJobs(DiveLogContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public void SaveLogs(List<LogEntry> logEntries)
		{
			_context.ChangeTracker.AutoDetectChangesEnabled = false;

			foreach (var logEntry in logEntries)
			{
				var hash = HashGenerator.GenerateKey(logEntry.DataPoints);
				if (_context.LogEntries.Any(x => x.HashCode.Equals(hash)))
				{
					continue;
				}
				logEntry.HashCode = hash;
				_context.LogEntries.Add(logEntry);
				_context.SaveChanges();
			}
		}
	}
}
