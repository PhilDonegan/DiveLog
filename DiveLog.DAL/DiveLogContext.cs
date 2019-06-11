using DiveLog.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DiveLog.DAL
{
    public class DiveLogContext : DbContext
    {
        public DiveLogContext(DbContextOptions<DiveLogContext> options)
            :base(options)
        {
        }

        public DbSet<LogEntry> LogEntries { get; set; }
    }
}
