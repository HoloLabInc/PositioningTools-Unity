using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NmeaParser;
using NmeaParser.Gnss;
using UnityEngine;

#if NET_4_6
using System.IO.Ports;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloLab.PositioningTools.Nmea
{
    public class NmeaDeviceService : ICardinalDirectionService, IGeographicLocationService
    {
        private bool isRunning;
        private GnssMonitor monitor;
        private NmeaDevice nmeaDevice;

        private NmeaDeviceInfo selectedDevice;
        private SynchronizationContext context;

        public event Action<CardinalDirection> OnDirectionUpdated;

        public event Action<GeographicLocation> OnLocationUpdated;

        public bool IsEnabled { get; }

        public bool DebugLogReceivedMessage { set; get; }
        public string NmeaFilePathForSimulation { set; get; }

        public NmeaDeviceService()
        {
        }

        public NmeaDeviceService(SynchronizationContext context)
        {
            this.context = context;
        }

        public async void StartService()
        {
            await StartServiceAsync();
        }

        public async Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            StopService();

            if (selectedDevice == null)
            {
                return (false, new InvalidOperationException("Device is not selected"));
            }

            nmeaDevice = await selectedDevice.CreateMethod();
            nmeaDevice.MessageReceived += NmeaDevice_MessageReceived;
            await nmeaDevice.OpenAsync();

            monitor = new GnssMonitor(nmeaDevice);
            monitor.SynchronizationContext = context;
            monitor.LocationChanged += GnssMonitor_LocationChanged;
            return (true, null);
        }

        public async void StopService()
        {
            await StopServiceAsync();
        }

        public async Task<(bool ok, Exception exception)> StopServiceAsync()
        {
            if (nmeaDevice != null)
            {
                try
                {
                    nmeaDevice.MessageReceived -= NmeaDevice_MessageReceived;
                    await nmeaDevice.CloseAsync();
                    nmeaDevice.Dispose();
                    nmeaDevice = null;
                }
                catch (Exception e)
                {
                    return (false, e);
                }
            }
            return (true, null);
        }

        public IReadOnlyList<NmeaDeviceInfo> GetDeviceList()
        {
            var deviceList = new List<NmeaDeviceInfo>();

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(NmeaFilePathForSimulation))
            {
                var simulatedDeviceInfo = new NmeaDeviceInfo(
                    "Simulated NMEA Device",
                    () =>
                    {
                        return Task.FromResult<NmeaDevice>(new NmeaFileDevice(NmeaFilePathForSimulation));
                    }
                );
                deviceList.Add(simulatedDeviceInfo);
            }
#endif

            var tcpDeviceInfo = new NmeaDeviceInfo(
                "TCP Listener",
                () =>
                {
                    return Task.FromResult<NmeaDevice>(new NmeaTcpListenerDevice());
                }
            );
            deviceList.Add(tcpDeviceInfo);

#if NET_4_6 && !WINDOWS_UWP
            var portNames = SerialPort.GetPortNames();
            foreach (var portName in portNames)
            {
                Debug.Log(portName);
                var deviceInfo = new NmeaDeviceInfo(
                    $"Serial Port ({portName}) @ 9600 baud",
                    () =>
                    {
                        var serialPort = new SerialPort
                        {
                            BaudRate = 9600,
                            DataBits = 8,
                            Parity = Parity.None
                        };
                        return Task.FromResult<NmeaDevice>(new SerialPortDevice(serialPort));
                    }
                );
                deviceList.Add(deviceInfo);
            }
#endif
            return deviceList;
        }

        public void SelectDevice(NmeaDeviceInfo deviceInfo)
        {
            selectedDevice = deviceInfo;
        }

        private void NmeaDevice_MessageReceived(object sender, NmeaMessageReceivedEventArgs e)
        {
            if (DebugLogReceivedMessage)
            {
                Debug.Log(e.Message);
            }
        }

        private void GnssMonitor_LocationChanged(object sender, EventArgs e)
        {
            if (context == null)
            {
                InvokeUpdateEvent();
            }
            else
            {
                context.Post(_ =>
                {
                    InvokeUpdateEvent();
                }, false);
            }
        }

        private void InvokeUpdateEvent()
        {
            var dateTimeOffset = DateTimeOffset.Now;

            var heading = monitor.Course;
            if (!double.IsNaN(heading))
            {
                var cardinalDirection = new CardinalDirection((float)heading, dateTimeOffset);

                OnDirectionUpdated?.Invoke(cardinalDirection);
            }

            var latitude = monitor.Latitude;
            var longitude = monitor.Longitude;
            var altitude = monitor.Altitude;

            if (!double.IsNaN(latitude) && !double.IsNaN(longitude) && !double.IsNaN(altitude))
            {
                var geographicLocation = new GeographicLocation(latitude, longitude, altitude, dateTimeOffset);
                OnLocationUpdated?.Invoke(geographicLocation);
            }
        }
    }
}