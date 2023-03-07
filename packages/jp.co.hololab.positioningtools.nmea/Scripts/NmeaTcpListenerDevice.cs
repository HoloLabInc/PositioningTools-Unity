using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NmeaParser;
using UnityEngine;

namespace HoloLab.PositioningTools.Nmea
{
    public class NmeaTcpListenerDevice : NmeaDevice
    {
        private readonly object lockObject = new object();

        private TcpListener server;

        private TcpClient tcpClient;
        private TcpClient nextTcpClient;

        private NetworkStream stream;

        private CancellationTokenSource tokenSource;

        private int port;

        private const int DefaultPort = 8620;

        public NmeaTcpListenerDevice() : this(DefaultPort)
        {
        }

        public NmeaTcpListenerDevice(int port)
        {
            this.port = port;
        }

        protected override Task<Stream> OpenStreamAsync()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                tokenSource = new CancellationTokenSource();
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        var client = await server.AcceptTcpClientAsync();

                        if (tokenSource.IsCancellationRequested)
                        {
                            DisposeTcpClient(client);
                            break;
                        }

                        lock (lockObject)
                        {
                            if (nextTcpClient != null)
                            {
                                DisposeTcpClient(nextTcpClient);
                            }

                            nextTcpClient = client;
                        }
                    }
                });
            }
            catch (SocketException e)
            {
                Debug.LogWarning($"SocketException: {e}");
            }

            var dummyStream = new MemoryStream();
            return Task.FromResult<Stream>(dummyStream);
        }

        protected override Task CloseStreamAsync(Stream stream)
        {
            try
            {
                server.Stop();
                server = null;
            }
            catch (Exception) { }

            tokenSource.Cancel();
            tokenSource = null;

            DisposeTcpClient(tcpClient);
            tcpClient = null;

            DisposeTcpClient(nextTcpClient);
            nextTcpClient = null;

            return Task.FromResult(true);
        }

        private static void DisposeTcpClient(TcpClient client)
        {
            client?.Close();
            client?.Dispose();
        }

        protected override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            lock (lockObject)
            {
                if (nextTcpClient != null)
                {
                    DisposeTcpClient(tcpClient);
                    tcpClient = nextTcpClient;
                    nextTcpClient = null;

                    stream = tcpClient.GetStream();
                }
            }

            if (stream == null)
            {
                return Task.FromResult(0);
            }

            return stream.ReadAsync(buffer, 0, 1024, cancellationToken);
        }
    }
}