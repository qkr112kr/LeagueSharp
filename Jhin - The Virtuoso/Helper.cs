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

        private static readonly double[] QDamage = { 60, 85, 110, 135, 160 };
        private static readonly double[] QSubDamage = { 0.3, 0.35, 0.4, 0.45, 0.5 };
        private static readonly double[] WDamage = { 50, 85, 120, 155, 190 };
        private static readonly double[] EDamage = { 20, 80, 140, 200, 260 };
        private static readonly double[] RDamage = { 50, 125, 200 };



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
            var aCalculator = ObjectManager.Player.CalcDamage(enemy, (Damage.DamageType) LeagueSharp.DamageType.Physical, ObjectManager.Player.TotalAttackDamage);
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

        public static float Damage(this Spell spell, Obj_AI_Base enemy)
        {
            if (!spell.IsReady())
            {
                return 0f;
            }
            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    var qdamage = (QDamage[spell.Level - 1] +
                                   QSubDamage[spell.Level - 1] * ObjectManager.Player.TotalAttackDamage) +
                                  (0.6*ObjectManager.Player.TotalMagicalDamage);
                    return (float)ObjectManager.Player.CalcDamage(enemy, LeagueSharp.Common.Damage.DamageType.Physical, qdamage);
                case SpellSlot.W:
                    var wdamage = WDamage[spell.Level - 1] + ObjectManager.Player.TotalAttackDamage * 0.7;
                    return (float)ObjectManager.Player.CalcDamage(enemy, LeagueSharp.Common.Damage.DamageType.Physical, wdamage);
                case SpellSlot.E:
                    var edamage = (EDamage[spell.Level - 1] + ObjectManager.Player.TotalAttackDamage * 1.2) +
                                  (1.0*ObjectManager.Player.TotalMagicalDamage);
                    return (float)ObjectManager.Player.CalcDamage(enemy, LeagueSharp.Common.Damage.DamageType.Physical, edamage);
                case SpellSlot.R:
                    return (float)ObjectManager.Player.CalcDamage(enemy, LeagueSharp.Common.Damage.DamageType.Physical, (new[] { 50, 125, 200 }[Program.R.Level] + ObjectManager.Player.TotalAttackDamage * 0.25) * ((100f - enemy.HealthPercent) * 0.02f + (IsLastRShoot ? 1f : 0f)));
            }
            return 0;
        }
        private static bool IsLastRShoot
        {
            get { return IsRShootCastable && ObjectManager.Player.HasBuff("jhinrlast"); }
        }
        private static bool IsRShootCastable
        {
            get { return Program.R.Instance.Name == "JhinRShot"; }
        }

        public static float GetUltimateDamage(this Obj_AI_Hero enemy)
        {
            return Program.R.IsReady()
                ? (float)
                    ObjectManager.Player.CalcDamage(enemy, LeagueSharp.Common.Damage.DamageType.Physical,
                        (new[] {50, 125, 200}[Program.R.Level] + ObjectManager.Player.TotalAttackDamage*0.25)*
                        ((100f - enemy.HealthPercent)*0.02f + (IsLastRShoot ? 1f : 0f)))
                : 0f;
        }

        public static float ComboDamage(Obj_AI_Hero enemy)
        {
            var damage = 0f;
            if (Program.Q.IsReady())
            {
                damage += Program.Q.Damage(enemy);
            }
            if (Program.W.IsReady())
            {
                damage += Program.W.Damage(enemy);
            }
            if (Program.E.IsReady())
            {
                damage += Program.E.Damage(enemy);
            }
            if (Program.R.IsReady())
            {
                damage += Program.R.Damage(enemy) * 3;
            }
            return damage;
        }

        public static bool IsLastShoot(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("jhinrlast");
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
