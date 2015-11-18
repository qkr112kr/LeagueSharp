using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Champions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utility = HybridCommon.Utility;

namespace HikiCarry_Lee_Sin.Stages
{
    class WardJump : LeeSin
    {
        private static float LastWardJumpTime;
        public static float LastWardCreated;
        public static Vector3 WardCastPosition;

        /// <summary>
        /// Ward Cast Delay & Checker
        /// </summary>
        public static bool WardCastable // CanCastWard
        {
            get { return Game.Time - LastWardJumpTime > 0.50 && Items.GetWardSlot().SpellSlot.IsReady(); }
        }

        /// <summary>
        /// Basic Wardjump
        /// </summary>
        /// <param name="W">W Skill</param>
        /// <param name="position">Jump Position</param>
        public static void HikiJump(Spell W,Vector3 position)
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (WardCastable && W.Instance.Name == "BlindMonkWOne" && W.IsReady())
            {
                ObjectManager.Player.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, position);
                LastWardCreated = Game.Time;
                LastWardJumpTime = Game.Time;
                WardCastPosition = position;
            }
            var ward = ObjectManager.Get<Obj_AI_Base>()
                    .OrderBy(obj => obj.Distance(ObjectManager.Player.ServerPosition))
                    .FirstOrDefault(
                        obj =>
                            obj.IsAlly && !obj.IsMe && obj.Name.IndexOf("ward", StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                            (!(obj.Name.IndexOf("turret", StringComparison.InvariantCultureIgnoreCase) >= 0) &&
                             Vector3.DistanceSquared(Game.CursorPos, obj.ServerPosition) <= 150 * 150));
            if (ward != null)
            {
                W.CastOnUnit(ward);
            }
        }

        public static void Jump(Spell W,Vector3 position)
        {
            
        }
    }
}
