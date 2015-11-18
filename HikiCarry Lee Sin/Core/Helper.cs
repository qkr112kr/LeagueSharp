using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Champions;
using HikiCarry_Lee_Sin.Stages;
using HybridCommon;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace HikiCarry_Lee_Sin.Core
{
    class Helper : LeeSin 
    {
        public static int SliderCheck(string menuName)
        {
            return Config.Item(menuName).GetValue<Slider>().Value;
        }
        public static bool InsecEnabled()
        {
            if (Config.Item("insec.status").GetValue<StringList>().SelectedIndex == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void InsecTo(Obj_AI_Hero enemy,Spell W)
        {
            switch (Config.Item("insec.to").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                     
                    if (ObjectManager.Player.Distance(Insec.AllyInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"))) < W.Range)
                    {
                        WardJump.HikiJump(W, Insec.AllyInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance")));
                    }
                    break;
                case 1:
                    if (ObjectManager.Player.Distance(Insec.TowerInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"))) < W.Range)
                    {
                        WardJump.HikiJump(W, Insec.TowerInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance")));
                    }
                    break;
                case 2:
                    if (ObjectManager.Player.Distance(Insec.CursorInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"))) < W.Range)
                    {
                        WardJump.HikiJump(W, Insec.CursorInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance")));
                    }
                    break;
            }
        }

        public static void InsecDrawLine(Color color)
        {
            if (InsecEnabled())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(SliderCheck("max.enemy.count.distance")) && x.IsVisible && !x.IsDead && !x.IsZombie))
                {
                    if (Config.Item("insec.whitelist." + enemy.ChampionName).GetValue<bool>())
                    {
                        switch (Config.Item("insec.to").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                Drawing.DrawLine(Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(Insec.AllyInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"))), 2, color);
                                break;
                            case 1:
                                Drawing.DrawLine(Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(Insec.TowerInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"))), 2, color);
                                break;
                            case 2:
                                Drawing.DrawLine(Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(Insec.CursorInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"))), 2, color);
                                break;
                        }
                    }
                }
            }
        }

        public static void WardDraw(Color color)
        {
            foreach (var ward in ObjectManager.Get<Obj_AI_Base>().Where(x => x.Name.IndexOf("ward", StringComparison.InvariantCultureIgnoreCase) > 0 && x.IsAlly && x.IsVisible))
            {
                Render.Circle.DrawCircle(ward.Position,50,color);
            }
        }
        public static void InsecDrawCircle(Color color, int thickness)
        {
            if (InsecEnabled())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(SliderCheck("max.enemy.count.distance")) && x.IsVisible && !x.IsDead && !x.IsZombie))
                {
                    if (Config.Item("insec.whitelist." + enemy.ChampionName).GetValue<bool>())
                    {
                        switch (Config.Item("insec.to").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                Render.Circle.DrawCircle(Insec.AllyInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance")), 50, color, thickness);
                                break;
                            case 1:
                                Render.Circle.DrawCircle(Insec.TowerInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance")), 50, color, thickness);
                                break;
                            case 2:
                                Render.Circle.DrawCircle(Insec.CursorInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance")), 50, color, thickness);
                                break;
                        }
                    }
                }
            }
           /* if (Config.Item("insec.status").GetValue<StringList>().SelectedIndex == 0)
            {
                switch (Config.Item("insec.to").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                       // return Insec.AllyInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"));
                        break;
                    case 1:
                       // return Insec.TowerInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"));
                        break;
                    case 2:
                       // return Insec.CursorInsec(SliderCheck("max.enemy.count.distance"), SliderCheck("insec.distance"));
                        break;

                }
            }*/
            
        }
        public static bool MaxEnemyInsec()
        {
            if (ObjectManager.Player.CountEnemiesInRange(SliderCheck("max.enemy.count.distance")) < SliderCheck("max.enemy.count"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MinEnergyInsec()
        {
            if (ObjectManager.Player.ManaPercent > SliderCheck("min.mana"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void JumpAlternative()
        {
            switch (Config.Item("flash.insec").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    // Enabled
                    break;
                case 1:
                    // Disabled
                    break;
            }
        }

        public static bool PassiveUsage(string menuName)
        {
            if (Config.Item(menuName).GetValue<StringList>().SelectedIndex == 0)
            {
                if (Passive())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;  
            }
        }
        public static bool Passive()
        {
            if (ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool QOne(Spell Q)
        {
            if (Q.Instance.Name == "BlindMonkQOne")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool QTwo(Spell Q)
        {
            if (Q.Instance.Name == "blindmonkqtwo")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool WOne(Spell W)
        {
            if (W.Instance.Name == "BlindMonkWOne")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool WTwo(Spell W)
        {
            if (W.Instance.Name == "blindmonketwo")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool EOne (Spell E)
        {
            if (E.Instance.Name == "BlindMonkEOne")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool ETwo(Spell E)
        {
            if (E.Instance.Name == "blindmonketwo")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool HasQBuff(Obj_AI_Hero target)
        {
            if (target.HasBuff("BlindMonkQOne"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool HasQBuffMinion(Obj_AI_Base minion)
        {
            if (minion.HasBuff("BlindMonkQOne"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DragonStealQ()
        {
            if (Config.Item("steal.dragon").GetValue<bool>())
            {
                if (QOne(Spells[Q]) && Spells[Q].IsReady() && Config.Item("q.steal").GetValue<bool>())
                {
                    foreach (var drago in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget(Spells[Q].Range) && x.Name.ToLower().Contains("dragon") && !x.IsDead && !x.IsZombie))
                    {
                        if (Spells[Q].GetPrediction(drago).Hitchance > HitChance.High && Spells[Q].GetPrediction(drago).CollisionObjects.Count == 0)
                        {
                            Spells[Q].Cast(drago);
                        }
                    }
                }

                if (QTwo(Spells[Q]) && Spells[Q2].IsReady() && Config.Item("q2.steal").GetValue<bool>())
                {
                    foreach (var drago in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget(Spells[Q].Range) && x.Name.ToLower().Contains("dragon") && !x.IsDead && !x.IsZombie && HasQBuffMinion(x) && Calculator.Q2DamageMinion(x, Spells[Q]) > x.Health))
                    {
                        Spells[Q2].Cast();
                    }
                }
            }
        }
        public static void BaronStealQ()
        {
            if (Config.Item("steal.baron").GetValue<bool>())
            {
                if (QOne(Spells[Q]) && Spells[Q].IsReady() && Config.Item("q.steal").GetValue<bool>())
                {
                    foreach (var baron in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget(Spells[Q].Range) && x.Name.ToLower().Contains("baron") && !x.IsDead && !x.IsZombie))
                    {
                        if (Spells[Q].GetPrediction(baron).Hitchance > HitChance.High && Spells[Q].GetPrediction(baron).CollisionObjects.Count == 0)
                        {
                            Spells[Q].Cast(baron);
                        }
                    }
                }

                if (QTwo(Spells[Q]) && Spells[Q2].IsReady() && Config.Item("q2.steal").GetValue<bool>())
                {
                    foreach (var baron in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget(Spells[Q].Range) && x.Name.ToLower().Contains("baron") && !x.IsDead && !x.IsZombie && HasQBuffMinion(x) && Calculator.Q2DamageMinion(x, Spells[Q]) > x.Health))
                    {
                        Spells[Q2].Cast();
                    }
                }
            }
        }
        public static void CustomizableInterrupter()
        {
            if (ObjectManager.Player.CountEnemiesInRange(Spells[R].Range) > 1)
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity) && x.ChampionName == "Katarina" || x.ChampionName == "MissFortune"))
                {
                    if (enemy.HasBuff("KatarinaR") || enemy.HasBuff("katarinarsound") && Config.Item("katarina.r").GetValue<bool>())
                    {
                        Spells[R].Cast(enemy);
                    }
                    if (enemy.HasBuff("missfortunebulletsound") || enemy.HasBuff("MissFortuneBulletTime") && Config.Item("miss.fortune.r").GetValue<bool>())
                    {
                        Spells[R].Cast(enemy);
                    }
                }
            }
        }

    }
}
