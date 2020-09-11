using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Winecrash.Engine;

namespace Winecrash.Engine.Networking
{
    public delegate void ClientConnectDelegate(TcpClient client);
    public delegate void ClientDataDelegate(TcpClient client, NetObject data);
    public abstract class BaseServer : BaseObject
    {
        public event ClientConnectDelegate OnClientConnect;
        public event ClientDataDelegate OnClientDataReceived;

        public static List<BaseServer> Servers { get; private set; } = new List<BaseServer>();

        protected List<TcpClient> Clients { get; set; } = new List<TcpClient>();
        protected object ClientsLocker = new object();
        public TcpListener Server { get; private set; } = null;
        protected Thread TickThread { get; private set; } = null;

        protected List<NetObject> PendingData { get; private set; } = new List<NetObject>();
        protected object PendingDataLocker = new object();

        /// <summary>
        /// Is the server running.
        /// </summary>
        public bool Running { get; private set; } = false;

        /// <summary>
        /// The Tick-Per-Second (Hz) rate of the server; basically server sided FPS.
        /// </summary>
        public uint TPS { get; set; } = 60U;

        /// <summary>
        /// Defines how often the server will ping clients per second. (Hz)
        /// </summary>
        public uint PingRate { get; set; } = 1U;

        /// <summary>
        /// The listening address.
        /// </summary>
        public IPEndPoint ListenAddress { get; private set; }

        /// <summary>
        /// Default listening address: 0.0.0.0:27716
        /// </summary>
        public static IPEndPoint DefaultListenAddress { get; } = new IPEndPoint(0L, 27716);

        public BaseServer(string listenIp = "0.0.0.0", int port = 27716)
        {
            Servers.Add(this);
            OnClientConnect += BaseServer_OnClientConnect;
            ListenAddress = new IPEndPoint(IPAddress.Parse(listenIp), port);
        }

        private void BaseServer_OnClientConnect(TcpClient client)
        {
            Task.Run(async () =>
            {
                while (this.Running)
                {
                    NetObject obj = await ReceiveDataAsync(client.Client);

                    if (this.Running)
                    {
                        lock (PendingDataLocker)
                        {
                            PendingData.Add(obj);
                        }
                    }
                }
            });
        }

        public async virtual Task Run()
        {
            if(this.Running)
            {
                Debug.LogError("Unable to run server: already running.");
            }
            try
            {
                Server = new TcpListener(this.ListenAddress);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return;
            }
            TickThread = new Thread(TickLoop)
            {
                ApartmentState = ApartmentState.MTA,
                IsBackground = false,
                Priority = ThreadPriority.Highest,
                Name = "Server Main Tick Thread"
            };
            TickThread.Start();
        }

        public abstract void Tick();

        public void Stop()
        {
            DisconnectAllClients();
            Running = false;
        }

        private Stopwatch TickLoopTimer = new Stopwatch();
        protected void TickLoop()
        {
            Running = true;

            Task.Run(AcceptClientAsync);

            while (Running)
            {
                TickLoopTimer.Start();
                Tick();
                TickLoopTimer.Stop();

                double waitTime = (1.0D/(double)TPS) - TickLoopTimer.Elapsed.TotalSeconds;
                TickLoopTimer.Reset();
                if (waitTime > 0.0D)
                {
                    Thread.Sleep((int)(waitTime * 1000.0D));
                }
            }
        }

        protected async Task<NetObject> ReceiveDataAsync(Socket client)
        {
            NetObject netobj = null;

            await Task.Run(() =>
            {
                byte[] sizeInfo = new byte[sizeof(int)];

                int totalread = 0, currentread = 0;
                currentread = totalread = client.Receive(sizeInfo);

                while (totalread < sizeInfo.Length && currentread > 0)
                {
                    currentread = client.Receive(sizeInfo,
                        totalread, //offset into the buffer
                        sizeInfo.Length - totalread, //max amount to read
                        SocketFlags.None);

                    totalread += currentread;
                }

                int messageSize = 0;

                //could optionally call BitConverter.ToInt32(sizeinfo, 0);
                messageSize |= sizeInfo[0];
                messageSize |= (((int)sizeInfo[1]) << 8);
                messageSize |= (((int)sizeInfo[2]) << 16);
                messageSize |= (((int)sizeInfo[3]) << 24);

                byte[] data = new byte[messageSize];

                //read the first chunk of data
                totalread = 0;
                currentread = totalread = client.Receive(data,
                    totalread, //offset into the buffer
                    data.Length - totalread, //max amount to read
                    SocketFlags.None);
                //if we didn't get the entire message, read some more until we do
                while (totalread < messageSize && currentread > 0)
                {
                    currentread = client.Receive(data,
                        totalread, //offset into the buffer
                        data.Length - totalread, //max amount to read
                        SocketFlags.None);
                    totalread += currentread;
                }

                string rawdata = Encoding.Unicode.GetString(data);

                netobj = NetObject.Receive(rawdata, client);
            }).ConfigureAwait(false);

            return netobj;
        }

        protected async Task AcceptClientAsync()
        {
            while (Running)
            {
                TcpClient client = await Server.AcceptTcpClientAsync();

                if (Running)
                {
                    lock (ClientsLocker)
                    {
                        OnClientConnect?.BeginInvoke(client, null, null);
                        Clients.Add(client);
                    }
                }

                else
                {
                    client.Close();
                    client.Dispose();
                }
            }
        }

        public void DisconnectAllClients()
        {
            lock (ClientsLocker)
            {
                if (Clients != null)
                {
                    foreach (TcpClient client in this.Clients)
                    {
                        client.Close();
                        client.Dispose();
                    }

                    Clients = null;
                }
            }
        }

        public override void Delete()
        {
            Running = false;

            DisconnectAllClients();

            lock(PendingDataLocker)
            {
                if(PendingData != null)
                {
                    foreach(NetObject obj in PendingData)
                    {
                        obj.Delete();
                    }

                    PendingData = null;
                }
            }

            Server?.Stop();
            Server = null;
            ListenAddress = null;

            base.Delete();
        }
    }
}
