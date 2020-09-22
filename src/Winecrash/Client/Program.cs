﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WEngine;
using WEngine.GUI;
using WEngine.Networking;
using Winecrash.Net;
using Label = WEngine.GUI.Label;

namespace Client
{
    static class Program
    {
        private static ManualResetEvent End = new ManualResetEvent(false);

        private static Label LbDebug;

        public static GameClient Client;

        static void Main(string[] args)
        {

            Engine.Run(true).Wait();

            Engine.OnStop += () => End.Set();

            CreateDebugWindow();

            GameApplication app = (GameApplication)Graphics.Window;
            app.OnLoaded += () =>
            {
                WObject debugLabel = new WObject("Debug Text") { Parent = Canvas.Main.WObject };
                LbDebug = debugLabel.AddModule<Label>();
                LbDebug.Aligns = TextAligns.Up | TextAligns.Left;
                LbDebug.FontSize = 42;
                LbDebug.Text = "\\/ DEBUG \\/";

                WObject connectorWrapper = new WObject("Connector") { Parent = Canvas.Main.WObject };
                Image wrapperPanel = connectorWrapper.AddModule<Image>();
                wrapperPanel.MinAnchor = new Vector2D(0.6, 0.6);
                wrapperPanel.Color = new Color256();
                CreateDebugConnector(connectorWrapper);

                /*GameClient client = new GameClient("localhost", 27716);
                client.OnConnect += (tpcclient) =>
                Task.Run(async () =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        NetObject.Send(new NetPing(), tpcclient.Client);
                        await Task.Delay(1000);
                    }
                });*/
            };

            NetPing.OnPingBack += (span) => {
                Debug.Log($"Ping to the server: {span.TotalMilliseconds:F0} ms");
            };


            WObject wobj = new WObject();
            wobj.AddModule<ClientTester>();

            //Thread thread = new Thread(async () =>
            //{
            //Debug.Log("yes");
            /*for (int i = 0; i < 1; i++)
            {*/
            /*Task.Run(async () =>
            {
                await Task.Delay(10)
                NetObject.Send(new NetPing(DateTime.UtcNow, default), client.Client.Client);
            };*/
                    //await Task.Delay(100);
                //}
            /*})
            {
                Priority = ThreadPriority.Highest
            };*/

            //thread.Start();



            Task.Run(() =>
            {
                End.WaitOne();
                //thread.Abort();
                return;
            }).Wait();
        }

        

        private static void CreateDebugConnector(WObject parent)
        {
            WObject connectorPanelWobj = new WObject("Server Connector") { Parent = parent };
            Image connectorPanel = connectorPanelWobj.AddModule<Image>();
            connectorPanel.Color = Color256.Gray;

            WObject header = new WObject("header") { Parent = connectorPanelWobj };
            Label lbHeader = header.AddModule<Label>();
            lbHeader.Text = "(Debug) Connect to server";
            lbHeader.Aligns = TextAligns.Up | TextAligns.Vertical;
            lbHeader.AutoSize = true;
            lbHeader.MinAnchor = new Vector2D(0, 0.9);

            WObject addressInputWobj = new WObject("Server Address Input") { Parent = connectorPanelWobj };
            TextInputField addressField = addressInputWobj.AddModule<TextInputField>();
            addressField.KeepRatio = true;
            addressField.Background.Picture = Texture.GetOrCreate("assets/textures/text_field.png");
            addressField.MinAnchor = new Vector2D(0.025, 0.5);
            addressField.MaxAnchor = new Vector2D(0.675, 0.75);
            addressField.EmptyText = "localhost";
            addressField.Label.MinAnchor = new Vector2D(0.02, 0.1);
            addressField.Label.MaxAnchor = new Vector2D(0.98, 0.9);
            addressField.Label.Color = new Color256(1.0, 1.0, 1.0, 1.0);
            addressField.Label.AutoSize = true;

            WObject addressPortWobj = new WObject("Server Port Input") { Parent = connectorPanelWobj };
            TextInputField portField = addressPortWobj.AddModule<TextInputField>();
            portField.KeepRatio = true;
            portField.Background.Picture = Texture.GetOrCreate("assets/textures/short_text_field.png");
            portField.MinAnchor = new Vector2D(0.685, 0.5);
            portField.MaxAnchor = new Vector2D(0.975, 0.75);
            portField.EmptyText = "27716";
            portField.Label.MinAnchor = new Vector2D(0.02, 0.1);
            portField.Label.MaxAnchor = new Vector2D(0.98, 0.9);
            portField.Label.Color = new Color256(1.0, 1.0, 1.0, 1.0);
            portField.Label.AutoSize = true;

            WObject connectButtonWobj = new WObject("connect button") { Parent = connectorPanelWobj };
            Button btConnect = connectButtonWobj.AddModule<Button>();
            btConnect.KeepRatio = false;
            btConnect.MinAnchor = new Vector2D(0.0, 0.0);
            btConnect.MaxAnchor = new Vector2D(1.0, 0.45);
            btConnect.Label.Text = "Connect";
            btConnect.Label.Aligns = TextAligns.Middle;
            btConnect.Label.AutoSize = true;
            btConnect.Label.Color = Color256.White;
            btConnect.IdleColor = Color256.LightGray;
            btConnect.Label.MinAnchor = new Vector2D(0.25, 0.25);
            btConnect.Label.MaxAnchor = new Vector2D(0.75, 0.75);

            btConnect.OnClick += () => 
            {
                Debug.Log("Connecting to server " + (addressField.Text ?? addressField.EmptyText) + ":" + (string.IsNullOrEmpty(portField.Text) ? portField.EmptyText : portField.Text));

                try
                {
                    Client = new GameClient(addressField.Text ?? addressField.EmptyText, int.Parse(string.IsNullOrEmpty(portField.Text) ? portField.EmptyText : portField.Text));
                    Client.OnConnected += (server) =>
                    {
                        NetObject.Send(new NetPlayer("Arthur"), server.Client);
                        Debug.Log("Connected !");
                    };

                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);

                    TimeSpan span = new TimeSpan();
                }

                
            };
        }

        private static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarning , LogError, LogException));
        }

        private static void LogVerbose(object obj)
        {
            if (LbDebug == null) return;
            string msg = "\nVerbose   - " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }

        private static void LogWarning(object obj)
        {
            if (LbDebug == null) return;

            string msg = "\nWarning   - " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }

        private static void LogError(object obj)
        {
            if (LbDebug == null) return;

            string msg = "\nError     - " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }

        private static void LogException(object obj)
        {
            if (LbDebug == null) return;

            string msg = "\nExeception- " + (obj == null ? "Null" : obj.ToString());
            LbDebug.Text += msg;
        }
    }
}
