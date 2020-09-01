using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;

using FKeys = System.Windows.Forms.Keys;
using WKeys = Winecrash.Engine.Keys;

namespace Winecrash.Engine
{
    public sealed class Input : Module
    {
        internal static Input Instance { get; private set; }
        private static IInputWrapper InputWrapper { get; set; }

        private static int KeysAmount = Enum.GetValues(typeof(WKeys)).Length;
        private static Dictionary<WKeys, KeyStates> RegisteredKeyStates = new Dictionary<WKeys, KeyStates>(KeysAmount);
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

                    if (!firstTime)
                    {
                        MouseDelta = Graphics.Window.Focused && Input.LockMode == CursorLockModes.Lock ? centre - (Vector2D)pos : Vector2D.Zero;

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
            if (!Graphics.Window.Focused) return false;

            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Pressed;
        }
        public static bool IsPressing(WKeys key)
        {
            if (!Graphics.Window.Focused) return false;
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Pressing;
        }
        public static bool IsReleased(WKeys key)
        {
            if (!Graphics.Window.Focused) return false;
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Released;
        }
        public static bool IsReleasing(WKeys key)
        {
            if (!Graphics.Window.Focused) return false;
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Releasing;
        }
    }
}
