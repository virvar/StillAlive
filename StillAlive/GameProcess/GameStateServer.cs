using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameObjects;
using GameObjects.DrawableClasses;
using GameProcess.Actors.Server;
using Microsoft.Xna.Framework;
using Virvar.Net;
using System.Collections.Concurrent;
using GameObjects.MoveBehaviors;
using System.IO;
using GameProcess.Actors;

namespace GameProcess
{
    public class GameStateServer : GameState
    {
        private static int _curCharacterId = 1;

        private ConnectionsCheckActor _connectionsCheckerActor;
        private List<IServerPacketHandler> _packetHandlers;
        private LinkedList<GameMoment> _moments = new LinkedList<GameMoment>();
        private int _momentsLength = 30;

        public ConcurrentDictionary<int, RealCharacter> Players { get; set; }
        public HashSet<RealCharacter> Bots { get; set; }
        public ConcurrentDictionary<byte, NetPlayer> NetPlayers { get; set; }

        public GameStateServer(string mapFile)
        {
            LoadMap(mapFile);
            CollisionActor collisionsActor = new CollisionActor(this);
            MovesActor movesActor = new MovesActor(this, collisionsActor);
            RotationsActor rotationsActor = new RotationsActor(this);
            HitsActor hitsActor = new HitsActor(this);
            ScoresActor scoresActor = new ScoresActor(this);
            ConsoleCommandsActor consoleCommandsActor = new ConsoleCommandsActor(this);
            AddActor(movesActor);
            AddActor(hitsActor);
            _connectionsCheckerActor = new ConnectionsCheckActor(this);
            _packetHandlers = new List<IServerPacketHandler>();
            _packetHandlers.Add(_connectionsCheckerActor); // must be first
            _packetHandlers.Add(movesActor);
            _packetHandlers.Add(rotationsActor);
            _packetHandlers.Add(hitsActor);
            _packetHandlers.Add(scoresActor);
            _packetHandlers.Add(consoleCommandsActor);

            Players = new ConcurrentDictionary<int, RealCharacter>();
            Bots = new HashSet<RealCharacter>();

            NetPlayers = new ConcurrentDictionary<byte, NetPlayer>();
        }

        private void LoadMap(string file)
        {
            SetLevel(ParseLevel(file));
            Console.WriteLine("Map loaded.");
        }

        private Level ParseLevel(string name)
        {
            using (StreamReader sr = new StreamReader(name))
            {
                string line;
                line = sr.ReadLine();
                int width = int.Parse(line.Split('=')[1]);
                line = sr.ReadLine();
                int height = int.Parse(line.Split('=')[1]);
                Level level = new Level(width, height);
                while ((line = sr.ReadLine()) != null)
                {
                    // respawn points
                    if (line == "[RespawnPoint]")
                    {
                        line = sr.ReadLine();
                        int pX = int.Parse(line.Split('=')[1]);
                        line = sr.ReadLine();
                        int pY = int.Parse(line.Split('=')[1]);
                        level.RespawnPoints.Add(new Vector2(pX, pY));
                    }
                    // map objects
                    if (line == "[Solid]")
                    {
                        DrawableSolid solid = new DrawableSolid();
                        line = sr.ReadLine();
                        int w = int.Parse(line.Split('=')[1]);
                        line = sr.ReadLine();
                        int h = int.Parse(line.Split('=')[1]);
                        solid.Size = new Point(w, h);
                        line = sr.ReadLine();
                        int pX = int.Parse(line.Split('=')[1]);
                        line = sr.ReadLine();
                        int pY = int.Parse(line.Split('=')[1]);
                        solid.Position = new Vector2(pX, pY);
                        line = sr.ReadLine();
                        solid.Angle = float.Parse(line.Split('=')[1]);
                        level.Solids.Add(solid);
                    }
                }
                if (level.RespawnPoints.Count == 0)
                {
                    level.CreateDefaultRespawnPositions();
                }
                return level;
            }
        }

        public RealCharacter CreateCharacter(bool isPlayer)
        {
            RealCharacter character = new RealCharacter(_curCharacterId++);
            Scores.TryAdd((byte)character.Id, new Score());
            character.Health = Character.MaxHealth;
            character.HealthRegen = 1;
            character.Speed = 400;
            character.DamagePerSecond = 120;
            character.Mana = Character.MaxMana;
            character.ManaRegen = 10;
            character.Position = Level.GetRespawnPosition();
            if (isPlayer)
            {
                character.MoveBehavior = new EmptyBehavior(character);
                Players.TryAdd(character.Id, character);
                lock (Bots)
                {
                    foreach (Character monster in Bots)
                    {
                        if (monster.MoveBehavior is StupidMonsterMove)
                        {
                            (monster.MoveBehavior as StupidMonsterMove).AddTarget(character);
                        }
                    }
                }
            }
            else
            {
                // todo: temp, move behavior shouldn't be initialized here
                character.MoveBehavior = new MonsterMoveAStar(character, Level);
                lock (Bots)
                {
                    Bots.Add(character);
                    foreach (Character player in Players.Values)
                    {
                        if (character.MoveBehavior is StupidMonsterMove)
                        {
                            (character.MoveBehavior as StupidMonsterMove).AddTarget(player);
                        }
                    }
                }
            }
            NotifyCharacterAdded(character);
            NotifyOthersNewPlayerCreated(character);
            return character;
        }

        private void NotifyOthersNewPlayerCreated(RealCharacter character)
        {
            foreach (var netplayer in NetPlayers.Values)
            {
                netplayer.ServerPacket.PlayersConnections.TryAdd((byte)character.Id, ConnectionState.Connected);
            }
        }

        public void AddNetPlayer(NetPlayer netPlayer)
        {
            NetPlayers.TryAdd(netPlayer.PlayerId, netPlayer);
            _connectionsCheckerActor.AddPlayersListToPacket(netPlayer);
        }

        public override void RemoveCharacter(int playerId)
        {
            lock (Bots)
            {
                foreach (Character monster in Bots)
                {
                    if (monster.MoveBehavior is StupidMonsterMove)
                        (monster.MoveBehavior as StupidMonsterMove).RemoveTarget(Players[playerId]);
                }
            }
            RealCharacter removedCharacter;
            Players.TryRemove(playerId, out removedCharacter);
            NetPlayer removedPlayer;
            NetPlayers.TryRemove((byte)playerId, out removedPlayer);
            Score removedScore;
            Scores.TryRemove(playerId, out removedScore);
            _connectionsCheckerActor.SetPlayerDisconnected((byte)playerId);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // создаём "слепок"
            GameMoment moment = new GameMoment(gameTime, Players.Count + Bots.Count);
            moment.Add(Players.Values);
            moment.Add(Bots);
            _moments.AddLast(moment);
            while (_moments.Count > _momentsLength)
            {
                _moments.RemoveFirst();
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
            ClientPacket pack = new ClientPacket();
            pack.Parse(data);
            if (!NetPlayers.ContainsKey(pack.PlayerId))
            {
                return pack;
            }
            // ignore out of date packets
            if (pack.Sequence < NetPlayers[pack.PlayerId].ServerPacket.Ack)
            {
                return pack;
            }
            foreach (var handler in _packetHandlers)
            {
                handler.Receive(pack);
            }
            NetPlayers[pack.PlayerId].ServerPacket.Ack = pack.Sequence;
            return pack;
        }

        public override void Send()
        {
            foreach (var netplayer in NetPlayers.Values)
            {
                netplayer.SendToClient(netplayer.ServerPacket.Build());
                //netplayer.ServerPacket.Clear();
            }
        }

        public override void Stop()
        {
            // todo: надо проверить, выполняется ли когда-нибудь это условие
            if (NetPlayers != null)
            {
                NetPlayers.Values.ToList().ForEach(p => p.Close());
            }
        }
    }
}
