using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

namespace Winecrash.Engine
{
    public static class Input
    {
        private static IInputWrapper InputWrapper { get; set; }

        private static int KeysAmount = Enum.GetNames(typeof(Keys)).Length;
        private static Dictionary<Keys, KeyStates> RegisteredKeyStates = new Dictionary<Keys, KeyStates>(KeysAmount);
        private static bool[] KeyEditedForThisFrame = new bool[KeysAmount];

        private static Dictionary<Keys, KeyStates> InterTreadKeys;
        private static Dictionary<Keys, KeyStates> FrameKeys;
        private static bool LockThreadDictionaryEdit = false;

        internal static Thread KeyUpdateThread { get; private set; }
        internal static int UpdateWait { get; set; } = 5;

        [Initializer]
        private static void Initialize()
        {
            if(!CreateWrapper())
            {
                throw new Exception("System is not supported by the input system.");
            }

            CreateKeys();

            Updater.OnFrameStart += OnFrameStart;

            KeyUpdateThread = new Thread(StartUpdate)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            KeyUpdateThread.Start();
        }

        private static bool CreateWrapper()
        {
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    if (!type.IsInterface && typeof(IInputWrapper).IsAssignableFrom(type))
                    {
                        IInputWrapper wrapper = Activator.CreateInstance(type) as IInputWrapper;
                        if(wrapper.CorrespondingOS == Engine.OS)
                        {
                            InputWrapper = wrapper;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private static void CreateKeys()
        {
            for (int i = 0; i < KeysAmount; i++)
            {
                RegisteredKeyStates.Add((Keys)i, KeyStates.Released);
            }
        }
        private static void OnFrameStart()
        {
            if (InterTreadKeys == null) return;

            LockThreadDictionaryEdit = true;
            FrameKeys = new Dictionary<Keys, KeyStates>(InterTreadKeys);
            KeyEditedForThisFrame = new bool[KeysAmount];
            LockThreadDictionaryEdit = false;
        }

        private static void StartUpdate()
        {
            while(true)
            {
                UpdateKeysStates();

                Thread.Sleep(UpdateWait);
            }
        }

        private static void UpdateKeysStates()
        {
            for (int i = 0; i < KeysAmount; i++)
            {
                Keys key = (Keys)i;

                if (KeyEditedForThisFrame[i]) continue;

                KeyStates previousState = GetKeyState(key, RegisteredKeyStates);
                KeyStates newState;

                bool pressed = InputWrapper.GetKey(key);

                if(pressed)
                {
                    if (previousState == KeyStates.Pressed) continue; // nothing new, continue;

                    else if (previousState == KeyStates.Pressing) // pressing => pressed
                    {
                        newState = KeyStates.Pressed;

                        KeyEditedForThisFrame[i] = true;
                    }

                    else // released => pressing
                    {
                        newState = KeyStates.Pressing;
                    }
                }

                else
                {
                    if (previousState == KeyStates.Released) continue; // nothing new, continue;

                    else if (previousState == KeyStates.Pressing || previousState == KeyStates.Pressed) // pressing or pressed => releasing
                    {
                        newState = KeyStates.Releasing;

                        KeyEditedForThisFrame[i] = true;
                    }

                    else // releasing => released
                    {
                        newState = KeyStates.Released;

                        KeyEditedForThisFrame[i] = true;
                    }
                }

                RegisteredKeyStates[key] = newState;
            }

            if (!LockThreadDictionaryEdit)
                InterTreadKeys = new Dictionary<Keys, KeyStates>(RegisteredKeyStates);
        }

        private static KeyStates GetKeyState(Keys key, Dictionary<Keys, KeyStates> dictionary)
        {
            if (dictionary == null) return KeyStates.None;

            if(dictionary.TryGetValue(key, out KeyStates state))
            {
                return state;
            }

            throw new Exception("Key not existing in keys dictionary !");
        }

        public static bool IsPressed(Keys key)
        {
            return GetKeyState(key, FrameKeys) == KeyStates.Pressed;
        }
        public static bool IsPressing(Keys key)
        {
            return GetKeyState(key, FrameKeys) == KeyStates.Pressing;
        }
        public static bool IsReleased(Keys key)
        {
            return GetKeyState(key, FrameKeys) == KeyStates.Released;
        }
        public static bool IsReleasing(Keys key)
        {
            return GetKeyState(key, FrameKeys) == KeyStates.Releasing;
        }
    }
}
