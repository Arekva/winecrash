using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEngine;
using WEngine.GUI;
using WEngine.Networking;
using Label = WEngine.GUI.Label;

namespace Client
{
    static class Program
    {
        private static ManualResetEvent End = new ManualResetEvent(false);

        private static Label LbDebug;

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

                WObject addressInputWobj = new WObject("Server Address Input") { Parent = Canvas.Main.WObject };
                TextInputField addressField = addressInputWobj.AddModule<TextInputField>();
                addressField.KeepRatio = true;
                addressField.Background.Picture = new Texture("assets/textures/text_field.png");
                addressField.MinAnchor = new Vector2D(0.3, 0.25);
                addressField.MaxAnchor = new Vector2D(0.7, 0.75);

                addressField.Label.MinAnchor = new Vector2D(0.01, 0.1);
                addressField.Label.MaxAnchor = new Vector2D(0.99, 0.9);
                addressField.Label.Color = new Color256(1.0, 1.0, 1.0, 1.0);
                addressField.Label.AutoSize = true;


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
