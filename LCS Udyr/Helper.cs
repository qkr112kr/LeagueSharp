using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

namespace LCS_Udyr
{
    static class Helper
    {
        public static bool IsTiger(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("udyrtigerpunch");
        }
        public static bool IsTurtle(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("udyrturtleactivation");
        }
        public static bool IsBear(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("udyrbearactivation");
        }
        public static bool IsPhoenix(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("udyrphoenixactivation"); // udyrbearstuncheck
        }
        public static bool HasBearPassive(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("udyrbearstuncheck"); // 
        }
    }
}
