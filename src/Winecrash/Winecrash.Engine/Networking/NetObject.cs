using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Newtonsoft.Json;

using Winecrash.Engine;
using System.Reflection;

namespace Winecrash.Engine.Networking
{
    public delegate void NetObjectCallback(NetObject data);

    /// <summary>
    /// Basic definition of a network object able to send himself over a socket.
    /// </summary>
    public abstract class NetObject : BaseObject
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

        /// <summary>
        /// Deserialize an object from its raw net data.
        /// </summary>
        /// <param name="rawJson">The just-recieved data from the socket.</param>
        /// <returns>The NetObject gaven by the json.</returns>
        internal static NetObject Deserialize(string rawJson)
        {
            NetData<NetObject> obj = JsonConvert.DeserializeObject<NetData<NetObject>>(rawJson);

            return JsonConvert.DeserializeObject(obj.Data, obj.Type) as NetObject;
        }

        internal static void Send(NetObject netobj, Socket socket)
        {
            if (!netobj) return;

            Type type = netobj.GetType();

            if(!_GenericConstructors.TryGetValue(type, out ConstructorInfo ctor))
            {
                ctor = typeof(NetData<>).MakeGenericType(type).GetConstructor(new Type[] { type });
                _GenericConstructors.Add(type,ctor);
            }

            ((ISendable)ctor.Invoke(new object[] { netobj })).Send(socket);
        }
    }
}
