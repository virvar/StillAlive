using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameObjects
{
    public class Pistol : Weapon
    {
        internal override void Strike(Character owner, Character target)
        {
            // после выстрела можно запускать таймер блокировки выстрелов (реализация скорострельности)
            // может быть это не тот уровень, на котором нужно учитывать скорострельность,
            // может её надо учитывать на стадии "button1_Click"?
            target.Health -= (byte)((owner.DamagePerSecond + base.DamagePerSecond) / base.StrikesPerSecond);
        }
    }
}
