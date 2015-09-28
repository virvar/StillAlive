using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GameObjects;
using GameObjects.DrawableClasses;
using GameObjects.MoveBehaviors;
using Microsoft.Xna.Framework;
using Virvar.Net;
using GameProcess;

namespace GameServer
{
    public class GameLogic
    {
        public static GameLogic Instance;

        private GameStateServer _gameState;
        private Stopwatch _stopwatch;
        private TimeSpan _lastUpdTime;
        private int _updateInterval = 40;
        private bool _shouldContinue;

        public static void CreateInstance(GameStateServer gameState)
        {
            Instance = new GameLogic(gameState);
        }

        private GameLogic(GameStateServer gameState)
        {
            _gameState = gameState;
            Initialize(gameState.Level);
        }

        public void Initialize(Level level)
        {
            // monster
            {
                RealCharacter monster = _gameState.CreateCharacter(false);
                //monster.MoveBehavior = new MonsterMoveAStar(monster, level);
                monster.Position = new Vector2(1300, 1500);
                monster.Speed = 200;
            }
            //// monster
            //{
            //    RealCharacter monster = _gameState.CreateCharacter(false);
            //    //monster.moveBehavior = new StupidMonsterMove(monster);
            //    monster.MoveBehavior = new EmptyBehavior(monster);
            //    monster.Position = new Vector2(200, 170);
            //    monster.Speed = 25;
            //}
            _shouldContinue = true;
            Start();
        }

        private void Start()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _lastUpdTime = TimeSpan.Zero;
            new Thread(Update).Start();
        }

        public void Update()
        {
            while (_shouldContinue)
            {
                GameTime gameTime = new GameTime(_stopwatch.Elapsed, _stopwatch.Elapsed - _lastUpdTime);
                _lastUpdTime = _stopwatch.Elapsed;
                _gameState.Update(gameTime);
                Thread.Sleep(_updateInterval);
            }
        }

        public void Stop()
        {
            _shouldContinue = false;
        }

        ~GameLogic()
        {
            _shouldContinue = false;
            _stopwatch.Stop();
        }
    }
}
