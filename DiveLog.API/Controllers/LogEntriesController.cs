using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DiveLog.API.Helpers;
using DiveLog.DAL;
using DiveLog.DAL.Models;
using DiveLog.DTO;
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
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogEntryDTO>>> Get()
        {
            var results = await _context.LogEntries.ToListAsync();
            if (results == null)
            {
                return NotFound();
            }

            return _mapper.Map<List<LogEntry>, List<LogEntryDTO>>(results);
        }

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

        // POST api/values
        ////[HttpPost]
        ////public void Post([FromBody] LogEntry value)
        ////{
        ////    var hash = HashGenerator.GenerateKey(value.DataPoints);
        ////    if (context.LogEntries.Any(x => x.HashCode.Equals(hash)))
        ////    {
        ////        throw new InvalidOperationException("Log entry already exists.");
        ////    }
            
        ////    value.HashCode = hash;
        ////    context.LogEntries.Add(value);
        ////    context.SaveChanges();
        ////}

        [HttpPost]
        public IActionResult PostList(List<LogEntryDTO> logEntries)
        {
            return NotFound();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
