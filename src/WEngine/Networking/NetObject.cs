﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        private static ConcurrentDictionary<Type, ConstructorInfo> _GenericConstructors = new ConcurrentDictionary<Type, ConstructorInfo>();

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

        internal static Task<NetObject> ReceiveAsync(string rawDataJson, Socket socket)
        {
            return Task.Run(() => Receive(rawDataJson, socket));
        }

        /// <summary>
        /// Send a <see cref="NetObject"/>.
        /// </summary>
        /// <param name="netobj">The NetObject to send. Cannot be null.</param>
        /// <param name="socket">The client to send the data to.</param>
        /// <exception cref="NullReferenceException"></exception>
        public void Send(Socket socket, bool deleteOnSend = true)
        {
            if (socket == null) return;

            Task.Run(() =>
            {
                Type type = this.GetType();

                if (!_GenericConstructors.TryGetValue(type, out ConstructorInfo ctor))
                {
                    ctor = typeof(NetData<>).MakeGenericType(type).GetConstructor(new [] { type });
                    _GenericConstructors.TryAdd(type, ctor);
                }

                OnSend?.BeginInvoke(this, type, socket, null, null);

                ((ISendible)ctor.Invoke(new object[] { this })).Send(socket);
                if (deleteOnSend) this.Delete();
            });
        }

        public void Send(Socket[] sockets, bool deleteOnSend = true)
        {
            if (sockets == null) return;
            Parallel.ForEach(sockets, socket => this.Send(socket, false));
            if (deleteOnSend) this.Delete();
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
