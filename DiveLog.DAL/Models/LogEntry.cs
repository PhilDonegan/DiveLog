using DiveLog.DAL.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiveLog.DAL.Models
{
    [Serializable]
    public class LogEntry
    {
        [Key]
        public long Id { get; set; }

        public DateTime DiveDate { get; set; }

        public DiveType DiveType { get; set; }

        public short SampleRate { get; set; }

        public List<DataPoint> DataPoints { get; set; }

        public DiveOutcome Outcome { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal MaxDepth { get; set; }

        public TimeSpan DiveLength { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal FractionO2 { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal FractionHe { get; set; }

        public string HashCode { get; set; }
    }
}
