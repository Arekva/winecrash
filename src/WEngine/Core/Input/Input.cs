using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using WEngine.Core.Input;

namespace WEngine
{
    public sealed class Input : WEngine.Module
    {
        internal static Input Instance { get; private set; }
        private static IInputWrapper InputWrapper { get; set; }

        private static int KeysAmount = Enum.GetValues(typeof(Keys)).Length;
        private static Dictionary<Keys, KeyStates> RegisteredKeyStates = new Dictionary<Keys, KeyStates>(KeysAmount);

        public static ReadOnlyDictionary<Keys, KeyStates> KeyDictionary
        {
            get
            {
                return new ReadOnlyDictionary<Keys, KeyStates>(RegisteredKeyStates);
            }
        }

        public static bool CursorVisible
        {
            get
            {
                return Graphics.Window.CursorVisible;
            }

            set
            {
                Graphics.Window.CursorVisible = value;
            }
        }

        public static Vector2I MousePosition { get; internal set; }

        public static Vector2D MouseDelta { get; internal set; } = Vector2D.Zero;

        public static double MouseScrollDelta { get; private set; } = 0.0D;

        public static double MouseSensivity { get; set; } = 1.0D;

        public static CursorLockModes LockMode { get; set; } = CursorLockModes.Free;

        public override bool Undeletable { get; internal set; } = true;

        private static double _PreviousScroll = 0.0D;
        internal static void SetMouseScroll(double scroll)
        {
            MouseScrollDelta = scroll - _PreviousScroll;

            _PreviousScroll = scroll;
        }

        protected internal override void Creation()
        {
            if(Instance)
            {
                Debug.LogWarning("An Input instance is already existing !");
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

        public static double MouseRefreshTime { get; set; } = 1 / 60.0D;

        private double timeSinceMouseRefresh = 0.0D;
        private bool firstTime = true;
        private bool ignoreNextFocusFrame = true;
        protected internal override void Update()
        {
            timeSinceMouseRefresh += Time.DeltaTime;

            if (timeSinceMouseRefresh >= MouseRefreshTime)
            {
                timeSinceMouseRefresh = 0.0D;
                Vector2I pos = InputWrapper.GetMousePosition();

                if (Graphics.Window == null)
                {
                    MouseDelta = Vector2D.Zero;
                }

                else
                {
                    Vector2I s = Graphics.Window.SurfaceResolution;
                    Vector2I winpos = Graphics.Window.SurfacePosition;
                    Vector2I centre = new Vector2I((int)(winpos.X + s.X / 2f), (int)(winpos.Y + s.Y / 2f));

                    if (!Graphics.Window.Focused)
                    {
                        ignoreNextFocusFrame = true;
                    }

                    if (!firstTime)
                    {
                        MouseDelta = Graphics.Window.Focused && Input.LockMode == CursorLockModes.Lock ? (ignoreNextFocusFrame ? Vector2D.Zero : centre - (Vector2D)pos) : Vector2D.Zero;
                        if (Graphics.Window.Focused && ignoreNextFocusFrame) ignoreNextFocusFrame = false;

                        MousePosition = Graphics.Window.Focused && Input.LockMode == CursorLockModes.Free ? /*centre - pos*/Graphics.Window.ScreenToWindow(pos) : Vector2I.Zero;
                    }
                    if (Input.LockMode == CursorLockModes.Lock && Graphics.Window.Focused)
                    {
                        InputWrapper.SetMousePosition(centre);
                    }

                    if(firstTime)
                    {
                        InputWrapper.SetMousePosition(centre);
                    }

                    firstTime = false;
                }
            }
            
            if(Graphics.Window != null)
                UpdateKeysStates();
        }

        protected internal override void OnDelete()
        {
            if(Instance == this)
            {
                Instance = null;
            }
            InputWrapper = null;
            RegisteredKeyStates = null;
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
                        if(wrapper.CorrespondingOS == Engine.OS.Platform)
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
            foreach(Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                RegisteredKeyStates.Add(key, KeyStates.Released);
            }
        }

        private static void UpdateKeysStates()
        {
            foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                KeyStates previousState = GetKeyState(key, RegisteredKeyStates);
                KeyStates newState;
                
                if (Graphics.Window.Focused && InputWrapper.GetKey(key))
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

        public static Keys[] GetAllKeys(KeyStates states)
        {
            List<Keys> keys = new List<Keys>(RegisteredKeyStates.Count);

            foreach(KeyValuePair<Keys, KeyStates> keystate in RegisteredKeyStates)
            {
                if((keystate.Value & states) != 0)
                {
                    keys.Add(keystate.Key);
                }
            }

            return keys.ToArray();
        }

        public static KeysModifiers GetModifiers()
        {
            KeysModifiers m = KeysModifiers.None;

            KeyStates shiftState = GetKeyState(Keys.LeftShift, RegisteredKeyStates);
            if(shiftState == KeyStates.Released | shiftState == KeyStates.Releasing)
                shiftState = GetKeyState(Keys.RightShift, RegisteredKeyStates);
            if (shiftState == KeyStates.Pressed | shiftState == KeyStates.Pressing)
                m |= KeysModifiers.Shift;

            if (KeysMethods.CapsLocked())
            {
                if(m.HasFlag(KeysModifiers.Shift))
                {
                    m &= ~KeysModifiers.Shift;
                }
                else
                {
                    m |= KeysModifiers.Shift;
                }
            }


            KeyStates altState = GetKeyState(Keys.LeftAlt, RegisteredKeyStates);
            if (altState == KeyStates.Released | altState == KeyStates.Releasing)
                altState = GetKeyState(Keys.RightAlt, RegisteredKeyStates);
            if (altState == KeyStates.Pressed | altState == KeyStates.Pressing)
                m |= KeysModifiers.Alt;

            KeyStates ctrlState = GetKeyState(Keys.LeftControl, RegisteredKeyStates);
            if (ctrlState == KeyStates.Released | ctrlState == KeyStates.Releasing)
                ctrlState = GetKeyState(Keys.RightControl, RegisteredKeyStates);
            if (ctrlState == KeyStates.Pressed | ctrlState == KeyStates.Pressing)
                m |= KeysModifiers.Control;

            return m;
        }

        public static KeyStates GetKeyState(Keys key, IDictionary<Keys, KeyStates> dictionary)
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
            if (!Graphics.Window.Focused) return false;

            return GetKeyState((Keys)key, RegisteredKeyStates) == KeyStates.Pressed;
        }
        public static bool IsPressing(Keys key)
        {
            if (!Graphics.Window.Focused) return false;
            return GetKeyState((Keys)key, RegisteredKeyStates) == KeyStates.Pressing;
        }
        public static bool IsReleased(Keys key)
        {
            if (!Graphics.Window.Focused) return false;
            return GetKeyState((Keys)key, RegisteredKeyStates) == KeyStates.Released;
        }
        public static bool IsReleasing(Keys key)
        {
            if (!Graphics.Window.Focused) return false;
            return GetKeyState((Keys)key, RegisteredKeyStates) == KeyStates.Releasing;
        }
    }
}
