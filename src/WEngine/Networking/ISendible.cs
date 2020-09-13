namespace WEngine.Networking
{
    /// <summary>
    /// Interface describing a sendable object. Used into <see cref="NetData{T}"/> in order to cast it into <see cref="NetObject.Send(NetObject, System.Net.Sockets.Socket)"/>.
    /// </summary>
    internal interface ISendible
    {
        /// <summary>
        /// Send this object into the interwebs.
        /// </summary>
        /// <param name="socket">The socket to send the data to.</param>
        public void Send(System.Net.Sockets.Socket socket);
    }
}
