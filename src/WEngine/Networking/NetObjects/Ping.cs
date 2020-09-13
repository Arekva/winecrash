using Newtonsoft.Json;
using System;
using System.Net.Sockets;

namespace WEngine.Networking
{
    /// <summary>
    /// A netobject describing a ping.
    /// </summary>
    public class Ping : NetObject
    {
        /// <summary>
        /// Serialized variable for <see cref="SendDate"/>.
        /// </summary>
        private DateTime _SendDate;
        /// <summary>
        /// The send date. Automatically set on creation.
        /// </summary>
        public DateTime SendDate
        {
            get
            {
                return _SendDate;
            }

            set
            {
                _SendDate = value;
            }
        }

        /// <summary>
        /// Serialized variable for <see cref="ReceiveDate"/>.
        /// </summary>
        public DateTime _ReceiveDate;
        /// <summary>
        /// The receive date. Automatically set on receiver <see cref="NetObject.OnReceive"/>.
        /// </summary>
        public DateTime ReceiveDate
        {
            get
            {
                return _ReceiveDate;
            }

            set
            {
                _ReceiveDate = value;
            }
        }

        /// <summary>
        /// Adds the <see cref="ReceivePing"/> method to the <see cref="NetObject.OnReceive"/>.
        /// </summary>
        static Ping()
        {
            NetObject.OnReceive += ReceivePing;
        }

        /// <summary>
        /// Create a ping from already existing informations. This is mostly used by <see cref="Newtonsoft.Json"/>.
        /// </summary>
        /// <param name="sendDate">The sender send date.</param>
        /// <param name="receiveDate">The receiver receive date.</param>
        [JsonConstructor]
        public Ping(DateTime sendDate, DateTime receiveDate)
        {
            this.SendDate = sendDate;
            this.ReceiveDate = receiveDate;
        }

        /// <summary>
        /// Create a ping. Sets <see cref="SendDate"/> as <see cref="DateTime.Now"/> on instantiation.
        /// </summary>
        public Ping()
        {
            SendDate = DateTime.UtcNow;
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
            if (data is Ping ping && ping._ReceiveDate == default) ping.ReceiveDate = DateTime.UtcNow;
        }
    }
}
