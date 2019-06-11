using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiveLog.DAL.Models
{
    public class DataPoint
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("LogEntry")]
        public long LogEntryId { get; set; }

        public int Time { get; set; }

        [Column(TypeName = "decimal(4, 1)")]
        public decimal Depth { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        public decimal AveragePPO2 { get; set; }

        public short WaterTemp { get; set; }


    }
}
