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

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<List<LogEntryDTO>>> Get()
        {
            var results = await _context.LogEntries.Include(x => x.DataPoints).ToListAsync();
            if (results == null)
            {
                return NotFound();
            }

            var dtos = _mapper.Map<List<LogEntry>, List<LogEntryDTO>>(results);
            return dtos;
        }

        [Route("[action]/{query}")]
        [HttpGet]
        public async Task<ActionResult<List<LogEntryDTO>>> SearchDives([FromQuery]SearchDivesQuery query)
        {
            _ = query ?? throw new ArgumentNullException(nameof(query));

            var clause = _context.LogEntries.Include(x => x.DataPoints).AsQueryable();

            var lowDepth = query.TargetDepth - query.TargetDepthRange > 0 ? query.TargetDepth - query.TargetDepthRange : 0;
            var highDepth = query.TargetDepth + query.TargetDepthRange;

            clause = clause.Where(x => x.MaxDepth >= lowDepth && x.MaxDepth < highDepth);

            var lowTime = query.TargetDiveLength - query.TargetDiveLengthRange > TimeSpan.Zero ? query.TargetDiveLength - query.TargetDiveLengthRange : TimeSpan.Zero;
            var highTime = query.TargetDiveLength + query.TargetDiveLengthRange;

            clause = clause.Where(x => x.DiveLength >= lowTime && x.DiveLength < highTime);

            if (query.DiveType.HasValue)
            {
                clause = clause.Where(x => x.DiveType.Equals(query.DiveType));
            }

            var results = await clause.ToListAsync();
            if (results == null)
            {
                return NotFound();
            }

            var dtos = _mapper.Map<List<LogEntry>, List<LogEntryDTO>>(results);
            return dtos;
        }

        ////[Route("{TargetDepth}/{TargetDepthRange}")]
        ////[HttpGet]
        ////public async Task<ActionResult<List<LogEntryDTO>>> SearchDives(decimal? TargetDepth, short? TargetDepthRange)
        ////{
        ////    var query = _context.LogEntries.AsQueryable();

        ////    if (TargetDepth.HasValue)
        ////    {
        ////        var lowDepth = TargetDepth.Value - TargetDepthRange > 0 ? TargetDepth.Value - TargetDepthRange : 0;
        ////        var highDepth = TargetDepth.Value + TargetDepthRange;

        ////        query = query.Where(x => x.MaxDepth >= lowDepth && x.MaxDepth < highDepth);
        ////    }

        ////    var results = await query.Include(x => x.DataPoints).ToListAsync();
        ////    if (results == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    var dtos = _mapper.Map<List<LogEntry>, List<LogEntryDTO>>(results);
        ////    return dtos;
        ////}

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogEntryDTO>> Get(long id)
        {
            var result = await _context.LogEntries.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return _mapper.Map<LogEntry, LogEntryDTO>(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LogEntryDTO), StatusCodes.Status201Created)]
        public void PostList([FromBody]List<LogEntryDTO> logEntries)
        {
            var entities = _mapper.Map<List<LogEntryDTO>, List<LogEntry>>(logEntries);

            // If start seeing weird shit with missing entities may be better to create new DbContext for this one post with change tracking disabled.
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            foreach (var logEntry in entities)
            {
                var hash = HashGenerator.GenerateKey(logEntry.DataPoints);
                if (_context.LogEntries.Any(x => x.HashCode.Equals(hash)))
                {
                    continue;
                }
                logEntry.HashCode = hash;
                _context.LogEntries.Add(logEntry);
            }

            _context.SaveChanges();

            //return CreatedAtAction("Get", new { LogEntries = _mapper.Map<List<LogEntry>, List<LogEntryDTO>>(entities) },);
        }
    }
}
