using System.Runtime.Serialization;

namespace DiveLog.Web.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        [DataMember(Name = "x")]
        public double X { get; private set; }

        [DataMember(Name = "y")]
        public double Y { get; private set; }
    }
}
