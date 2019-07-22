using System.Collections.Generic;

namespace DiveLog.GUI.Models
{
    public class DiveProfile
    {
        public DiveProfile()
        {
            Datapoints = new List<DataPoint>();
        }

        public int Index { get; set; }

        public List<DataPoint> Datapoints { get; set; }
    }
}
