using System;

namespace DiveLog.DTO
{
    [Serializable]
    public class DataPointDTO
    {
        public long LogEntryId { get; set; }

        public int Time { get; set; }

        public decimal Depth { get; set; }

        public decimal AveragePPO2 { get; set; }

        public short WaterTemp { get; set; }
    }
}
