using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    

    public delegate void ButtonClickDelegate();
    public delegate void ButtonHoverDelegate();
    public delegate void ButtonLockDelegate();
    public delegate void ButtonColorChange();

    public class Button : GUIModule
    {
        private static Winecrash.Engine.WRandom r = new WRandom();


        /// <summary>
        /// Event triggered when the button is clicked
        /// </summary>
        public event ButtonClickDelegate OnClick;
        /// <summary>
        /// Event triggered when the button is hovered by the mouse
        /// </summary>
        public event ButtonHoverDelegate OnHover;
        /// <summary>
        /// Event triggered when the button is not hovered anymore by the mouse
        /// </summary>
        public event ButtonHoverDelegate OnUnhover;
        public event ButtonLockDelegate OnLock;
        public event ButtonLockDelegate OnUnlock;

        public event ButtonColorChange OnColorChanged;

        private bool _Locked = false;
        /// <summary>
        /// Is the button clickable or locked?
        /// </summary>
        public bool Locked
        {
            get
            {
                return _Locked;
            }

            set
            {
                (value ? OnLock : OnUnlock)?.Invoke();

                _Locked = value;
            }
        }
        private bool _Hovered = false;
        /// <summary>
        /// Is the button hovered?
        /// </summary>
        public bool Hovered
        {
            get
            {
                return _Hovered;
            }

            private set
            {
                //if already hovered and sets to true, do nothing.
                if (_Hovered && value) return;

                //idem if not hovered and sets to false.
                else if (!_Hovered && !value) return;

                //otherwise invoke corresponding event
                else (value ? OnHover : OnUnhover)?.Invoke();

                _Hovered = value;
            }
        }

        /// <summary>
        /// The label used to display button's text
        /// </summary>
        public Label Label { get; private set; } = null;
        /// <summary>
        /// The button's background image.
        /// </summary>
        public Image Background { get; private set; } = null;


        private Color256 _LockedColor = Color256.DarkGray;
        /// <summary>
        /// The color of the button when locked.
        /// </summary>
        public Color256 LockedColor
        {
            get
            {
                return _LockedColor;
            }

            set
            {
                _LockedColor = value;
                OnColorChanged?.Invoke();
            }
        }
        private Color256 _IdleColor = Color256.White;
        /// <summary>
        /// The color of the button when nothing specific happens.
        /// </summary>
        public Color256 IdleColor
        {
            get
            {
                return _IdleColor;
            }

            set
            {
                _IdleColor = value;
                OnColorChanged?.Invoke();
            }
        }
        private Color256 _HoverColor = Color256.SkyBlue;
        /// <summary>
        /// The color of the button when nothing specific happens.
        /// </summary>
        public Color256 HoverColor
        {
            get
            {
                return _HoverColor;
            }

            set
            {
                _HoverColor = value;
                OnColorChanged?.Invoke();
            }
        }

        protected internal override void Creation()
        {
            this.ParentGUI = this.WObject.Parent.GetModule<GUIModule>();

            WObject bgWobj = new WObject("Image")
            {
                Parent = this.WObject
            };
            Background = bgWobj.AddModule<Image>();

            WObject lbWobj = new WObject("Label")
            {
                Parent = bgWobj
            };
            Label = lbWobj.AddModule<Label>();
            Label.Text = "Button";
            Label.Color = Color256.Black;
            this.Background.Color = IdleColor;

            OnHover += () =>
            {
                this.Background.Color = Locked ? LockedColor : HoverColor;
            };

            OnUnhover += () =>
            {
                this.Background.Color = Locked ? LockedColor : IdleColor;
            };

            OnLock += () =>
            {
                this.Background.Color = LockedColor;
            };

            OnUnlock += () =>
            {
                this.Background.Color = Hovered ? HoverColor : IdleColor;
            };

            OnClick += () =>
            {
                this.HoverColor = new Color32(r.Next(128, 256), r.Next(128, 256), r.Next(128, 256), 255);
            };

            OnColorChanged += () =>
            {
                Background.Color = Locked ? _LockedColor : (Hovered ? _HoverColor : _IdleColor);
            };
        }

        protected internal override void OnDisable()
        {
            if (Hovered)
            {
                Hovered = false;
            }

            base.OnEnable();
        }

        protected internal override void Update()
        {
            // if locked => FPS view => ignore.
            if (Input.LockMode == CursorLockModes.Lock)
            {
                Hovered = false;
            }

            Vector2I mpos = Input.MousePosition;
            Vector2I bpos = this.GlobalPosition.XY;
            Vector2I bsca = this.GlobalScale.XY / 2;

            //AABB with mouse point
            bool isHovered =
                   (mpos.X > bpos.X - bsca.X) && (mpos.X < bpos.X + bsca.X)  //within X
                && (mpos.Y > bpos.Y - bsca.Y) && (mpos.Y < bpos.Y + bsca.Y); //within Y

            Hovered = isHovered;

            if(Input.IsPressing(Keys.MouseLeftButton))
            {
                OnClick?.Invoke();
            }
        }
    }
}
