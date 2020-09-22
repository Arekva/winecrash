using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace WEngine.Networking
{
    public delegate void PingBackDelegate(TimeSpan timeSinceSent);
    /// <summary>
    /// A netobject describing a ping.
    /// </summary>
    public class NetPing : NetObject
    {
        private static readonly Dictionary<ushort, DateTime> _AwaitedPings = new Dictionary<ushort, DateTime>(3);
        private static readonly object _PingsLockers = new object();
        private static readonly WRandom _IDRandom = new WRandom("pings".GetHashCode());

        public static event PingBackDelegate OnPingBack;

        public ushort ID { get; set; } = 0;


        static NetPing()
        {
            NetObject.OnReceive += ReceivePing;
        }

        public NetPing()
        {
            this.ID = (ushort)_IDRandom.Next(1, ushort.MaxValue);

            lock (_PingsLockers)
            {
                _AwaitedPings.Add(this.ID, DateTime.Now);
            }
        }
        /// <summary>
        /// JSON constructor; use <see cref="NetPing.NetPing()"/>
        /// </summary>
        /// <param name="id"></param>
        [JsonConstructor]
        public NetPing(ushort id)
        {
            this.ID = id;
        }

        private static void ReceivePing(NetObject data, Type dataType, Socket connection)
        {
            if (data is NetPing ping)
            {
                lock (_PingsLockers)
                { 
                    if (_AwaitedPings.TryGetValue(ping.ID, out DateTime sendTime))
                    {
                        OnPingBack?.Invoke(DateTime.Now - sendTime);
                        _AwaitedPings.Remove(ping.ID);  
                    }

                    else //if ping id isn't ours, send back.
                    {
                        NetObject.Send(data, connection);
                    }
                }
            }
        }
    }
}
