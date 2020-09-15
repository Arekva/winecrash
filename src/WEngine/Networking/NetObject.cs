using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using Newtonsoft.Json;

namespace WEngine.Networking
{
    /// <summary>
    /// The callback delegate used when a NetObject is sent or received.
    /// </summary>
    /// <param name="data">The actual data send / received.</param>
    /// <param name="dataType">The data type.</param>
    /// <param name="connection">The socket where the data comes / has been sent.</param>
    public delegate void NetObjectCallback(NetObject data, Type dataType, Socket connection);

    /// <summary>
    /// Basic definition of a network object able to send himself over a socket.
    /// </summary>
    public abstract class NetObject
    {
        /// <summary>
        /// Triggered when data is received.
        /// </summary>
        public static event NetObjectCallback OnReceive;

        /// <summary>
        /// Triggered when data is sent.
        /// </summary>
        public static event NetObjectCallback OnSend;

        /// <summary>
        /// All the generic constructor already built.
        /// </summary>
        private static Dictionary<Type, ConstructorInfo> _GenericConstructors = new Dictionary<Type, ConstructorInfo>(16);

        [JsonIgnore]
        public bool Deleted { get; private set; } = false;

        /// <summary>
        /// Deserialize an object from its raw net data and make received.
        /// </summary>
        /// <param name="rawDataJson">The just-recieved data.</param>
        /// <param name="socket">The socket where the data comes from.</param>
        /// <returns>The NetObject gaven by the json.</returns>
        internal static NetObject Receive(string rawDataJson, Socket socket)
        {
            NetData<NetObject> data = JsonConvert.DeserializeObject<NetData<NetObject>>(rawDataJson);
            NetObject obj = JsonConvert.DeserializeObject(data.Data, data.Type) as NetObject;

            OnReceive?.Invoke(obj, data.Type, socket);
            return obj;
        }

        /// <summary>
        /// Send a <see cref="NetObject"/>.
        /// </summary>
        /// <param name="netobj">The NetObject to send. Cannot be null.</param>
        /// <param name="socket">The client to send the data to.</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void Send(NetObject netobj, Socket socket)
        {
            if (!netobj) throw new NullReferenceException("<NetObject.cs:58> " + nameof(netobj) + " cannot be null.");

            Type type = netobj.GetType();

            if(!_GenericConstructors.TryGetValue(type, out ConstructorInfo ctor))
            {
                ctor = typeof(NetData<>).MakeGenericType(type).GetConstructor(new Type[] { type });
                _GenericConstructors.Add(type,ctor);
            }

            OnSend?.BeginInvoke(netobj, type, socket, null, null);
            ((ISendible)ctor.Invoke(new object[] { netobj })).Send(socket);
        }

        public virtual void Delete()
        {
            this.Deleted = true;
        }

        /// <summary>
        /// Get if an object is null.
        /// </summary>
        /// <param name="bobj">The object to check out.</param>
        public static implicit operator bool(NetObject bobj)
        {
            return !(bobj is null);
        }
    }
}
