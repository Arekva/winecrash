using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEngine;
using WEngine.GUI;
using WEngine.Networking;
using Winecrash;
using Winecrash.GUI;
using Winecrash.Net;
using Label = WEngine.GUI.Label;

namespace Winecrash.Client
{
    public static class MainMenu
    {
        private static WObject MenuWobject = null;
        private static WObject MainMenuPanel = null;
        private static WObject MenuBGWObject = null;
        private static WObject OptionPanel = null;
        private static WObject MultiPanel = null;
        private static WObject MultiDisconnectionPanel = null;
        private static LocalizedLabel MultiDisconnectionReason = null;
            
        private static LocalizedLabel MultiErrorLabel { get; set; } = null;
        private static void CreateMenu()
        {
            MenuWobject = new WObject("Main Menu");
            MenuBGWObject = new WObject("Main Menu Background Object");
            MenuBGWObject.Parent = MenuWobject;
            MenuBGWObject.AddModule<MenuBackgroundControler>();

            WObject bgPanel = new WObject("Background Panel") { Parent = MenuWobject };
            bgPanel.Parent = MenuWobject;

            LocalizedLabel lbVersion = bgPanel.AddModule<LocalizedLabel>();
            lbVersion.LocalizationArgs = new object[] { Winecrash.Version };
            lbVersion.Localization = "#menu_game_version";          
            lbVersion.Aligns = TextAligns.Down | TextAligns.Left;
            lbVersion.AutoSize = true;
            lbVersion.MinAnchor = new Vector2F(0.0F, 0.0F);
            lbVersion.MaxAnchor = new Vector2F(1.0F, 0.05F);
            lbVersion.Color = Color256.White;
            lbVersion.Left = 20.0D;

            Label lbCopyright = bgPanel.AddModule<Label>();
            lbCopyright.Text = "Copyright Arekva 2020\nAll Rights Reserved";
            lbCopyright.Aligns = TextAligns.Down | TextAligns.Right;
            lbCopyright.AutoSize = true;
            lbCopyright.MinAnchor = new Vector2F(0.0F, 0.0F);
            lbCopyright.MaxAnchor = new Vector2F(1.0F, 0.1F);
            lbCopyright.Color = Color256.White;
            lbCopyright.Right = 20.0D;


            CreateMain();
            CreateOptions();
            CreateMultiplayer();
        }

        private static void CreateMain()
        {
            WObject mainPanel = MainMenuPanel = new WObject("Main UI Panel") { Parent = MenuWobject };
            Image mainPanelImg = mainPanel.AddModule<Image>();
            mainPanelImg.Color = new Color256(1.0, 0.0, 1.0, 0.0);
            mainPanelImg.MinAnchor = new Vector2F(0.225F, 0.10F);
            mainPanelImg.MaxAnchor = new Vector2F(0.775F, 0.95F);
            mainPanelImg.MinSize = new Vector3F(400.0F, 400.0F, Single.PositiveInfinity);
            mainPanelImg.MaxSize = new Vector3F(800.0F, 800.0F, Single.PositiveInfinity);
            mainPanelImg.KeepRatio = true;

            WObject logo = new WObject("Game Text Logo") { Parent = mainPanel };
            Image logoImage = logo.AddModule<Image>();
            logoImage.Picture = new Texture("assets/textures/logo.png");
            logoImage.MinAnchor = new Vector2F(0.0F, 0.8F);
            logoImage.MaxAnchor = new Vector2F(1.0F, 1.0F);
            logoImage.KeepRatio = true;

            Label lbTip = mainPanel.AddModule<Label>();
            lbTip.ParentGUI = logoImage;
            lbTip.Text = "Minecraft";
            lbTip.Color = new Color256(1.0, 1.0, 0.0, 1.0);
            lbTip.Aligns = TextAligns.Middle;
            lbTip.AutoSize = true;
            lbTip.MinAnchor = new Vector2F(0.7F, 0.0F);
            lbTip.MaxAnchor = new Vector2F(1.1F, 0.4F);
            lbTip.Rotation = -20.0D;


            MenuTip tip = MenuWobject.AddModule<MenuTip>();
            tip.ReferenceLabel = lbTip;
            lbTip.Text = tip.SelectRandom();


            WObject btnPanel = new WObject("Main UI Button Panel") { Parent = mainPanel };
            Image btnPanelImg = btnPanel.AddModule<Image>();
            btnPanelImg.Color = new Color256(1.0, 0.0, 1.0, 0.0);
            btnPanelImg.MinAnchor = new Vector2F(0.075F, 0.0F);
            btnPanelImg.MaxAnchor = new Vector2F(0.925F, 0.6F);

            WObject single = new WObject("Singleplayer Button") { Parent = btnPanel };
            GUI.LargeButton btnSingle = single.AddModule<GUI.LargeButton>();
            btnSingle.Button.MinAnchor = new Vector2F(0.0F, 0.9F);
            btnSingle.Button.MaxAnchor = new Vector2F(1.0F, 1.0F);
            btnSingle.Label.Localization = "#menu_singleplayer";
            btnSingle.Button.Locked = false;
            //btnSingle.Button.OnClick += () => { Graphics.Window.InvokeUpdate(() => Program.RunGameDebug()); };
            btnSingle.Button.OnClick += () =>
            {
                Game.InvokePartyJoined(PartyType.Singleplayer);
                Task.Run(() =>
                {
                    //Parallel.ForEach()
                    //IntegratedServer server = IntegratedServer.CurrentIntegratedServer = new IntegratedServer(new Player("Arthur"));
                    //server.Run();
                });
            };

            WObject mult = new WObject("Multiplayer Button") { Parent = btnPanel };
            GUI.LargeButton btnMult = mult.AddModule<GUI.LargeButton>();
            btnMult.Button.MinAnchor = new Vector2F(0.0F, 0.7F);
            btnMult.Button.MaxAnchor = new Vector2F(1.0F, 0.8F);
            btnMult.Label.Localization = "#menu_multiplayer";
            btnMult.Button.Locked = false;
            btnMult.Button.OnClick += () =>
            {
                ShowMulti();
            };

            WObject mods = new WObject("Mods Button") { Parent = btnPanel };
            GUI.LargeButton btnMods = mods.AddModule<GUI.LargeButton>();
            btnMods.Button.MinAnchor = new Vector2F(0.0F, 0.5F);
            btnMods.Button.MaxAnchor = new Vector2F(1.0F, 0.6F);
            btnMods.Label.Localization = "#menu_mods";
            btnMods.Button.Locked = true;

            WObject options = new WObject("Options Button") { Parent = btnPanel };
            GUI.SmallButton btnOptions = options.AddModule<GUI.SmallButton>();
            btnOptions.Button.MinAnchor = new Vector2F(0.0F, 0.2F);
            btnOptions.Button.MaxAnchor = new Vector2F(0.45F, 0.3F);
            btnOptions.Label.Localization = "#menu_settings";
            btnOptions.Button.OnClick += () => ShowOptions();
            btnOptions.Button.Locked = false;
            
            WObject quit = new WObject("Quit Button") { Parent = btnPanel };
            GUI.SmallButton btnQuit = quit.AddModule<GUI.SmallButton>();
            btnQuit.Button.MinAnchor = new Vector2F(0.55F, 0.2F);
            btnQuit.Button.MaxAnchor = new Vector2F(1.0F, 0.3F);
            btnQuit.Label.Localization = "#menu_quit_game";
            btnQuit.Button.OnClick += () => Engine.Stop();
        }

        public static void CreateOptions()
        {

            WObject panel = OptionPanel = new WObject("Main Menu Options") { Parent = MenuWobject };
            /*Image bgFullScreen = OptionPanel.AddModule<Image>();
            bgFullScreen.Color = Color256.DarkGray * new Color256(1.0, 1.0, 1.0, 0.75);*/

            Image mainPanel = panel.AddModule<Image>();
            mainPanel.Color = Color256.White * 0.0;//new Color256(1.0, 1.0, 1.0, 0.4);
            mainPanel.MinAnchor = new Vector2D(0.2, 0.05D);
            mainPanel.MaxAnchor = new Vector2D(0.8, 0.95D);
            mainPanel.MinSize = new Vector3D(800.0D, 400.0D, double.NegativeInfinity);
            

            WObject tabsPanelWobj = new WObject("Options tab panel") { Parent = panel };
            Image tabsPanel = tabsPanelWobj.AddModule<Image>();
            tabsPanel.MinAnchor = new Vector2D(0.0, 0.85);
            tabsPanel.MaxAnchor = new Vector2D(1.0, 1.0);
            tabsPanel.Color = new Color256(1.0, 1.0, 1.0, .0);

            WObject backOptions = new WObject("Back Options Button") { Parent = tabsPanelWobj };
            GUI.TinyButton btnBack = backOptions.AddModule<GUI.TinyButton>();
            btnBack.Button.MinAnchor = new Vector2D(0.0, 0.0);
            btnBack.Button.MaxAnchor = new Vector2D(0.06, 1.0);
            btnBack.Button.Label.Text = "<";
            btnBack.Button.OnClick += () => ShowMain();

            WObject gameOptions = new WObject("Game Options Panel") { Parent = panel };
            Image gameOptionsPanel = gameOptions.AddModule<Image>();
            gameOptionsPanel.MinAnchor = new Vector2D(0.0, 0.0);
            gameOptionsPanel.MaxAnchor = new Vector2D(1.0, 0.85);
            gameOptionsPanel.Color = Color256.White;

            WObject gameOptionsTab = new WObject("Game Options Button") { Parent = tabsPanelWobj };
            GUI.SmallButton btnGame = gameOptionsTab.AddModule<GUI.SmallButton>();
            btnGame.Button.MinAnchor = new Vector2D(WMath.Remap(0.00, 0.0, 1.0, 0.1, 1.0), 0.0);
            btnGame.Button.MaxAnchor = new Vector2D(WMath.Remap(0.30, 0.0, 1.0, 0.1, 1.0), 1.0);
            btnGame.Button.Label.Text = "Game";
            btnGame.Button.OnClick += () =>
            {
                gameOptions.Enabled = true;
            };

            

            WObject controlsOptionsTab = new WObject("Controls Options Button") { Parent = tabsPanelWobj };
            GUI.SmallButton btnCtrls = controlsOptionsTab.AddModule<GUI.SmallButton>();
            btnCtrls.Button.MinAnchor = new Vector2D(WMath.Remap(0.35, 0.0, 1.0, 0.1, 1.0), 0.0);
            btnCtrls.Button.MaxAnchor = new Vector2D(WMath.Remap(0.65, 0.0, 1.0, 0.1, 1.0), 1.0);
            btnCtrls.Button.Label.Text = "Controls";

            WObject videoOptionsTab = new WObject("Controls Options Button") { Parent = tabsPanelWobj };
            GUI.SmallButton btnVideo = videoOptionsTab.AddModule<GUI.SmallButton>();
            btnVideo.Button.MinAnchor = new Vector2D(WMath.Remap(0.70, 0.0, 1.0, 0.1, 1.0), 0.0);
            btnVideo.Button.MaxAnchor = new Vector2D(WMath.Remap(1.00, 0.0, 1.0, 0.1, 1.0), 1.0);
            btnVideo.Button.Label.Text = "Video";
        }
        
        public static void CreateMultiplayer()
        {
            WObject mainPanel = MultiPanel = new WObject("Multi UI Panel") { Parent = MenuWobject };
            Image mainPanelImg = mainPanel.AddModule<Image>();
            mainPanelImg.Color = new Color256(1.0, 0.0, 1.0, 0.0);
            mainPanelImg.MinAnchor = new Vector2F(0.225F, 0.10F);
            mainPanelImg.MaxAnchor = new Vector2F(0.775F, 0.95F);
            mainPanelImg.MinSize = new Vector3F(400.0F, 400.0F, Single.PositiveInfinity);
            mainPanelImg.MaxSize = new Vector3F(800.0F, 800.0F, Single.PositiveInfinity);
            mainPanelImg.KeepRatio = true;
            
            WObject btnPanel = new WObject("Main UI Button Panel") { Parent = mainPanel };
            Image btnPanelImg = btnPanel.AddModule<Image>();
            btnPanelImg.Color = new Color256(1.0, 0.0, 1.0, 0.0);
            btnPanelImg.MinAnchor = new Vector2F(0.075F, 0.0F);
            btnPanelImg.MaxAnchor = new Vector2F(0.925F, 0.6F);
            
            WObject multiTitle = new WObject("Multi Main Label") { Parent = btnPanel };
            LocalizedLabel mainLb = multiTitle.AddModule<LocalizedLabel>();
            mainLb.Localization = "#multiplayer_menu_title";
            mainLb.MinAnchor = new Vector2F(0.0F, 1.0F);
            mainLb.MaxAnchor = new Vector2F(1.0F, 1.1F);
            mainLb.AutoSize = true;
            mainLb.Aligns = TextAligns.Middle;
            
            WObject error = new WObject("Multi Error Label") {Parent = btnPanel };
            LocalizedLabel errLb = MultiErrorLabel = error.AddModule<LocalizedLabel>();
            errLb.Localization = "#server_connection_error";
            errLb.MinAnchor = new Vector2F(0.0F, 0.82F);
            errLb.MaxAnchor = new Vector2F(1.0F, 0.88F);
            errLb.AutoSize = true;
            errLb.Color = Color256.Red;
            errLb.Enabled = false;

            WObject addressTif = new WObject("Multi Address Input") {Parent = btnPanel };
            GUI.LargeTextField taddress = addressTif.AddModule<LargeTextField>();
            taddress.MinAnchor = new Vector2F(0.0F, 0.7F);
            taddress.MaxAnchor = new Vector2F(1.0F, 0.8F);
            taddress.Localization = "#enter_server_address";
            taddress.Text = Game.LastAddress;


            WObject back = new WObject("Multi Back") { Parent = btnPanel };
            GUI.SmallButton btnBack = back.AddModule<GUI.SmallButton>();
            btnBack.Button.MinAnchor = new Vector2F(0.0F, 0.5F);
            btnBack.Button.MaxAnchor = new Vector2F(0.45F, 0.6F);
            btnBack.Label.Localization = "#menu_back";
            btnBack.Button.OnClick += () =>
            { 
                HideMulti();
                ShowMain();
            };
            
            WObject connect = new WObject("Multi Connect") { Parent = btnPanel };
            GUI.SmallButton btnConnect = connect.AddModule<GUI.SmallButton>();
            btnConnect.Button.MinAnchor = new Vector2F(0.55F, 0.5F);
            btnConnect.Button.MaxAnchor = new Vector2F(1.0F, 0.6F);
            btnConnect.Label.Localization = "#connect_to_server";

            void ShowConnectError(string reason)
            {
                errLb.LocalizationArgs = new[] { reason };
                errLb.Localization = "#server_connection_error";
                errLb.Enabled = true;
            }
            
            btnConnect.Button.OnClick += () =>
            {
                Task.Run(() =>
                {
                    errLb.LocalizationArgs = null;
                    errLb.Enabled = false;

                    string address = "localhost";
                    int port = Networking.DefaultPort;

                    string input = taddress.Text;
                    Game.LastAddress = input;

                    if (string.IsNullOrEmpty(input) | string.IsNullOrWhiteSpace(input))
                    {
                        ShowConnectError(Game.Language.GetText("#error_no_address_entered"));
                        return;
                    }

                    int separatorPos = input.IndexOf(':');
                    if (separatorPos > 0) // custom port
                    {
                        address = input.Substring(0, separatorPos);

                        if (int.TryParse(input.Substring(separatorPos + 1, input.Length - 1 - separatorPos), out port))
                        {
                            if (port < 0 || port > Networking.MaxPort)
                            {
                                ShowConnectError(Game.Language.GetText("#error_invalid_port"));
                                return;
                            }
                        }
                        else
                        {
                            ShowConnectError(Game.Language.GetText("#error_invalid_port"));
                            return;
                        }
                    }
                    else
                    {
                        address = input;
                    }

                    btnConnect.Button.Locked = true;
                    btnBack.Button.Locked = true;
                    taddress.Locked = true;
                    btnConnect.Label.Localization = "#connecting_to_server";
                    if (!Program.Client)
                    {
                        Program.Client = new GameClient(new Player("Arthur"));
                    }

                    try
                    {
                        Program.Client.Connect(address, port);
                        Game.InvokePartyJoined(PartyType.Multiplayer);
                        MainMenu.Hide();
                    }
                    catch (SocketException e)
                    {
                        switch (e.SocketErrorCode)
                        {
                            case SocketError.HostNotFound:
                                ShowConnectError(Game.Language.GetText("#error_socket_host_not_found"));
                                break;
                            case SocketError.ConnectionRefused:
                                ShowConnectError(Game.Language.GetText("#error_socket_connection_refused"));
                                break;
                            case SocketError.TryAgain:
                                ShowConnectError(Game.Language.GetText(("#error_socket_try_again")));
                                break;
                            default:
                                ShowConnectError(e.Message);
                                break;
                        }

                    }
                    finally
                    {
                        btnConnect.Button.Locked = false;
                        btnBack.Button.Locked = false;
                        taddress.Locked = false;
                        btnConnect.Label.Localization = "#connect_to_server";
                    }
                });
            };
        }
        
         public static void CreateDisconnection()
        {
            WObject mainPanel = MultiDisconnectionPanel = new WObject("Multi Disconnect UI Panel") { Parent = MenuWobject };
            Image mainPanelImg = mainPanel.AddModule<Image>();
            mainPanelImg.Color = new Color256(1.0, 0.0, 1.0, 0.0);
            mainPanelImg.MinAnchor = new Vector2F(0.225F, 0.10F);
            mainPanelImg.MaxAnchor = new Vector2F(0.775F, 0.95F);
            mainPanelImg.MinSize = new Vector3F(400.0F, 400.0F, Single.PositiveInfinity);
            mainPanelImg.MaxSize = new Vector3F(800.0F, 800.0F, Single.PositiveInfinity);
            mainPanelImg.KeepRatio = true;
            
            WObject btnPanel = new WObject("Main UI Button Panel") { Parent = mainPanel };
            Image btnPanelImg = btnPanel.AddModule<Image>();
            btnPanelImg.Color = new Color256(1.0, 0.0, 1.0, 0.0);
            btnPanelImg.MinAnchor = new Vector2F(0.075F, 0.0F);
            btnPanelImg.MaxAnchor = new Vector2F(0.925F, 0.6F);
            
            WObject multiTitle = new WObject("Multi Main Label") { Parent = btnPanel };
            LocalizedLabel mainLb = multiTitle.AddModule<LocalizedLabel>();
            mainLb.Localization = "#multiplayer_disconnected";
            mainLb.MinAnchor = new Vector2F(0.0F, 1.0F);
            mainLb.MaxAnchor = new Vector2F(1.0F, 1.1F);
            mainLb.AutoSize = true;
            mainLb.Aligns = TextAligns.Middle;
            
            WObject error = new WObject("Multi Disconnected Label") { Parent = btnPanel };
            LocalizedLabel reaLb = MultiDisconnectionReason = error.AddModule<LocalizedLabel>();
            reaLb.Localization = "#server_disconnection_unspecified";
            reaLb.MinAnchor = new Vector2F(0.0F, 0.82F);
            reaLb.MaxAnchor = new Vector2F(1.0F, 0.88F);
            reaLb.AutoSize = true;
            reaLb.Aligns = TextAligns.Up | TextAligns.Vertical;
            
            WObject back = new WObject("Multi Back") { Parent = btnPanel };
            GUI.LargeButton btnBack = back.AddModule<GUI.LargeButton>();
            btnBack.Button.MinAnchor = new Vector2F(0.0F, 0.6F);
            btnBack.Button.MaxAnchor = new Vector2F(1.0F, 0.7F);
            btnBack.Label.Localization = "#menu_back";
            btnBack.Button.OnClick += () =>
            {
                HideDisconnection();
                ShowMain();
            };
        }

         public static void ShowDisconnection(string reasonLoc = "#server_disconnection_unspecified")
         {
             if (!MultiDisconnectionPanel) CreateDisconnection();
             
             HideMain();
             MultiDisconnectionReason.Localization = reasonLoc;
             MultiDisconnectionPanel.Enabled = true;
         }

         public static void HideDisconnection()
         {
             if(MultiDisconnectionPanel) MultiDisconnectionPanel.Enabled = false;
         }
        
        
        private static void ShowOptions()
        {
            if (OptionPanel) OptionPanel.Enabled = true;
            HideMain();
        }
        
        private static void ShowMulti()
        {
            if (MultiPanel) MultiPanel.Enabled = true;
            HideMain();
        }

        private static void HideOptions()
        {
            if (OptionPanel) OptionPanel.Enabled = false;
        }
        
        private static void HideMulti()
        {
            if (MultiPanel)
            {
                MultiErrorLabel.Enabled = false;
                MultiPanel.Enabled = false;
            }
        }
        public static void ShowMain()
        {
            HideOptions();
            HideMulti();
            MainMenuPanel.Enabled = true;
        }
        public static void HideMain()
        {
            MainMenuPanel.Enabled = false;
        }

        public static void Show()
        {
            MainLoadScreen.Hide();
            if (!MenuWobject)
            {
                CreateMenu();
            }

            ShowMain();
            MenuWobject.Enabled = true;
        }

        public static void Hide()
        {
            //Camera.Main.FOV = 45.0D;
            //Camera.Main.WObject.LocalRotation = new Quaternion(0, 0, 0);
            //MainMenuPanel.Enabled = false;
            HideMain();
            HideOptions();
            HideMulti();

            MenuWobject.Enabled = false;
        }
    }
}
