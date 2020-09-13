using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using Winecrash;

namespace Winecrash.Server
{
    public static class Entry
    {
        public static void Main(string[] args)
        {
            CreateDebugWindow();

            PrintSaves();

            //new Save("Interdits: \\/:*?\"<>|", false);
            //new Save("Sale **te", false);

            Console.ReadKey();
        }

        public static void PrintSaves()
        {
            var saves = Save.GetAllSaves();

            const string top_board = "╔═════════════════════════════════════════════════════════════════╗";
            const string line_board = "║─────────────────────┼─────────────────────┼─────────────────────║";
            const string bottom_board = "╚═════════════════════════════════════════════════════════════════╝";

            Console.WriteLine(top_board);
            Console.WriteLine("║ Folder name         │ Save name           │ Status              ║");
            Console.WriteLine(line_board);
            //for each folder
            for (int i = 0; i < saves.Length; i++)
            {
                string folder = saves[i].Key;

                Save saveFile = saves[i].Value.Key;
                SaveStatus status = saves[i].Value.Value;

                if (folder.Length > 19)
                {
                    folder = folder.Substring(0, 16) + "...";
                }
                else
                {
                    int dif = 19 - folder.Length;
                    for (int j = 0; j < dif; j++)
                    {
                        folder += " ";
                    }
                }
                string saveName = saveFile?.Informations.Name;
                if (saveFile != null)
                {
                    if (saveName.Length > 19)
                    {
                        saveName = saveName.Substring(0, 16) + "...";
                    }
                    else
                    {
                        int dif = 19 - saveName.Length;
                        for (int j = 0; j < dif; j++)
                        {
                            saveName += " ";
                        }
                    }
                }
                else
                {
                    saveName = "                   ";
                }

                string statusText = (status.HasFlag(SaveStatus.Unknown) ? "Unknown" : (status.HasFlag(SaveStatus.Corrupted) ? "Corrupted" : "Sane"));
                if(statusText == "Sane")
                {
                    if(status.HasFlag(SaveStatus.OutOfDate))
                    {
                        statusText = "Require Upgrade";
                    }
                }

                int statusTextDiff = 19 - statusText.Length;
                for (int j = 0; j < statusTextDiff; j++)
                {
                    statusText += " ";
                }



                Console.Write("║ " + folder + " │ " + saveName + " │ ");

                Console.ForegroundColor = (status.HasFlag(SaveStatus.Unknown) ? ConsoleColor.DarkGray : (status.HasFlag(SaveStatus.Corrupted) ? ConsoleColor.Red : ConsoleColor.Green));
                if (status.HasFlag(SaveStatus.OutOfDate))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.Write(statusText);       
                Console.ResetColor();

                Console.Write(" ║\n");

                if (i < saves.Length - 1)
                {
                    Console.WriteLine(line_board);
                }
            }
            Console.WriteLine(bottom_board);
        }

        private static void CreateDebugWindow()
        {
            Debug.AddLogger(new Logger(LogVerbose, LogWarn, LogErr, LogException));
        }

        private static void LogVerbose(object obj)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[ Verbose ] " + obj);
        }
        private static void LogWarn(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[ Warning ] " + obj);
            Console.ResetColor();
        }
        private static void LogErr(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[  Error  ] " + obj);
            Console.ResetColor();
        }
        private static void LogException(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[Exception] " + obj);
            Console.ResetColor();
        }
    }
}
