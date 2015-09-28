using GameObjects;
using GameObjects.DrawableClasses;
using GameProcess;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Threading;
using Virvar.Net;

namespace StillAlive
{
    class Communicator
    {
        // todo: избавиться от связи с Battlefield
        private Battlefield _battlefield;
        private GameStateClient _gameState;
        private int _clientPort = 7778;
        private int _receiveTimeout = 5000;

        private IMessenger _messenger;
        private int _updateInterval = 40;
        private bool _shouldContinue = true;

        public Communicator(Settings settings, Battlefield battlefield, GameStateClient gameState, out RealCharacter player)
        {
            _battlefield = battlefield;
            _gameState = gameState;
            // подключаемся к серверу
            ServerConnectionPacket connMsg = Connect(settings, ref _clientPort);
            if (connMsg == null)
            {
                _battlefield.Game.Exit();
            }
            _messenger = new UdpMessenger(settings.ServerAddress, settings.ServerPort, _clientPort, _receiveTimeout);
            _gameState.SetLevel((Level)connMsg.Level);
            // обновление состояния игрока
            _gameState.CreateCharacter(connMsg.PlayerId);
            player = _gameState.Players[connMsg.PlayerId];
            player.Position = (Vector2)connMsg.PlayerPosition;
            player.Health = connMsg.Health;
            player.Speed = connMsg.Speed;
            player.MoveBehavior = new PlayerMove1(player);
            _gameState.Player = player;

            _gameState.NetPlayer = new NetPlayer(_messenger, player);
            new Thread(StartReceiving).Start();
            new Thread(StartSending).Start();
        }

        private ServerConnectionPacket Connect(Settings settings, ref int port)
        {
            ServerConnectionPacket connAnswer = null;
            ProtoMessenger<ConnectionMessage> connMessenger = null;
            for (int j = 0; j < 20; j++)
            {
                try
                {
                    connMessenger = new ProtoMessenger<ConnectionMessage>(new UdpMessenger(settings.ServerAddress, settings.ServerConnectionPort, port));
                    break;
                }
                catch
                {
                    port++;
                }
            }
            if (connMessenger == null)
            {
                Trace.WriteLine("Failed to connect.");
            }
            else
            {
                ConnectionMessage connMsg = new ConnectionMessage(port);
                for (int i = 0; i < 10; i++)
                {
                    connMessenger.Send(connMsg);
                    try
                    {
                        connAnswer = (ServerConnectionPacket)connMessenger.Receive<ServerConnectionPacket>();
                        break;
                    }
                    catch
                    {
                        // временно
                        _battlefield.PacketLength = -1;
                    }
                }
                connMessenger.Close();
                if (connAnswer != null)
                {
                    Trace.WriteLine("Connection succesful.");
                }
                else
                {
                    Trace.WriteLine("Failed to connect.");
                }
            }
            return connAnswer;
        }

        private void StartReceiving()
        {
            while (_shouldContinue)
            {
                Receive();
            }
        }

        public void Stop()
        {
            _shouldContinue = false;
            _battlefield.PacketLength = 0;
            _gameState.Stop();
            _messenger.Close();
        }

        //private void RemovePlayer(byte playerId)
        //{
        //    _battlefield.RemoveCharacter(playerId);
        //}

        private void StartSending(object state)
        {
            while (_shouldContinue)
            {
                _gameState.Send();
                Thread.Sleep(_updateInterval);
            }
        }

        private void Receive()
        {
            byte[] data = _messenger.Receive();
            ServerPacket packet = (ServerPacket)_gameState.ProcessData(data);
            if (data == null)
            {
                return;
            }
            _battlefield.PacketLength = data.Length;
        }
    }
}
