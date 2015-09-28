using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects;
using GameObjects.DrawableClasses;
using GameProcess.Actors.Client;
using Microsoft.Xna.Framework;
using Virvar.Net;
using System.Collections.Concurrent;
using GameObjects.MoveBehaviors;
using System.IO;
using GameProcess.Actors;

namespace GameProcess
{
    public class GameStateClient : GameState
    {
        private List<IClientPacketHandler> _packetHandlers; // агенты, обрабатывающие сообщения

        public RealCharacter Player { get; set; } // (клиент)
        public ConcurrentDictionary<int, RealCharacter> Players { get; set; } // игроки + боты
        public NetPlayer NetPlayer { get; set; } // объект, отчечающий за передачу пакетов (клиент)

        public GameStateClient(NetPlayer netPlayer = null)
        {
            CollisionActor collisionsActor = new CollisionActor(this);
            MovesActor movesActor = new MovesActor(this, collisionsActor);
            RotationsActor rotationsActor = new RotationsActor(this);
            HitsActor hitsActor = new HitsActor(this);
            ScoresActor scoresActor = new ScoresActor(this);
            ConsoleCommandsActor consoleCommandsActor = new ConsoleCommandsActor(this);
            AddActor(movesActor);
            AddActor(rotationsActor);
            AddActor(hitsActor);
            AddActor(scoresActor);
            ConnectionsCheckActor connectionsCheckerActor = new ConnectionsCheckActor(this);
            _packetHandlers = new List<IClientPacketHandler>();
            _packetHandlers.Add(connectionsCheckerActor); // обязательно должен быть первым
            _packetHandlers.Add(movesActor);
            _packetHandlers.Add(new HealthActor(this));
            _packetHandlers.Add(rotationsActor);
            _packetHandlers.Add(hitsActor);
            _packetHandlers.Add(scoresActor);
            _packetHandlers.Add(consoleCommandsActor);

            Players = new ConcurrentDictionary<int, RealCharacter>();

            // todo: netPlayer всегда Null
            this.NetPlayer = netPlayer;
        }

        /// <summary>
        /// Создаём персонажа. Создаётся событие CharacterAdded.
        /// </summary>
        /// <param name="playerId">Id персонажа.</param>
        public void CreateCharacter(int playerId)
        {
            RealCharacter character = new RealCharacter(playerId);
            character.MoveBehavior = new EmptyBehavior(character);
            Players.TryAdd(character.Id, character);
            NotifyCharacterAdded(character);
        }

        public override void RemoveCharacter(int playerId)
        {
            RealCharacter removedCharacter;
            Players.TryRemove(playerId, out removedCharacter);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var player in Players.Values)
            {
                player.Update(gameTime);
            }
        }

        /// <summary>
        /// Обрабатывает пришедшие данные.
        /// </summary>
        /// <param name="data">Пришедшие данные.</param>
        public override Packet ProcessData(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            ServerPacket pack = new ServerPacket();
            pack.Parse(data);
            // игнорируем пакет, если старый
            if (pack.Sequence < NetPlayer.ClientPacket.Ack)
            {
                return pack;
            }
            foreach (var handler in _packetHandlers)
            {
                handler.Receive(pack);
            }
            NetPlayer.ClientPacket.Ack = pack.Sequence;
            return pack;
        }
        /// <summary>
        /// Отправляет данные клиенту\серверу.
        /// </summary>
        public override void Send()
        {
            // отправляем серверу
            NetPlayer.SendToServer(NetPlayer.ClientPacket.Build());
        }
        public override void Stop()
        {
            // todo: надо проверить, выполняется ли когда-нибудь это условие
            if (NetPlayer != null)
            {
                NetPlayer.Close();
            }
        }
    }
}
