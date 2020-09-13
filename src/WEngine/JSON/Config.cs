using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WEngine
{
    public class Config
    {
        protected delegate void ConfigChangeDelegate();

        protected event ConfigChangeDelegate OnConfigChanged;

        private static readonly JsonSerializer serializer = new JsonSerializer()
        {
            Culture = System.Globalization.CultureInfo.InvariantCulture
        };

        private bool _LockWrite = false;

        [JsonIgnore]
        public string Path { get; }
        [JsonIgnore]
        public bool AutoSave { get; set; } = true;

        public Config(string path)
        {
            Path = path;

            SyncMode mode;

            OnConfigChanged += () =>
            {
                if (AutoSave && !_LockWrite)
                {
                    Synchronize(mode = SyncMode.Write);
                }
            };

            if (File.Exists(path))
            {
                mode = SyncMode.Read;
            }
            else
            {
                mode = SyncMode.Write;
            }

            Synchronize(mode);
        }

        protected void FireConfigChanged()
        {
            OnConfigChanged?.Invoke();
        }

        public void Save()
        {
            Synchronize(SyncMode.Write);
        }

        public void Reload()
        {
            Synchronize(SyncMode.Read);
        }

        protected void Synchronize(SyncMode mode = SyncMode.Read)
        {
            if (String.IsNullOrEmpty(Path)) return;

            switch (mode)
            {
                case SyncMode.Read:
                    SyncRead();
                    break;
                case SyncMode.Write:
                    SyncWrite();
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unvalid synchronisation mode for Config synchronisation.", new ArgumentException("Unvalid argument.", "mode"));
            }
        }
        private void SyncRead()
        {
            _LockWrite = true;
            using (StreamReader sr = new StreamReader(Path, Encoding.UTF8))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(sr)
                {
                    Culture = System.Globalization.CultureInfo.InvariantCulture
                })
                {
                    object config = serializer.Deserialize(jsonReader, this.GetType());

                    foreach (PropertyInfo property in this.GetType().GetProperties())
                    {
                        if (property.GetCustomAttributes(typeof(SynchronizeAttribute)).Any())
                        {
                            //Console.WriteLine(this.GetType() + " has " + property.Name + " marked as sync");
                            property.SetValue(this, property.GetValue(config));
                        }
                    }
                }
            }
            _LockWrite = false;
        }
        private void SyncWrite()
        {
            using (StreamWriter sw = new StreamWriter(Path, false, Encoding.UTF8))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw)
                {
                    Culture = System.Globalization.CultureInfo.InvariantCulture,
                    Formatting = Formatting.Indented
                })
                {
                    serializer.Serialize(jsonWriter, this, this.GetType());
                }
            }
        }
    }
}
