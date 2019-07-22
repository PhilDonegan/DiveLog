using System.Runtime.Serialization;

namespace DiveLog.GUI.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(double x, double y, double cns = 0)
        {
            this.X = x;
            this.Y = y;
            this.CNS = cns;
        }

        [DataMember(Name = "x")]
        public double X { get; private set; }

        [DataMember(Name = "y")]
        public double Y { get; private set; }

        public double CNS { get; private set; }
    }
}
