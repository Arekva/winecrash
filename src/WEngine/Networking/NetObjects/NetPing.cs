using Newtonsoft.Json;
using System;
using System.Net.Sockets;

namespace WEngine.Networking
{
    /// <summary>
    /// A netobject describing a ping.
    /// </summary>
    public class NetPing : NetObject
    {
        /// <summary>
        /// The send date. Automatically set on creation.
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// The receive date. Automatically set on receiver <see cref="NetObject.OnReceive"/>.
        /// </summary>
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// Adds the <see cref="ReceivePing"/> method to the <see cref="NetObject.OnReceive"/>.
        /// </summary>
        static NetPing()
        {
            NetObject.OnReceive += ReceivePing;
        }

        /// <summary>
        /// Create a ping from already existing informations. This is mostly used by <see cref="Newtonsoft.Json"/>.
        /// </summary>
        /// <param name="sendDate">The sender send date.</param>
        /// <param name="receiveDate">The receiver receive date.</param>
        [JsonConstructor]
        public NetPing(DateTime sendTime, DateTime receiveTime)
        {
            this.SendTime = sendTime;
            this.ReceiveTime = receiveTime;
        }

        /// <summary>
        /// Receives a ping. 
        /// </summary>
        /// <param name="data">The generic data. Might not be a ping.</param>
        /// <param name="dataType">The data type.</param>
        /// <param name="connection">The socket the data comes from.</param>
        private static void ReceivePing(NetObject data, Type dataType, Socket connection)
        {
            //if no receive date, it means we are the receiver. Set the receive date to now.
            if (data is NetPing ping && ping.ReceiveTime == default)
            {
                ping.ReceiveTime = DateTime.UtcNow;
            }
        }
    }
}
