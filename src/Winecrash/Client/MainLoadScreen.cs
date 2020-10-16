using WEngine;
using WEngine.GUI;

namespace Winecrash.Client
{
    public static class MainLoadScreen
    {
        private static WObject _PanelWobject;

        private static void Create()
        {
            _PanelWobject = new WObject("Main Loading Screen") { Parent = Canvas.Main.WObject };
            Image bg = _PanelWobject.AddModule<Image>();
            bg.Picture = Texture.GetOrCreate("assets/textures/icon.png");
            bg.Color = Color256.White;
        }
        
        public static void Show()
        {
            if(!_PanelWobject) Create();

            _PanelWobject.Enabled = true;
        }

        public static void Hide()
        {
            if(_PanelWobject) _PanelWobject.Enabled = false;
        }
    }
}