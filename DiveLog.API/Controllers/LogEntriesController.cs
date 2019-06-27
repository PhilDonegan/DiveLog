using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DiveLog.API.Helpers;
using DiveLog.DAL;
using DiveLog.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiveLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogEntriesController : ControllerBase
    {
        private readonly DiveLogContext context;

        public LogEntriesController(DiveLogContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogEntry>>> Get()
        {
            var results = await context.LogEntries.ToListAsync();
            if (results == null)
            {
                return NotFound();
            }

            return results;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogEntry>> Get(long id)
        {
            var result = await context.LogEntries.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] LogEntry value)
        {
            var hash = HashGenerator.GenerateKey(value.DataPoints);
            if (context.LogEntries.Any(x => x.HashCode.Equals(hash)))
            {
                throw new InvalidOperationException("Log entry already exists.");
            }
            
            value.HashCode = hash;
            context.LogEntries.Add(value);
            context.SaveChanges();
        }

        [HttpPost]
        public void PostList([FromBody] IEnumerable<LogEntry> logEntries)
        { }

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
