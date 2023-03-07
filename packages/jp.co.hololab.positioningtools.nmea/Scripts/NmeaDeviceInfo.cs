using System;
using System.Threading.Tasks;
using NmeaParser;

namespace HoloLab.PositioningTools.Nmea
{
    public class NmeaDeviceInfo
    {
        public Func<Task<NmeaDevice>> CreateMethod { get; }
        public string DisplayName { get; }
        public override string ToString() => DisplayName;

        public NmeaDeviceInfo(string displayName, Func<Task<NmeaDevice>> createMethod)
        {
            DisplayName = displayName;
            CreateMethod = createMethod;
        }
    }
}