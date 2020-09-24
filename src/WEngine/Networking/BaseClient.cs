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
        protected List<NetObject> PendingData { get; private set; } = new List<NetObject>();
        /// <summary>
        /// The thread locker object for the <see cref="PendingData"/> list.
        /// </summary>
        protected object PendingDataLocker = new object();

        public TcpClient Client { get; private set; }

        public const int DefaultPort = 27716;

        public bool Connected
        {
            get
            {
                return Client != null && Client.Connected;
            }
        }

        public Thread ClientThread { get; private set; } = null;

        public BaseClient(string host = "127.0.0.1", int port = DefaultPort)
        {
            ClientThread = new Thread(() =>
            {
                Client = new TcpClient();
                OnConnected?.Invoke(Client);
                Connect(host, port);
            });
            ClientThread.Start();
            Engine.OnStop += () => ClientThread.Abort();
        }

        public void Connect(string host = "127.0.0.1", int port = DefaultPort)
        {
            Client.Connect(host, port);
            LoopListeningAsync();
        }

        public void SendObject<T>(T netobj) where T : NetObject
        {
            NetObject.Send(netobj, Client.Client);
        }

        protected void LoopListeningAsync()
        {
            Task.Run(async () =>
            {
                while (this.Connected)
                {
                    NetObject obj = await ReceiveDataAsync(this.Client.Client);

                    if (this.Connected)
                    {
                        if (obj is NetKick kick)
                        {
                            this.OnDisconnected(kick.Reason);
                            Disconnect();
                            return;
                        }
                        else
                        {
                            lock (PendingDataLocker)
                            {
                                OnServerDataReceived?.Invoke(obj);
                                PendingData.Add(obj);
                            }
                        }
                    }
                }

                Disconnect();
            });
        }

        public virtual void Disconnect()
        {
            Debug.Log("Disconnected");
            this.Client.Close();
            this.Client.Dispose();
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

                string rawdata = NetData<NetDummy>.Encoding.GetString(data);

                netobj = NetObject.Receive(rawdata, client);

            }).ConfigureAwait(false);

            return netobj;
        }
    }
}
