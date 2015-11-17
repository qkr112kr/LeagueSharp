using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace HikiCarry_Lee_Sin.Core
{
    class Calculator
    {
        private static readonly double[] Q2Dmg = { 50, 80, 110, 140, 170 };
        private static readonly double[] QDmg = { 50, 80, 110, 140, 170 };
        public static float Q2Damage(Obj_AI_Hero target,Spell Q)
        {
            if (target == null)
            {
                return 0f;
            }
            var damage = 0f;
            if (target.HasBuff("BlindMonkQOne") && Q.Instance.Name == "blindmonkqtwo")
            {
                var total = Q2Dmg[Q.Level - 1] + 0.9*ObjectManager.Player.FlatPhysicalDamageMod +
                            0.08*(target.MaxHealth - target.Health);
                damage += (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, total);
            }
            return (float)damage;
        }
        public static float Q2DamageMinion(Obj_AI_Base minion, Spell Q)
        {
            if (minion == null)
            {
                return 0f;
            }
            var damage = 0f;
            if (minion.HasBuff("BlindMonkQOne") && Q.Instance.Name == "blindmonkqtwo")
            {
                var total = Q2Dmg[Q.Level - 1] + 0.9 * ObjectManager.Player.FlatPhysicalDamageMod +
                            0.08 * (minion.MaxHealth - minion.Health);
                damage += (float)ObjectManager.Player.CalcDamage(minion, Damage.DamageType.Physical, total);
            }
            return (float)damage;
        }
        public static float QDamage(Obj_AI_Base minion, Spell Q)
        {
            if (minion == null)
            {
                return 0f;
            }
            var damage = 0f;
            if (Helper.QOne(Q))
            {
                var total = QDmg[Q.Level - 1] + 0.9*ObjectManager.Player.FlatPhysicalDamageMod;
                damage += (float)ObjectManager.Player.CalcDamage(minion, Damage.DamageType.Physical, total);
            }
            return (float)damage;
        }
    }
}
