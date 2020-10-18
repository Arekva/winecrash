using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WEngine.Networking
{
    /// <summary>
    /// Delegate used when data is recieved from a server. Similar to <see cref="NetObject.OnReceive"/>
    /// </summary>
    /// <param name="client">The TCP server the data comes from.</param>
    /// <param name="data">Any NetObject data received.</param>
    public delegate void ServerDataDelegate(NetObject data);

    public delegate void ServerConnectDelegate(TcpClient client);

    public delegate void ServerDisconnectDelegate(string reason);
    

    public abstract class BaseClient : BaseObject
    {
        /// <summary>
        /// Triggered when data from a client is received.
        /// </summary>
        public event ServerDataDelegate OnServerDataReceived;
        public event ServerConnectDelegate OnConnected;
        public event ServerDisconnectDelegate OnDisconnected;
        /// <summary>
        /// All the received untreated data of the server.
        /// </summary>
        protected List<NetObject> PendingData { get; private set; } = new List<NetObject>(1024);
        /// <summary>
        /// The thread locker object for the <see cref="PendingData"/> list.
        /// </summary>
        protected object PendingDataLocker = new object();

        public TcpClient Client { get; private set; }

        public string Host { get; private set; } = "localhost";
        public int Port { get; private set; } = Networking.DefaultPort;
        
        private static uint PingRate = 1U;


        public bool Connected { get; set; } = false;

        private bool TCPConnected
        {
            get
            {
                return Client != null && Client.Connected;
            }
        }

        public Thread ClientThread { get; private set; } = null;

        public BaseClient()
        {
            Engine.OnStop += () => ClientThread?.Abort();
        }

        public void Connect(string host = "127.0.0.1", int port = 27716)
        {
            this.Host = host;
            this.Port = port;
            
            Client = new TcpClient();
            OnConnected?.Invoke(Client);

            Connected = true;
            Client.Connect(host, port);
            
            
            LoopListeningAsync();
            DisconnectionCheckLoopAsync();
        }

        public void SendObject<T>(T netobj) where T : NetObject
        {
            netobj.Send(Client.Client);
        }

        private ManualResetEvent LoopResetEvent = new ManualResetEvent(true);
        protected void LoopListeningAsync()
        {
            Task.Run(async () =>
            {
                while (this.Connected)
                {
                    LoopResetEvent.WaitOne();
                    if (!this.Connected)
                    {
                        this.Disconnect("#server_disconnection_timeout");
                    }
                    LoopResetEvent.Reset();

                    Task.Run(async () =>
                    {
                        try
                        {
                            NetObject obj = await NetData<NetDummy>.ReceiveDataAsync(this.Client.Client, LoopResetEvent)
                                .ConfigureAwait(true); //ReceiveDataAsync(this.Client.Client).Result;


                            if (this.Connected)
                            {
                                if (obj is NetKick kick)
                                {
                                    Disconnect(kick.Reason, true);
                                    return;
                                }

                                if (obj is NetDummy)
                                {

                                }
                                else
                                {
                                    Task.Run(() =>
                                    {
                                        OnServerDataReceived?.Invoke(obj);

                                        lock (PendingDataLocker) PendingData.Add(obj);
                                    });
                                }
                            }
                        }
                        catch (System.IO.InvalidDataException e)
                        {
                            
                        }
                        catch (AggregateException e)
                        {
                            foreach (Exception exc in e.InnerExceptions)
                            {
                                if (exc is SocketException se)
                                {
                                    Connected = false;
                                    LoopResetEvent.Set();
                                }
                            }

                            if (Connected)
                            {
                                Debug.LogError("Multiple errors while receiving data: " + e);
                                LoopResetEvent.Set();
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Error while receiving data: " + e);
                            LoopResetEvent.Set();
                        }
                    });
                }

                //Debug.Log("stopped checking data.");
                
                //Disconnect();
            });
        }
        protected async Task DisconnectionCheckLoopAsync()
        {
            while (this.TCPConnected)
            {
                int waitTime = (int)((1.0D / PingRate) * 1000.0D);
                
                await Task.Delay(waitTime);
                new NetPing().Send(this.Client.Client);
            }
            Disconnect("#server_disconnection_timeout");
        }

        private bool IgnoreFollowingDisconnection = false;
        public virtual void Disconnect(string reason, bool ignoreFollowing = false)
        {
            if (IgnoreFollowingDisconnection)
            {
                IgnoreFollowingDisconnection = ignoreFollowing;
                return;
            }

            IgnoreFollowingDisconnection = ignoreFollowing;

            Connected = false;
            
            this.OnDisconnected?.Invoke(reason);
            this.Client.Dispose();
            Client = null;
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

                LoopResetEvent.Set();
                
                try
                {
                    netobj = NetObject.Receive(NetData<NetDummy>.Encoding.GetString(NetData<NetDummy>.Decompress(data)), client);
                }
                catch (Exception e)
                {
                    Debug.LogError("Transmission error or decompressing error.");
                    netobj = new NetDummy();
                }

            }).ConfigureAwait(true);

            return netobj;
        }
    }
}
