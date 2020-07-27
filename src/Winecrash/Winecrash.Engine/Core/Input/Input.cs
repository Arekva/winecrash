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


        private static bool _UpdateCursorVisible = false;
        private static bool _CursorVisible = true;
        public static bool CursorVisible
        {
            get
            {
                return _CursorVisible;
            }

            set
            {
                _CursorVisible = value;
                _UpdateCursorVisible = true;
            }
        }

        private static bool _UpdateWindowMode = false;
        private static WindowState _WindowMode = WindowState.Normal;
        public static WindowState WindowMode
        {
            get
            {
                _WindowMode = (WindowState)Viewport.Instance.WindowState;
                return _WindowMode;
            }

            set
            {
                _WindowMode = value;
                _UpdateWindowMode = true;
            }
        }


        public static Vector2I MousePosition { get; internal set; }

        internal static bool Focused
        {
            get
            {
                return Viewport.Instance != null ? Viewport.Instance.Focused : false;
            }
        }

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


        protected internal override void Update()
        {
            Vector2I pos = InputWrapper.GetMousePosition();
            

            if (Viewport.Instance == null)
            {
                MouseDelta = Vector2D.Zero;
            }

            else
            {
                Size s = Viewport.Instance.Size;
                Vector2I centre = new Vector2I((int)(Viewport.Instance.X + s.Width / 2f), (int)(Viewport.Instance.Y + s.Height / 2f));

                MouseDelta = Viewport.Instance.Focused && Input.LockMode == CursorLockModes.Lock ? centre - (Vector2D)pos : Vector2D.Zero;

                MousePosition = Viewport.Instance.Focused && Input.LockMode == CursorLockModes.Free ? centre - pos : Vector2I.Zero;


                if (Input.LockMode == CursorLockModes.Lock && Focused)
                {
                    InputWrapper.SetMousePosition(centre);
                }
            }
            
            UpdateKeysStates();

            
        }

        protected internal override void LateUpdate()
        {
            Viewport.DoOnceRender += () => Viewport.Instance.CursorVisible = Focused ? CursorVisible : true;

            if(_UpdateWindowMode)
            {
                _UpdateWindowMode = false;
                Viewport.DoOnceRender += () => Viewport.Instance.WindowState = (OpenTK.WindowState)_WindowMode;
            }
            
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
            if (!Focused) return false;

            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Pressed;
        }
        public static bool IsPressing(WKeys key)
        {
            if (!Focused) return false;
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Pressing;
        }
        public static bool IsReleased(WKeys key)
        {
            if (!Focused) return false;
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Released;
        }
        public static bool IsReleasing(WKeys key)
        {
            if (!Focused) return false;
            return GetKeyState((WKeys)key, RegisteredKeyStates) == KeyStates.Releasing;
        }
    }
}
