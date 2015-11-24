using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Champions;
using HikiCarry_Lee_Sin.Core;
using LeagueSharp;
using LeagueSharp.Common;

namespace HikiCarry_Lee_Sin.Stages
{
    class WardjumpFlashInsec : LeeSin
    {
        public static readonly float WardJumpRange = 600;
        public static readonly float FlashRange = 600;
        public static readonly float QRange = Spells[Q].Range;
        public static readonly float Q2Range = Spells[Q2].Range;
        public static readonly float TotalRange = WardJumpRange + QRange;
        public static readonly Obj_AI_Hero LeeSin = ObjectManager.Player;
        private static readonly SpellSlot FlashSlot = ObjectManager.Player.GetSpellSlot("SummonerFlash");


        public static bool Flashable // CanCastWard
        {
            get
            {
                return FlashSlot.IsReady() && FlashSlot != SpellSlot.Unknown && 
                    LeeSin.Spellbook.CanUseSpell(FlashSlot) == SpellState.Ready ;
            }
        }

        public static void WardJumpFlashInsec(Obj_AI_Hero enemy)
        {
            if (Config.Item("insec.whitelist." + enemy.ChampionName).GetValue<bool>() && HybridCommon.Utility.GetPriority(enemy.ChampionName) > 2)
            {
                if (ObjectManager.Player.Distance(enemy.Position) > Spells[Q].Range &&
                ObjectManager.Player.Distance(enemy.Position) < Spells[Q].Range + 600 && WardJump.WardCastable)
                {
                    WardJump.HikiJump(Spells[W], Helper.WardJumpToEnemy(enemy, 600));
                }
                if (enemy.Distance(LeeSin.Position) < Spells[Q].Range && Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High)
                {
                    Spells[Q].Cast(enemy);
                }
                if (Helper.QTwo(Spells[Q]) && Spells[Q2].IsReady())
                {
                    Spells[Q2].Cast();
                }
                if (Flashable && LeeSin.Distance(Helper.InsecPositions()) < FlashRange)
                {
                    Utility.DelayAction.Add(Helper.SliderCheck("flash.insec.flash.delay"),
                        () => LeeSin.Spellbook.CastSpell(FlashSlot, Helper.InsecPositions()));
                }
                if (LeeSin.Spellbook.CanUseSpell(FlashSlot) == SpellState.Cooldown && !Spells[Q].IsReady() &&
                    LeeSin.Distance(enemy.Position) < Spells[R].Range)
                {
                    Spells[R].Cast(enemy);
                }
            }  
        }

        public static bool WardJumpFlashText(Obj_AI_Hero enemy)
        {
            if (ObjectManager.Player.Distance(enemy.Position) > Spells[Q].Range &&
                ObjectManager.Player.Distance(enemy.Position) < Spells[Q].Range + 600 && WardJump.WardCastable && Flashable && Spells[R].IsReady() && Spells[Q].IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }


    }
}
