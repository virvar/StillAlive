using Microsoft.Xna.Framework;

namespace GameObjects
{
    public class Character : ISolid
    {
        public int Id;
        /// <summary>
        /// Точность (будущее)
        /// </summary>
        public double Accuracy;
        /// <summary>
        /// Шанс критической атаки (будущее)
        /// </summary>
        public double CriticalChance;
        /// <summary>
        /// Базовый урон в секунду
        /// </summary>
        public int DamagePerSecond;
        /// <summary>
        /// Здоровье персонажа
        /// </summary>
        public float Health { get; set; }
        /// <summary>
        /// Скорость восстановления здоровья (единиц в секунду).
        /// </summary>
        public float HealthRegen { get; set; }
        /// <summary>
        /// Максимальное здоровье.
        /// </summary>
        public const float MaxHealth = 100;
        /// <summary>
        /// Защита
        /// </summary>
        public int Defence { get; set; }
        /// <summary>
        /// Поведение движения персонажа
        /// </summary>
        public IMoveBehavior MoveBehavior { get; set; }
        /// <summary>
        /// Координаты персонажа
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Скорость передвижения
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// Оружие
        /// </summary>
        public Weapon Weapon
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        /// <summary>
        /// Направление взгляда персонажа (в радианах).
        /// </summary>
        public float Angle { get; set; }
        /// <summary>
        /// Диаметр персонажа.
        /// </summary>
        private const int Diameter = 32;
        /// <summary>
        /// Размер зоны столкновения.
        /// </summary>
        public Point CollisionOffset { get { return new Point(Diameter, Diameter); } }
        /// <summary>
        /// Стреляет ли персонаж.
        /// </summary>
        public bool IsShooting { get; set; }
        /// <summary>
        /// Время стрельбы.
        /// </summary>
        public float ShootingTime { get; set; }
        /// <summary>
        /// Мана. Тратится на стрельбу.
        /// </summary>
        public float Mana { get; set; }
        /// <summary>
        /// Скорость восстановления маны (единиц в секунду).
        /// </summary>
        public float ManaRegen { get; set; }
        /// <summary>
        /// Максимальный уровень маны.
        /// </summary>
        public const float MaxMana = 100;
        /// <summary>
        /// Имя игрока.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Вершины квадрата, ограничивающего игрока.
        /// </summary>
        public Vector2[] Vertices
        {
            get
            {
                Vector2[] retval = new Vector2[4];
                retval[0] = new Vector2(Position.X - (Diameter >> 1), Position.Y - (Diameter >> 1));
                retval[1] = new Vector2(Position.X + (Diameter >> 1), Position.Y - (Diameter >> 1));
                retval[2] = new Vector2(Position.X + (Diameter >> 1), Position.Y + (Diameter >> 1));
                retval[3] = new Vector2(Position.X - (Diameter >> 1), Position.Y + (Diameter >> 1));
                return retval;
            }
        }

        public Character(int id)
        {
            this.Id = id;
            Name = "Player" + Id;
        }

        public void Fight(Character enemy)
        {
            Weapon.Strike(this, enemy);
        }

        public void Move(GameTime gameTime)
        {
            MoveBehavior.Move(gameTime);
        }
    }
}
