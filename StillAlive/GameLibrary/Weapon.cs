using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameObjects
{
    public abstract class Weapon
    {
        /*
         * Продублировал поля Player'а, думаю, так и надо, т.е. есть характеристики персонажа, и есть характеристики оружия
         */

        /// <summary>
        /// Точность (будущее)
        /// </summary>
        public double Accuracy { get; set; }
        /// <summary>
        /// Шанс критической атаки (будущее)
        /// </summary>
        public double CriticalChance { get; set; }

        /// <summary>
        /// Базовый урон
        /// </summary>
        public int DamagePerSecond { get; set; }
        /// <summary>
        /// Ударов в секунду
        /// </summary>
        public int StrikesPerSecond { get; set; }

        internal abstract void Strike(Character owner, Character target);
    }
}
