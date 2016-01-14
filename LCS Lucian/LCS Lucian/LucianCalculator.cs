using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace LCS_Lucian
{
    class LucianCalculator
    {
        public static float LucianTotalDamage(Obj_AI_Hero enemy)
        {
            if (LucianSpells.Q.IsReady() && Helper.LEnabled("lucian.q.combo"))
            {
                return LucianSpells.Q.GetDamage(enemy);
            }
            if (LucianSpells.W.IsReady() && Helper.LEnabled("lucian.w.combo"))
            {
                return LucianSpells.W.GetDamage(enemy);
            }
            if (LucianSpells.R.IsReady() && Helper.LEnabled("lucian.r.combo"))
            {
                return LucianSpells.R.GetDamage(enemy);
            }
            return 0;
        }
    }
}