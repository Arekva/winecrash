using System.Collections.Generic;
using Newtonsoft.Json;
using WEngine;
using WEngine.Networking;

namespace WinecrashCore.Net
{
    public class NetInput : NetObject
    {
        public Dictionary<string, KeyStates> KeyStateses { get; set; }

        [JsonConstructor]
        public NetInput(Dictionary<string, KeyStates> keys)
        {
            this.KeyStateses = keys;
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