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
    public enum Owner
    {
        Client,
        Server
    }

    public abstract class GameState
    {
        private List<IActor> _actors; // агенты, отвечающие за свою область

        public Level Level { get; private set; } // уровень
        internal Grid Grid { get; private set; } // сетка на карте
        public ConcurrentDictionary<int, Score> Scores { get; set; } // таблица очков

        public event Action<RealCharacter> CharacterAdded;

        protected GameState()
        {
            _actors = new List<IActor>();
            Grid = new Grid(); // сетка на карте
            Scores = new ConcurrentDictionary<int, Score>();
        }

        protected void AddActor(IActor actor)
        {
            _actors.Add(actor);
        }

        public void SetLevel(Level level)
        {
            this.Level = level;
            Grid.SetLevel(level);
        }

        protected void NotifyCharacterAdded(RealCharacter character)
        {
            CharacterAdded(character);
        }

        public abstract void RemoveCharacter(int playerId);

        public virtual void Update(GameTime gameTime)
        {
            foreach (var actor in _actors)
            {
                actor.Update(gameTime);
            }
        }

        /// <summary>
        /// Обрабатывает пришедшие данные.
        /// </summary>
        /// <param name="data">Пришедшие данные.</param>
        public abstract Packet ProcessData(byte[] data);

        /// <summary>
        /// Отправляет данные клиенту\серверу.
        /// </summary>
        public abstract void Send();

        public virtual void Stop() { }

        ~GameState()
        {
            Stop();
        }
    }
}
