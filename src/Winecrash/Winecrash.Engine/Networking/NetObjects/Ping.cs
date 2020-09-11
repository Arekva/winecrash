using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.Networking
{
    public class Ping : NetObject
    {
        private DateTime _SendDate;
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

        public DateTime _ReceiveDate;
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

        static Ping()
        {
            NetObject.OnReceive += ReceivePing;
        }

        [JsonConstructor]
        public Ping(DateTime sendDate, DateTime receiveDate)
        {
            this.SendDate = sendDate;
            this.ReceiveDate = receiveDate;
        }

        public Ping()
        {
            SendDate = DateTime.UtcNow;
        }

        private static void ReceivePing(NetObject data, Type dataType, Socket connection)
        {
            if (data is Ping ping)
            {
                //if no receive date, send back
                //(means that the other side of the connexion sent the ping)
                if (ping._ReceiveDate == default)
                {
                    ping.ReceiveDate = DateTime.UtcNow;
                    Send(ping, connection);
                }

                //it means that ping has done its way back from the receiver
                else
                {
                    //todo: move to sender treatment.
                }
            }
        }
    }
}
