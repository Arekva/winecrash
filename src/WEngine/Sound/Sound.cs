using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace WEngine
{
    public class Sound : BaseObject
    {
        private static List<Sound> _Sounds = new List<Sound>();
        
        public string Path { get; private set; }
        private AudioFileReader _AudioReader = null;

        private WaveOut _RawSound = null;
        

        public Sound(string path, string name = null)
        {
            this.Path = path;
            
            if (name == null)
            {
                Name = path.Split('/', '\\').Last().Split('.')[0];
            }
            else
            {
                Name = name;
            }

            try
            {
                //_AudioReader = new AudioFileReader(path);
                _RawSound = new WaveOut();
                //_RawSound.Init(_AudioReader);

                _Sounds.Add(this);
            }
            catch(Exception e)
            {
                Debug.LogError($"Unable to load sound from \"{path}\": {e.Message}");
                this.Delete();
            }
        }

        public void Play()
        {
            Task.Run(() =>
            {
                using (AudioFileReader reader = new AudioFileReader(Path))
                using (WaveOut player = new WaveOut())
                {
                    player.Init(reader);
                    player.Play();

                    while (player.PlaybackState == PlaybackState.Playing)
                    {
                        Task.Delay(1).Wait();
                    }
                }
            });
        }

        public override void Delete()
        {
            _Sounds.Remove(this);
            Path = null;
            _AudioReader?.Dispose();
            _AudioReader = null;
            _RawSound?.Dispose();
            _RawSound = null;
            
            base.Delete();
        }

        public static Sound Find(string name)
        {
            return _Sounds.FirstOrDefault(s => s.Name == name);
        }
    }
}