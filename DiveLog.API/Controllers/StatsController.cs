using DiveLog.API.Helpers;
using DiveLog.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DiveLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : Controller
    {
        private readonly DiveLogContext _context;

        public StatsController(DiveLogContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<int> TotalLogEntries()
        {
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
        }
    }
}