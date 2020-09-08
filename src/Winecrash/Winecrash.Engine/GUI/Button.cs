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
    public delegate void ButtonPropertyChange();

    public class Button : GUIModule
    {
#region Events
        /// <summary>
        /// Event triggered when the button is clicked.
        /// </summary>
        public event ButtonClickDelegate OnClick;
        /// <summary>
        /// Event triggered when the button is hovered by the mouse.
        /// </summary>
        public event ButtonHoverDelegate OnHover;
        /// <summary>.
        /// Event triggered when the button is not hovered anymore by the mouse
        /// </summary>
        public event ButtonHoverDelegate OnUnhover;
        /// <summary>
        /// Event triggered when the button gets locked.
        /// </summary>
        public event ButtonLockDelegate OnLock;
        /// <summary>
        /// Event triggered when the button gets unlocked.
        /// </summary>
        public event ButtonLockDelegate OnUnlock;
        /// <summary>
        /// Event triggered when the color is changed.
        /// </summary>
        public event ButtonPropertyChange OnColorChanged;
#endregion

#region Fields
        private bool _Locked = false;
        private bool _Hovered = false;
        private Color256 _LockedColor = Color256.DarkGray;
        private Color256 _IdleColor = Color256.White;
        private Color256 _HoverColor = Color256.SkyBlue;
#endregion

#region Properties
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
                //if both true or both false, do not invoke
                if(!(_Locked && value) && !(!_Locked && !value))
                {
                    (value ? OnLock : OnUnlock)?.Invoke();
                }

                _Locked = value;
            }
        }      
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
        /// Do the background image should keep its ratio?
        /// </summary>
        public bool KeepRatio
        {
            get
            {
                return Background.KeepRatio;
            }

            set
            {
                Background.KeepRatio = value;
            }
        }
        /// <summary>
        /// The label used to display button's text
        /// </summary>
        public Label Label { get; set; } = null;
        /// <summary>
        /// The button's background image.
        /// </summary>
        public Image Background { get; private set; } = null;    
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
        #endregion

#region Engine Logic
        protected internal override void Creation()
        {
            this.ParentGUI = this.WObject.Parent?.GetModule<GUIModule>();

            WObject bgWobj = new WObject("Image")
            {
                Parent = this.WObject
            };
            this.Background = bgWobj.AddModule<Image>();

            WObject lbWobj = new WObject("Label")
            {
                Parent = bgWobj
            };

            this.Label = lbWobj.AddModule<Label>();
            this.Label.Text = "Button";
            this.Label.Color = Color256.Black;
            this.Background.Color = IdleColor;

            this.OnHover += () =>
            {
                this.Background.Color = this.Locked ? this.LockedColor : this.HoverColor;
            };

            this.OnUnhover += () =>
            {
                this.Background.Color = this.Locked ? this.LockedColor : this.IdleColor;
            };

            this.OnLock += () =>
            {
                this.Background.Color = this.LockedColor;
            };

            this.OnUnlock += () =>
            {
                this.Background.Color = this.Hovered ? this.HoverColor : this.IdleColor;
            };

            this.OnColorChanged += () =>
            {
                this.Background.Color = this.Locked ? this._LockedColor : (this.Hovered ? this._HoverColor : this._IdleColor);
            };

        }
        protected internal override void OnDisable()
        {
            if (Hovered)
            {
                Hovered = false;
            }

            this.Background.Enabled = false;

            base.OnEnable();
        }
        protected internal override void OnEnable()
        {
            this.Background.Enabled = true;
            base.OnEnable();
        }
        protected internal override void Update()
        {
            // if locked => FPS view => ignore.
            if (Input.LockMode == CursorLockModes.Lock)
            {
                Hovered = false;
            }

            if (Locked) return;

            Vector2I mpos = Input.MousePosition;
            Vector2I bpos = (Vector2I)this.Background.GlobalPosition.XY;
            Vector2I bsca = (Vector2I)this.Background.GlobalScale.XY / 2;

            //AABB with mouse point
            bool isHovered =
                   (mpos.X > bpos.X - bsca.X) && (mpos.X < bpos.X + bsca.X)  //within X
                && (mpos.Y > bpos.Y - bsca.Y) && (mpos.Y < bpos.Y + bsca.Y); //within Y

            Hovered = isHovered;

            if(Hovered && Input.IsPressing(Keys.MouseLeftButton))
            {
                OnClick?.Invoke();
            }
        }
#endregion
    }
}
