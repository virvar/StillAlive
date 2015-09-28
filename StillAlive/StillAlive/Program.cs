#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace StillAlive
{
    public static class Program
    {
        private static Game1 game;

        [STAThread]
        static void Main()
        {
            var settings = LoadSettings("Settings.txt");
            game = new Game1(settings);
            game.Run();
        }

        private static Settings LoadSettings(string file)
        {
            var settings = new Settings();
            settings.ServerPort = -1;
            settings.ServerConnectionPort = -1;
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("[Server]"))
                    {
                        settings.ServerAddress = line.Split('"')[1];
                    }
                    else if (line.StartsWith("[ServerPort]"))
                    {
                        settings.ServerPort = int.Parse(line.Split('"')[1]);
                    }
                    else if (line.StartsWith("[ServerConnectionPort]"))
                    {
                        settings.ServerConnectionPort = int.Parse(line.Split('"')[1]);
                    }
                }
            }
            if (settings.ServerAddress == null ||
                settings.ServerConnectionPort == -1 ||
                settings.ServerPort == -1)
            {
                throw new Exception("В Settings.txt отсутствуют все необходимые данные о сервере.");
            }
            return settings;
        }
    }

    class Settings
    {
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public int ServerConnectionPort { get; set; }
    }
}
