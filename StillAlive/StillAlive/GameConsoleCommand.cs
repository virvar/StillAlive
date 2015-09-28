using GameProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virvar.Net;

namespace StillAlive
{
    public class GameConsoleCommand
    {
        private static Dictionary<string, CommandCode> s_commandsMatchings;

        public CommandCode CommandCode { get; private set; }
        public string CommandName { get; private set; }
        public string CommandParams { get; private set; }

        static GameConsoleCommand()
        {
            s_commandsMatchings = new Dictionary<string, CommandCode>(1);
            s_commandsMatchings.Add("name", CommandCode.Name);
        }

        public GameConsoleCommand(GameStateClient gameState, string commandText)
        {
            var data = commandText.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            CommandName = data[0];
            CommandCode code;
            if (s_commandsMatchings.TryGetValue(CommandName.ToLower(), out code))
            {
                CommandCode = code;
            }
            if (data.Length > 1)
            {
                CommandParams = data[1];
            }
            if (code != (CommandCode)0)
            {
                gameState.NetPlayer.ClientPacket.ConsoleCommand = new ConsoleCommand(code, CommandParams);
            }
        }
    }
}
