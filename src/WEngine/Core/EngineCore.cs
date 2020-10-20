namespace WEngine
{
    public sealed class EngineCore : Module
    {
        public static EngineCore Instance { get; private set; }

        public override bool Undeletable { get; internal set; } = true;

        protected internal override void Creation()
        {
            if(Instance)
            {
                this.ForcedDelete();
                return;
            }

            Instance = this;
        }

        protected internal override void Start()
        {
            this.WObject.Position = Vector3F.Left * 5.0F;
        }

        protected internal override void Update()
        {
            // alt f4 close
            if (Input.IsPressed(Keys.LeftAlt) && Input.IsPressing(Keys.F4))
            {
                Graphics.Window.Close();
                return;
            }

            // fullscreen
            if (Input.IsPressed(Keys.LeftAlt) && Input.IsPressing(Keys.Enter) || Input.IsPressing(Keys.F11))
            {
                Graphics.Window.WindowState = Graphics.Window.WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
        }

        protected internal override void OnDelete()
        {
            if(Instance && Instance == this)
            {
                Instance = null;
            }
        }
    }
}
