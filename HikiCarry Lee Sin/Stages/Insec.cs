using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Champions;
using HikiCarry_Lee_Sin.Core;
using HybridCommon;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace HikiCarry_Lee_Sin.Stages
{
    public class Insec : LeeSin
    {
        /// <summary>
        /// Ally Insec
        /// </summary>
        /// <param name="searchRange">enemy and ally search range</param>
        /// <param name="distance">insec distance</param>
        /// <returns></returns>
        public static Vector3 AllyInsec(float searchRange, float distance)
        {
            var enemy = HeroManager.Enemies.Where(x => x.IsValidTarget(searchRange)).OrderBy(o => o.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            var ally = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.Distance(ObjectManager.Player.Position) < searchRange && x.IsAlly && !x.IsMe).OrderBy(o => o.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            if (ally != null)
            {
                return enemy.Position.To2D().Extend(ally.Position.To2D(), -distance).To3D();
            }
            else
            {
                return enemy.Position.To2D().Extend(ObjectManager.Player.Position.To2D(), -distance).To3D();
            }
        }
        /// <summary>
        /// Tower Insec
        /// </summary>
        /// <param name="searchRange">enemy and ally search range</param>
        /// <param name="distance">insec distance</param>
        /// <returns></returns>
        public static Vector3 TowerInsec(int searchRange, int distance)
        {
            var enemy = HeroManager.Enemies.Where(x => x.IsValidTarget(searchRange)).OrderBy(o => o.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            var tower = ObjectManager.Get<Obj_AI_Turret>().Where(x => x.IsAlly && !x.IsEnemy).OrderBy(o => o.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            if (tower != null)
            {

                return enemy.Position.To2D().Extend(tower.Position.To2D(), -distance).To3D();
            }
            else
            {
                return enemy.Position.To2D().Extend(ObjectManager.Player.Position.To2D(), -distance).To3D();
            }
           
        }
        /// <summary>
        /// Cursor Insec
        /// </summary>
        /// <param name="searchRange">enemy search range</param>
        /// <param name="distance">insec distance</param>
        /// <returns></returns>
        
        public static Vector3 CursorInsec(float searchRange, float distance)
        {
            var enemy = HeroManager.Enemies.Where(x => x.IsValidTarget(searchRange)).OrderBy(o => o.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            return enemy.Position.To2D().Extend(Game.CursorPos.To2D(), -distance).To3D();
        }

        public static void HikiInsec(Obj_AI_Hero enemy)
        {
            if (Config.Item("insec.whitelist." + enemy.ChampionName).GetValue<bool>())
            {
                if (Spells[Q].CanCast(enemy) && Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High && Spells[R].IsReady())
                {
                    Spells[Q].Cast(enemy);
                }
                if (Spells[Q].CanCast(enemy) && Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High && Spells[R].IsReady())
                {
                    Spells[Q].Cast(enemy);
                }
                if (Spells[Q].Instance.Name != "BlindMonkQOne" && Spells[Q2].IsReady())
                {
                    Spells[Q2].Cast();
                }
                if (!Spells[Q].IsReady() && !Spells[Q2].IsReady() && Spells[W].IsReady() && Spells[W].Instance.Name == "BlindMonkWOne")
                {
                    Helper.InsecTo(enemy, Spells[W]);
                }
                if (!Spells[Q].IsReady() && !Spells[Q2].IsReady() && Spells[W].Instance.Name != "BlindMonkWOne" && Spells[R].CanCast(enemy))
                {
                    Spells[R].Cast(enemy);
                }
            }   
        }

        

    }
}
