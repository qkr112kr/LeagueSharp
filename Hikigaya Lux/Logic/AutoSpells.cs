using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hikigaya_Lux.Core;
using SharpDX;

namespace Hikigaya_Lux.Logic
{
    class AutoSpells
    {
        public static void AutoQIfHit2Target(Obj_AI_Hero enemy)
        {
            if (Spells.Q.GetPrediction(enemy).CollisionObjects.Count == 2)
            {
                if (Spells.Q.GetPrediction(enemy).CollisionObjects[0].IsChampion() && Spells.Q.GetPrediction(enemy).CollisionObjects[1].IsChampion())
                {
                    Spells.Q.Cast(Spells.Q.GetPrediction(enemy).CastPosition);
                }
            }
        }
        public static void AutoQIfEnemyImmobile(Obj_AI_Hero enemy)
        {
            if (Helper.IsEnemyImmobile(enemy) && Spells.Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("q.hit.chance"))
            {
                Spells.Q.Cast(enemy);
            }
        }
        public static void AutoEIfHitXTarget(Obj_AI_Hero enemy)
        {
            if (Spells.E.CastIfWillHit(enemy, Helper.Slider("min.e.hit")) && Spells.E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("e.hit.chance"))
            {
                Spells.E.Cast(enemy);
            }
            if (Helper.LuxE != null && Helper.EInsCheck() == 2)
            {
                Spells.E.Cast();
            }
        }
        public static void AutoEIfEnemyImmobile(Obj_AI_Hero enemy)
        {
            if (Helper.IsEnemyImmobile(enemy) && Spells.E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("e.hit.chance"))
            {
                Spells.E.Cast(enemy);
            }
            if (Helper.LuxE != null && Helper.EInsCheck() == 2)
            {
                Spells.E.Cast();
            }
        }
        public static void AutoRIfEnemyKillable(Obj_AI_Hero enemy)
        {
            if (Spells.R.GetPrediction(enemy).Hitchance >= Helper.HikiChance("r.hit.chance.x") && Calculators.R(enemy) > enemy.Health)
            {
                Spells.R.Cast(enemy);
            }
        }

        public static void KillStealWithQ(Obj_AI_Hero enemy)
        {
            if (Spells.Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("q.hit.chance") && Calculators.Q(enemy) > enemy.Health)
            {
                Spells.Q.Cast(enemy);
            }
        }
        public static void KillStealWithE(Obj_AI_Hero enemy)
        {
            if (Spells.E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("e.hit.chance") && Calculators.E(enemy) > enemy.Health)
            {
                Spells.E.Cast(enemy);
            }
            if (Helper.LuxE != null && Helper.EInsCheck() == 2)
            {
                Spells.E.Cast();
            }
        }
    }
}
