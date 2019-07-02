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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>()
                .HasIndex(l => l.HashCode).IsUnique(true);
        }

        public DbSet<LogEntry> LogEntries { get; set; }
    }
}
