using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hikigaya_Lux.Core;

namespace Hikigaya_Lux.Logic
{
    class QLogic
    {
        public static readonly Obj_AI_Hero Lux = ObjectManager.Player;
        public static void HikiQx2Target(Obj_AI_Hero enemy)
        {
            if (Spells.Q.GetPrediction(enemy).CollisionObjects.Count == 2)
            {
                if (Spells.Q.GetPrediction(enemy).CollisionObjects[0].IsChampion() && Spells.Q.GetPrediction(enemy).CollisionObjects[1].IsMinion)
                {
                    Spells.Q.Cast(Spells.Q.GetPrediction(enemy).CastPosition);
                }
                if (Spells.Q.GetPrediction(enemy).CollisionObjects[0].IsMinion && Spells.Q.GetPrediction(enemy).CollisionObjects[1].IsChampion())
                {
                    Spells.Q.Cast(Spells.Q.GetPrediction(enemy).CastPosition);
                }
            }
        }
        public static void NormalQ(Obj_AI_Hero enemy)
        {
            if (Spells.Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("q.hit.chance"))
            {
                Spells.Q.Cast(enemy);
            }
        }
        public static void QGeneral(Obj_AI_Hero enemy)
        {
            switch (Helper.Slider("min.q.hit"))
            {
                case 1:
                    NormalQ(enemy);
                    break;
                case 2:
                    HikiQx2Target(enemy);
                    break;
            }
        }
        
    }
}
