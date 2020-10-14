using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;

using Newtonsoft.Json;

namespace WEngine.Networking
{
    /// <summary>
    /// Network data exchange structure.
    /// </summary>
    /// <typeparam name="T">NetObject type to exchange</typeparam>
    [Serializable]
    internal struct NetData<T> : ISendible where T : NetObject
    {
        public static Encoding Encoding { get; set; } = Encoding.Unicode;

        private static object SendLocker = new object();
        
        /// <summary>
        /// Serialization type variable.
        /// </summary>
        [JsonIgnore]
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

        /// <summary>
        /// Serialization data variable.
        /// </summary>
        [JsonIgnore]
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

        [JsonIgnore]
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

        [JsonIgnore]
        /// <summary>
        /// The raw data of this structure, ready to be sent.
        /// </summary>
        public byte[] Raw
        {
            get
            {
                return Encoding.GetBytes(JsonConvert.SerializeObject(this));
            }
        }

        [JsonConstructor]
        public NetData(Type type, string data)
        {
            this._Type = type;
            this._Data = data;
        }

        /// <summary>
        /// Create a new NetData from a NetObject
        /// </summary>
        /// <param name="data"></param>
        public NetData(T data) : this()
        {
            this._Type = typeof(T);
            this._Data = JsonConvert.SerializeObject(data, Formatting.None);
        }

        /// <summary>
        /// Send the data over the interwebs
        /// </summary>
        /// <param name="socket">The socket to send data through</param>
        public void Send(Socket socket)
        {
            byte[] data = Compress(this.Raw);
            byte[] sizeData = BitConverter.GetBytes(data.Length);

            /*byte[] sendibleData = new byte[data.Length + sizeData.Length];
            Array.Copy(sizeData, sendibleData, data.Length);
            Array.Copy(data, 0, sendibleData, data.Length, sendibleData.Length);


            using (SocketAsyncEventArgs dataE = new SocketAsyncEventArgs())
            {
                dataE.SetBuffer(data, 0, data.Length);
                socket.SendAsync(dataE);
            }*/

            lock (SendLocker)
            {
                socket.Send(sizeData);
                socket.Send(data);
                /*using (SocketAsyncEventArgs sizeDataE = new SocketAsyncEventArgs())
                {
                    sizeDataE.SetBuffer(sizeData, 0, sizeData.Length);
                    socket.SendAsync(sizeDataE);
                }
                using (SocketAsyncEventArgs byteDataE = new SocketAsyncEventArgs())
                {
                    byteDataE.SetBuffer(data, 0, data.Length);
                    socket.SendAsync(byteDataE);
                }*/
            }
        }
        
        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
