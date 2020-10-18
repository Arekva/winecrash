using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            byte[] actualData = Compress(this.Raw);
            byte[] headerData = BitConverter.GetBytes(actualData.Length);

            byte[] data = new byte[sizeof(int) + actualData.Length];
            
            Array.Copy(headerData, data, sizeof(int));
            Array.Copy(actualData, 0, data, sizeof(int), actualData.Length);

            lock (SendLocker)
            {
                socket.Send(data);
            }
        }
        
        public static async Task<NetObject> ReceiveDataAsync(Socket client, ManualResetEvent resetEvent = null)
        {
            return await Task.Run(() => Receive(client, resetEvent)).ConfigureAwait(false);
        }

        public static NetObject Receive(Socket client, ManualResetEvent resetEvent = null)
        {
            byte[] sizeInfo = new byte[sizeof(int)];
            client.Receive(sizeInfo);
            int actualDataSize = BitConverter.ToInt32(sizeInfo, 0);

            byte[] data = new byte[actualDataSize];
            
            int totalread = 0, currentread = sizeof(int);
            while (totalread < actualDataSize && currentread > 0)
            {
                currentread = client.Receive(data,
                    totalread, //offset into the buffer
                    data.Length - totalread, //max amount to read
                    SocketFlags.None);
                totalread += currentread;
            }

            resetEvent?.Set();
            
            return NetObject.Receive(NetData<NetDummy>.Encoding.GetString(NetData<NetDummy>.Decompress(data)), client);
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
