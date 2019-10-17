using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DiveLog.API.Controllers.Queries;
using DiveLog.API.Helpers;
using DiveLog.DAL;
using DiveLog.DAL.Models;
using DiveLog.DAL.Models.Types;
using DiveLog.DTO;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiveLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogEntriesController : ControllerBase
    {
        private readonly DiveLogContext _context;
        private readonly IMapper _mapper;

        public LogEntriesController(
            DiveLogContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<List<LogEntryDTO>>> SearchDives([FromQuery]SearchDivesQuery query)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            //var clause = _context.LogEntries.Include(x => x.DataPoints).AsQueryable();
			var clause = _context.LogEntries.AsQueryable();

			var lowDepth = query.TargetDepth - query.TargetDepthRange > 0 ? query.TargetDepth - query.TargetDepthRange : 0;
            var highDepth = query.TargetDepth + query.TargetDepthRange;

            clause = clause.Where(x => x.MaxDepth >= lowDepth && x.MaxDepth < highDepth);

            var lowTime = TimeSpan.FromMinutes(query.TargetDiveLength) - TimeSpan.FromMinutes(query.TargetDiveLengthRange) > TimeSpan.Zero ? TimeSpan.FromMinutes(query.TargetDiveLength) - TimeSpan.FromMinutes(query.TargetDiveLengthRange) : TimeSpan.Zero;
            var highTime = TimeSpan.FromMinutes(query.TargetDiveLength) + TimeSpan.FromMinutes(query.TargetDiveLengthRange);

            clause = clause.Where(x => x.DiveLength >= lowTime && x.DiveLength < highTime);

            if (query.DiveType.HasValue)
            {
                clause = clause.Where(x => (int)x.DiveType == (int)query.DiveType.Value);
            }

            var results = await clause.OrderByDescending(x => x.DiveDate).ToListAsync();
            if (results == null)
            {
                return NotFound();
            }

			results.ForEach(x => _context.Entry(x).Collection(y => y.DataPoints).Query().OrderBy(z => z.Time).Load());

            var dtos = _mapper.Map<List<LogEntry>, List<LogEntryDTO>>(results);
            return dtos;
        }

		[Route("[action]")]
		[HttpGet]
		public async Task<ActionResult<List<LogEntryDTO>>> CompareDives(int depth, int bottomTime, DTO.Types.DiveType diveType)
		{
			var clause = _context.LogEntries.Include(x => x.DataPoints).AsQueryable();
			clause = clause.Where(x => x.BottomTime.HasValue && x.BottomTime.Value == bottomTime);
			clause = clause.Where(x => x.AverageBottomDepth.HasValue && x.AverageBottomDepth.Value == depth);
			clause = clause.Where(x => (int)x.DiveType == (int)diveType);

			var results = await clause.ToListAsync();
			if (results == null)
			{
				return NotFound();
			}

			results.ForEach(x => _context.Entry(x).Collection(y => y.DataPoints).Query().OrderBy(z => z.Time).Load());

			var dtos = _mapper.Map<List<LogEntry>, List<LogEntryDTO>>(results);
			return dtos;
		}


        [HttpGet("{id}")]
        public async Task<ActionResult<LogEntryDTO>> Get(long id)
        {
			var result = await _context.LogEntries.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }

			_context.Entry(result).Collection(x => x.DataPoints).Query().OrderBy(y => y.Time).Load();

            return _mapper.Map<LogEntry, LogEntryDTO>(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LogEntryDTO), StatusCodes.Status201Created)]
        public void PostList([FromBody]List<LogEntryDTO> logEntries)
        {
            var entities = _mapper.Map<List<LogEntryDTO>, List<LogEntry>>(logEntries);

			BackgroundJob.Enqueue<IBackgroundJobs>(job => job.SaveLogs(entities));
        }
    }
}
