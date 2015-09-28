using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using GameObjects;
using GameProcess;
using Virvar.Net;
using GameObjects.ProtoClasses;

namespace GameServer
{
    class Communicator
    {
#if DEBUG
        private TimeSpan connectionTimeout = new TimeSpan(1, 0, 5);
#else
        private TimeSpan connectionTimeout = new TimeSpan(0, 0, 5);
#endif
        private GameStateServer _gameState;
        private Dictionary<int, NetPlayer> _newPlayers = new Dictionary<int, NetPlayer>();
        private IMessenger _inputMessenger;
        private IObjMessenger _connectionsMessenger;
        private int updateInterval = 40;
        private bool shouldContinue = true;
        Stopwatch stopWatch = new Stopwatch();
        int packetsReceivedCount = 0;
        int maxPacketLength = 0;

        public Communicator(int serverPort, int serverConnectionPort, GameStateServer gameState)
        {
            _gameState = gameState;
            _inputMessenger = new UdpMessenger(serverPort);
            _connectionsMessenger = new ProtoMessenger<ConnectionMessage>(new UdpMessenger(serverConnectionPort));
            new Thread(StartReceivingConnections).Start();
            new Thread(StartReceiving).Start();
            new Thread(StartSending).Start();
            new Thread(CheckDisconnections).Start();
            GameLogic.CreateInstance(_gameState);
        }

        void StartReceivingConnections()
        {
            // запускаем поток получения сообщений от клиентов
            while (shouldContinue)
            {
                IPEndPoint remoteEP = null;
                int packetLength;
                ConnectionMessage msg = (ConnectionMessage)_connectionsMessenger.Receive(ref remoteEP, out packetLength);
                // новое подключение
                NetPlayer player = AddPlayer(remoteEP, msg);
                Console.WriteLine("Player {0} trying to connect.", player.PlayerId);
                Trace.WriteLine(string.Format("Player {0} trying to connect.", player.PlayerId));
                new Thread(new ParameterizedThreadStart(SendFirstPacket)).Start(player);
            }
        }

        void SendFirstPacket(object netPlayer)
        {
            NetPlayer player = (NetPlayer)netPlayer;
            ServerConnectionPacket outMsg = new ServerConnectionPacket();
            outMsg.PlayerId = player.PlayerId;
            outMsg.Health = (byte)player.Character.Health;
            outMsg.Speed = player.Character.Speed;
            outMsg.PlayerPosition = (ProtoVector2)player.Character.Position;
            outMsg.Level = (ProtoLevel)_gameState.Level;
            IObjMessenger playerMessenger = new ProtoMessenger<ServerConnectionPacket>(player.Messenger);
            for (int i = 0; _newPlayers.ContainsKey(player.PlayerId) && i < 10; i++)
            {
                playerMessenger.Send<ServerConnectionPacket>(outMsg);
                Thread.Sleep(1000);
            }
            _newPlayers.Remove(player.PlayerId);
            if (_gameState.NetPlayers.ContainsKey(player.PlayerId))
            {
                Console.WriteLine("Player {0} connected.", player.PlayerId);
                Trace.WriteLine(string.Format("Player {0} connected.", player.PlayerId));
            }
            else
            {
                Console.WriteLine("Player {0} failed to connect.", player.PlayerId);
                Trace.WriteLine(string.Format("Player {0} failed to connect.", player.PlayerId));
            }
        }

        void StartReceiving()
        {
            // запускаем поток получения сообщений от клиентов
            stopWatch.Start();
            while (shouldContinue)
            {
                Receive();
            }
        }

        void CheckDisconnections()
        {
            // запускаем поток для проверки активности игроков
            while (shouldContinue)
            {
                HashSet<int> toRemove = new HashSet<int>();
                TimeSpan lastAdmission = stopWatch.Elapsed - connectionTimeout;
                foreach (var player in _gameState.NetPlayers.Values)
                {
                    if (player.LastUpdate < lastAdmission)
                        toRemove.Add(player.PlayerId);
                }
                foreach (var id in toRemove)
                {
                    _gameState.RemoveCharacter(id);
                    Console.WriteLine("Player {0} disconnected.", id);
                    Trace.WriteLine(string.Format("Player {0} disconnected.", id));
                }
                Thread.Sleep(connectionTimeout);
            }
        }

        void Stop()
        {
            shouldContinue = false;
            _inputMessenger.Close();
            _gameState.Stop();
        }

        NetPlayer AddPlayer(IPEndPoint remoteEP, ConnectionMessage msg)
        {
            Character character = _gameState.CreateCharacter(true);
            IMessenger messenger = new UdpMessenger(remoteEP.Address.ToString(), remoteEP.Port);
            NetPlayer player = new NetPlayer(messenger, character, stopWatch.Elapsed);

            _newPlayers.Add(player.PlayerId, player);
            return player;
        }

        void StartSending()
        {
            while (shouldContinue)
            {
                _gameState.Send();
                Thread.Sleep(updateInterval);
            }
        }

        void Receive()
        {
            byte[] data = _inputMessenger.Receive();
            packetsReceivedCount++;
            if (data.Length > maxPacketLength)
            {
                maxPacketLength = data.Length;
            }
            ClientPacket packet = (ClientPacket)_gameState.ProcessData(data);
            if (_newPlayers.ContainsKey(packet.PlayerId)) // убеждаемся, что первый пакет дошёл до клиента
            {
                _gameState.AddNetPlayer(_newPlayers[packet.PlayerId]);
                _newPlayers.Remove(packet.PlayerId);
                _gameState.ProcessData(data); // повторно обрабатываем данные
            }
            _gameState.NetPlayers[packet.PlayerId].LastUpdate = stopWatch.Elapsed;
        }

        ~Communicator()
        {
            if (stopWatch != null)
                stopWatch.Stop();
            Stop();
        }
    }
}
