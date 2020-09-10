using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.Networking
{
    /// <summary>
    /// Network data exchange structure.
    /// </summary>
    /// <typeparam name="T">NetObject type to exchange</typeparam>
    [Serializable]
    internal struct NetData<T> : ISendable where T : NetObject
    {
        private Type _Type;
        /// <summary>
        /// What type does the data represent.
        /// </summary>
        public Type Type
        {
            get
            {
                return _Type;
            }
        }

        private string _Data;
        /// <summary>
        /// The stored JSON data.
        /// </summary>
        public string Data
        {
            get
            {
                return _Data;
            }
        }

        /// <summary>
        /// Return the size in byte of the data.
        /// </summary>
        public int Size
        {
            get
            {
                return sizeof(char) * _Data.Length;
            }
        }

        /// <summary>
        /// The raw data of this structure, ready to be sent.
        /// </summary>
        public byte[] Raw
        {
            get
            {
                return Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(this));
            }
        }

        /// <summary>
        /// Create a new NetData from a NetObject
        /// </summary>
        /// <param name="data"></param>
        public NetData(T data) : this()
        {
            this._Type = typeof(T);
            this._Data = JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Send the data over the interwebs
        /// </summary>
        /// <param name="socket">The socket to send data through</param>
        public void Send(Socket socket)
        {
            byte[] data = this.Raw;

            socket.Send(BitConverter.GetBytes(data.Length));
            socket.Send(data);
        }
    }
}
