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
using Winecrash.Net;
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
                WObject buttonConnect = new WObject("Debug Text") { Parent = Canvas.Main.WObject };
                LbDebug = buttonConnect.AddModule<Label>();
                LbDebug.Aligns = TextAligns.Up | TextAligns.Left;
                LbDebug.FontSize = 42;
                LbDebug.Text = "DEBUG:\n";

                GameClient client = new GameClient("127.0.0.1", 27716);

                Debug.Log("test");
            };


            Task.Run(() =>
            {
                End.WaitOne();
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
