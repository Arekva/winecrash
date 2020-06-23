using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


using FKeys = System.Windows.Forms.Keys;
using WKeys = Winecrash.Engine.Keys;

namespace Winecrash.Engine
{
    public class Input : Module
    {
        internal static Input Instance { get; private set; }
        private static IInputWrapper InputWrapper { get; set; }

        private static int KeysAmount = Enum.GetValues(typeof(WKeys)).Length;
        private static Dictionary<WKeys, KeyStates> RegisteredKeyStates = new Dictionary<WKeys, KeyStates>(KeysAmount);

        public override bool Undeletable { get; internal set; } = true;

        protected internal override void Creation()
        {
            if(Instance)
            {
                Debug.Log("An Input instance is already existing !");
                this.ForcedDelete();
                return;
            }
            Instance = this;

            if (!CreateWrapper())
            {
                throw new Exception("System is not supported by the input system.");
            }

            CreateKeys();
        }


        protected internal override void Update()
        {
            UpdateKeysStates();
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
                        if(wrapper.CorrespondingOS == WEngine.OS)
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
            foreach(WKeys key in (WKeys[])Enum.GetValues(typeof(WKeys)))
            {
                RegisteredKeyStates.Add(key, KeyStates.Released);
            }
        }

        private static void UpdateKeysStates()
        {
            foreach (WKeys key in (WKeys[])Enum.GetValues(typeof(WKeys)))
            {
                KeyStates previousState = GetKeyState(key, RegisteredKeyStates);
                KeyStates newState;

                if (InputWrapper.GetKey(key))
                {
                    if (previousState == KeyStates.Pressed) continue; // nothing new, continue;

                    else if (previousState == KeyStates.Pressing) // pressing => pressed
                    {
                        newState = KeyStates.Pressed;
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
                    }

                    else // releasing => released
                    {
                        newState = KeyStates.Released;
                    }
                }

                RegisteredKeyStates[key] = newState;
            }
        }

        private static KeyStates GetKeyState(WKeys key, Dictionary<WKeys, KeyStates> dictionary)
        {
            if (dictionary == null) return KeyStates.None;

            if(dictionary.TryGetValue(key, out KeyStates state))
            {
                return state;
            }

            throw new Exception("Key not existing in keys dictionary !");
        }

        public static bool IsPressed(WKeys key)
        {
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Pressed;
        }
        public static bool IsPressing(WKeys key)
        {
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Pressing;
        }
        public static bool IsReleased(WKeys key)
        {
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Released;
        }
        public static bool IsReleasing(WKeys key)
        {
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Releasing;
        }
    }
}
