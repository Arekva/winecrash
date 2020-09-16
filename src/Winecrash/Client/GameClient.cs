using System;
using WEngine;
using WEngine.Networking;

namespace Client
{
    public class GameClient : BaseClient
    {
        public GameClient(string hostname, int port = BaseClient.DefaultPort) : base(hostname, port)
        {
            this.OnServerDataReceived += (data) =>{
                if (data is NetPing ping) {
                    if (ping.ReceiveTime != default) { // ping was sent by us and got all the way back

                        TimeSpan upTime = ping.ReceiveTime - ping.SendTime;
                        TimeSpan downTime = DateTime.UtcNow - ping.ReceiveTime;

                        Debug.Log(
                            $"Ping to server: {(upTime + downTime).Milliseconds:F0} ms " + 
                            $"(up: {upTime.Milliseconds:F0)} ms / down: {downTime.Milliseconds:F0)} ms)");
                    }
                    else
                    {
                        ping.ReceiveTime = DateTime.UtcNow;
                        NetObject.Send(ping, this.Client.Client);
                    }
                }
            };
        }
    }
}
