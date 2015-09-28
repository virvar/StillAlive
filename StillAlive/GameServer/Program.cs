using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameProcess;
using System.IO;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server started.");
            var settings = LoadSettings("Settings.txt");
            GameStateServer gameState = new GameStateServer(settings.Map);
            Communicator communicator = new Communicator(settings.ServerPort, settings.ServerConnectionPort, gameState);
            Console.ReadKey();
            System.Threading.Thread.Sleep(5000);
        }

        private static Settings LoadSettings(string file)
        {
            var settings = new Settings();
            settings.ServerPort = -1;
            settings.ServerConnectionPort = -1;
            using (var sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("[ServerPort]"))
                    {
                        settings.ServerPort = int.Parse(line.Split('"')[1]);
                    }
                    else if (line.StartsWith("[ServerConnectionPort]"))
                    {
                        settings.ServerConnectionPort = int.Parse(line.Split('"')[1]);
                    }
                    else if (line.StartsWith("[Map]"))
                    {
                        settings.Map = line.Split('"')[1];
                    }
                }
            }
            if (settings.ServerConnectionPort == -1 ||
                settings.ServerPort == -1 ||
                settings.Map == null)
            {
                throw new Exception("В Settings.txt отсутствуют все необходимые данные о сервере.");
            }
            return settings;
        }
    }

    class Settings
    {
        public int ServerPort { get; set; }
        public int ServerConnectionPort { get; set; }
        public string Map { get; set; }
    }
}
