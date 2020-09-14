using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WEngine;
using Winecrash.Net;

namespace Client
{
    static class Program
    {
        private static ManualResetEvent End = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            Engine.Run(true);

            Engine.OnStop += () => End.Set();

            GameClient client = new GameClient("127.0.0.1", 27716);

            /*for (int i = 0; i < 10000; i++)
            {
                client.SendObject(new NetMessage("Hello, World !"));
            }*/
            

            Task.Run(() =>
            {
                End.WaitOne();
                return;
            }).Wait();
        }
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        /*[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }*/
    }
}
