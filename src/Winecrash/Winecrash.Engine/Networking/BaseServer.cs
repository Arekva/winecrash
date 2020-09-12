using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WEngine.Networking
{
    /// <summary>
    /// Delegate used when a client connects to a server.
    /// </summary>
    /// <param name="client">The TCP client.</param>
    public delegate void ClientConnectDelegate(TcpClient client);
    /// <summary>
    /// Delegate used when data is recieved from a client. Similar to <see cref="NetObject.OnReceive"/>
    /// </summary>
    /// <param name="client">The TCP client the data comes from.</param>
    /// <param name="data">Any NetObject data received.</param>
    public delegate void ClientDataDelegate(TcpClient client, NetObject data);
    /// <summary>
    /// The base class for all servers.
    /// </summary>
    public abstract class BaseServer : BaseObject
    {
        /// <summary>
        /// Triggered when a client connects to the server.
        /// </summary>
        public event ClientConnectDelegate OnClientConnect;
        /// <summary>
        /// Triggered when data from a client is received.
        /// </summary>
        public event ClientDataDelegate OnClientDataReceived;

        /// <summary>
        /// All the servers running. Mostly used for the engine to shutdown.
        /// </summary>
        public static List<BaseServer> Servers { get; private set; } = new List<BaseServer>();

        /// <summary>
        /// The list containing all the connected clients.
        /// </summary>
        protected List<TcpClient> Clients { get; set; } = new List<TcpClient>();
        /// <summary>
        /// The thread locker object for the <see cref="Clients"/> list.
        /// </summary>
        protected object ClientsLocker = new object();
        /// <summary>
        /// This server's listener
        /// </summary>
        public TcpListener Server { get; private set; } = null;
        /// <summary>
        /// The main server thread that performs <see cref="Tick"/>.
        /// </summary>
        protected Thread TickThread { get; private set; } = null;

        /// <summary>
        /// All the received untreated data of the server.
        /// </summary>
        protected List<NetObject> PendingData { get; private set; } = new List<NetObject>();
        /// <summary>
        /// The thread locker object for the <see cref="PendingData"/> list.
        /// </summary>
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

        /// <summary>
        /// Create a basic server able to listen to incoming NetObjects from multiple clients.
        /// </summary>
        /// <param name="listenIp">The listening IP. Defaults to 0.0.0.0 (all).</param>
        /// <param name="port">The listening port. Defaults to 27716.</param>
        public BaseServer(string listenIp = "0.0.0.0", int port = 27716)
        {
            Servers.Add(this);
            OnClientConnect += LoopListening;
            ListenAddress = new IPEndPoint(IPAddress.Parse(listenIp), port);
        }

        /// <summary>
        /// Reads all incoming data from a client while <see cref="Running"/> is set to true.
        /// <br>Adds received data to <see cref="PendingData"/></br>
        /// </summary>
        /// <param name="client">The connected client to listen to.</param>
        protected void LoopListening(TcpClient client)
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

        /// <summary>
        /// Runs the server onto the <see cref="TickThread"/> Thread.
        /// </summary>
        public virtual void Run()
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

        /// <summary>
        /// Running once tick time. Set the tickrate with <see cref="TPS"/>.
        /// </summary>
        public abstract void Tick();
        /// <summary>
        /// Disconnects all clients and stops the server.
        /// </summary>
        public void Stop()
        {
            DisconnectAllClients();
            Running = false;
        }

        /// <summary>
        /// The tick time watcher. Used to compute the wait time between two ticks.
        /// </summary>
        private Stopwatch TickLoopTimer = new Stopwatch();

        /// <summary>
        /// The main tick loop while <see cref="Running"/> is set to true. Set the refresh rate with <see cref="TPS"/>.
        /// </summary>
        protected void TickLoop()
        {
            Running = true;

            Task.Run(AcceptClientsLoopAsync);

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

        /// <summary>
        /// Wait for received data asynchronously. Used by <see cref="LoopListening(TcpClient)"/>.
        /// </summary>
        /// <param name="client">The socket to listen to.</param>
        /// <returns>The received <see cref="NetObject"/></returns>
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

        /// <summary>
        /// Loops clients acceptation while <see cref="Running"/> is set to true.
        /// </summary>
        protected async Task AcceptClientsLoopAsync()
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

        /// <summary>
        /// Disconnects all TCP Clients within <see cref="Clients"/>.
        /// </summary>
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
