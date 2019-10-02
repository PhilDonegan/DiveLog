using DiveLog.API.Helpers;
using DiveLog.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace DiveLog.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StatsController : Controller
	{
		private readonly DiveLogContext _context;
		private readonly IMemoryCache _cache;

		public StatsController(
			DiveLogContext context,
			IMemoryCache cache)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
		}

		[HttpGet]
		public async Task<int> TotalLogEntries() => await _cache.GetOrCreateAsync<int>("TotalLogs",
				async cacheEntry =>
				{
					cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(5);
					cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);

					var rows = 0;
					using (var dr = await _context.Database.ExecuteSqlQueryAsync(
						@"SELECT SUM(PART.rows) AS rows
					FROM sys.tables TBL
						INNER JOIN sys.partitions PART ON TBL.object_id = PART.object_id
						INNER JOIN sys.indexes IDX ON PART.object_id = IDX.object_id AND PART.index_id = IDX.index_id
					WHERE TBL.name = 'LogEntries'
						AND IDX.index_id < 2
					GROUP BY TBL.object_id, TBL.name"))
					{
						var reader = dr.DbDataReader;
						while (reader.Read())
						{
							rows = Convert.ToInt32(reader[0]);
						}

						return rows;
					}
				});
	}
}