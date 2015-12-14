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
    class JungleSteal
    {
        public static void DragonSteal(Obj_AI_Minion dragon)
        {
            var time = (int)(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.SpellTotalTime * 1000) - 100 + Game.Ping / 2;

            HealthPrediction.GetHealthPrediction(dragon, time, (int) Spells.R.Delay);

            if (b)
            {
                
            }
        }
    }
}
