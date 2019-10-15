using DiveLog.API.Helpers;
using DiveLog.DAL;
using DiveLog.DAL.Models;
using Microsoft.EntityFrameworkCore;
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
		private readonly IDiveLogStatHelper _diveLogStatHelper;

		public BackgroundJobs(
			DiveLogContext context,
			IDiveLogStatHelper diveLogStatHelper)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_diveLogStatHelper = diveLogStatHelper ?? throw new ArgumentNullException(nameof(diveLogStatHelper));
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

		public void DeriveDiveLogStatisics()
		{
			var matchingLogs = _context.LogEntries.Include(x => x.DataPoints).Where(x => !x.BottomTime.HasValue).ToList();
			matchingLogs.ForEach(x => _context.Entry(x).Collection(y => y.DataPoints).Query().OrderBy(z => z.Time).Load());
			if (matchingLogs.Any())
			{
				foreach (var log in matchingLogs)
				{
					try
					{
						var result = _diveLogStatHelper.CalculateBottomTime(log);
						log.AverageBottomDepth = result.Item1;
						log.BottomTime = result.Item2;
						_context.SaveChanges();
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex.Message, ex.GetBaseException().Message);
					}
				}
			}
		}
	}
}
