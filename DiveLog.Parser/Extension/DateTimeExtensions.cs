using System;

namespace DiveLog.Parser.Extension
{
    public static class DateTimeExtensions
    {
        public static readonly double JulianAera = 2415018.5;
        public static readonly double Jd200Base = 2451545.0;

        public static double ToJulianDate(this DateTime dateTime)
        {
            return dateTime.ToOADate() + JulianAera;
        }

        public static double ToJd2000(this DateTime dateTime)
        {
            return dateTime.ToOADate() + JulianAera - Jd200Base;
        }

        public static DateTime FromJulianDate(double julianDate)
        {
            return DateTime.FromOADate(julianDate - JulianAera);
        }

        public static DateTime FromJd2000(double jd2000)
        {
            return DateTime.FromOADate(jd2000 + Jd200Base - JulianAera);
        }

        public static readonly DateTime UnixAera = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double ToUnixTime(this DateTime dateTime)
        {
            return dateTime.Subtract(UnixAera).TotalSeconds;
        }

        public static DateTime FromUnixTime(double unixTime)
        {
            return UnixAera.AddSeconds(unixTime);
        }
    }
}
