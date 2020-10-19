using System.Collections.Generic;
using Newtonsoft.Json;
using WEngine;
using WEngine.Networking;

namespace WinecrashCore.Net
{
    public class NetInput : NetObject
    {
        public Dictionary<string, KeyStates> KeyStateses { get; set; }
        
        public Vector2D MouseDeltas { get; set; }

        [JsonConstructor]
        public NetInput(Dictionary<string, KeyStates> keys, Vector2D deltas)
        {
            this.KeyStateses = keys;
            this.MouseDeltas = deltas;
        }

        public void AddInput(string key, KeyStates state)
        {
            if (KeyStateses.ContainsKey(key))
            {
                KeyStateses[key] = state;
            }
            else
            {
                KeyStateses.Add(key, state);
            }
        }
    }
}