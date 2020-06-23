using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace Winecrash.Engine
{
    public class Input : Module
    {
        internal static Input Instance { get; private set; }
        private static IInputWrapper InputWrapper { get; set; }

        private static int KeysAmount = Enum.GetNames(typeof(Keys)).Length;
        private static Dictionary<Keys, KeyStates> RegisteredKeyStates = new Dictionary<Keys, KeyStates>(KeysAmount);

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
            //OnFrameStart();
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
            for (int i = 0; i < KeysAmount; i++)
            {
                RegisteredKeyStates.Add((Keys)i, KeyStates.Released);
            }
        }

        private static void UpdateKeysStates()
        {
            for (int i = 0; i < KeysAmount; i++)
            {
                Keys key = (Keys)i;

                KeyStates previousState = GetKeyState(key, RegisteredKeyStates);
                KeyStates newState;

                bool pressed = InputWrapper.GetKey(key);

                if(pressed)
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
            return GetKeyState(key, RegisteredKeyStates) == KeyStates.Pressed;
        }
        public static bool IsPressing(Keys key)
        {
            return GetKeyState(key, RegisteredKeyStates) == KeyStates.Pressing;
        }
        public static bool IsReleased(Keys key)
        {
            return GetKeyState(key, RegisteredKeyStates) == KeyStates.Released;
        }
        public static bool IsReleasing(Keys key)
        {
            return GetKeyState(key, RegisteredKeyStates) == KeyStates.Releasing;
        }
    }
}
