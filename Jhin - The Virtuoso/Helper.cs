using System;
using LeagueSharp.Common;
using LeagueSharp;

namespace Jhin___The_Virtuoso
{
    static class Helper
    {
        public static float LastR;
        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static bool IsRActive
        {
            get { return Program.R.Instance.Name == "JhinRShot"; }
        }

        public static bool Rable
        {
            get { return Game.Time - LastR > 2; }
        }


        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[Program.Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }

        public static int AaIndicator(Obj_AI_Hero enemy)
        {
            var aCalculator = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical, ObjectManager.Player.TotalAttackDamage);
            var killableAaCount = enemy.Health / aCalculator;
            var totalAa = (int)Math.Ceiling(killableAaCount);
            return totalAa;
        }
        public static bool IsEnemyImmobile(Obj_AI_Hero target)
        {
            return target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                   target.HasBuffOfType(BuffType.Knockup) ||
                   target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                   target.HasBuffOfType(BuffType.Knockback) ||
                   target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                   target.IsStunned || target.IsChannelingImportantSpell();
        }
        public static bool IsUltimateFirstStage(this Spell spell)
        {
            return spell.Instance.Name.ToLower().Contains("jhinrsight");
        }
        public static bool IsUltimateSecondStage(this Spell spell)
        {
            return spell.Instance.Name.ToLower().Contains("jhinr");
        }
        public static bool IsHasLastAttackBuff(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("jhinpassiveattack");
        }
        public static bool IsHasReloadBuff(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("jhinpassivereload");
        }
        public static bool IsHasJhinTrapSlowDebuff(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("JhinETrapSlow");
        }
        public static bool IsHasJhinUltimateSlow(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("JhinRSlow");
        }
        public static bool IsHasJhinCaptiveAudienceDebuff(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("jhinespotteddebuff");
        }
        public static bool IsHasJhinDeadlyFlourishDebuff(this Obj_AI_Hero unit) 
        {
            return unit.HasBuff("jhinpassivehaste");
        }


    }
}
