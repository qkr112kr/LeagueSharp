using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HikiCarry_Lee_Sin.Core;
using HikiCarry_Lee_Sin.Stages;
using LeagueSharp;
using LeagueSharp.Common;
using HybridCommon;
using SharpDX;
using Color = System.Drawing.Color;

namespace HikiCarry_Lee_Sin.Champions
{
    public class LeeSin : BaseChamp
    {
        public LeeSin()
            : base ("LeeSin")
        {
            
        }

        public override void CreateConfigMenu()
        {
            ComboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                ComboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                ComboMenu.AddItem(new MenuItem("q2.combo", "> Use Q2").SetValue(true));
                ComboMenu.AddItem(new MenuItem("w.combo", "Use W").SetValue(true));
                ComboMenu.AddItem(new MenuItem("w2.combo", "> Use W2").SetValue(true));
                ComboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                ComboMenu.AddItem(new MenuItem("e2.combo", "> Use E2").SetValue(true));
                ComboMenu.AddItem(new MenuItem("r.combo", "Use R").SetValue(true));
                ComboMenu.AddItem(new MenuItem("passive.usage", "Passive Usage?").SetValue(new StringList(new[] { "Enabled", "Disabled" }))); // +
            }
            //SMenu
            SMenu = new Menu("Star Combo Settings", "Star Combo Settings");
            {
                SMenu.AddItem(new MenuItem("q.star", "Use Q").SetValue(true));
                SMenu.AddItem(new MenuItem("q2.star", "> Use Q2").SetValue(true));
                SMenu.AddItem(new MenuItem("w.star", "Use W").SetValue(true));
                SMenu.AddItem(new MenuItem("w2.star", "> Use W2").SetValue(true));
                SMenu.AddItem(new MenuItem("e.star", "Use E").SetValue(true));
                SMenu.AddItem(new MenuItem("e2.star", "> Use E2").SetValue(true));
                SMenu.AddItem(new MenuItem("r.star", "Use R").SetValue(true));
                var starItems = new Menu("Star Combo Items", "Star Combo Items");
                {
                    starItems.AddItem(new MenuItem("star.tiamat", "Tiamat / Hydra").SetValue(true));
                    SMenu.AddSubMenu(starItems);
                }

            }

            HikiMenu = new Menu("Hikigaya Combo Settings", "Hikigaya Combo Settings");
            {
                HikiMenu.AddItem(new MenuItem("q.hiki", "Use Q").SetValue(true));
                HikiMenu.AddItem(new MenuItem("q2.hiki", "> Use Q2").SetValue(true));
                HikiMenu.AddItem(new MenuItem("e.hiki", "Use E").SetValue(true));
                HikiMenu.AddItem(new MenuItem("e2.hiki", "> Use E2").SetValue(true));
                HikiMenu.AddItem(new MenuItem("r.hiki", "Use R").SetValue(true));
                var hikiItems = new Menu("Hikigaya Combo Items", "Hikigaya Combo Items");
                {
                    hikiItems.AddItem(new MenuItem("hiki.tiamat", "Tiamat / Hydra").SetValue(true));
                    HikiMenu.AddSubMenu(hikiItems);
                }
                
            }
            InsecMenu = new Menu("Insec Settings", "Insec Settings");
            {
                WhiteMenu = new Menu("Insec Whitelist", "Insec Whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid))
                    {
                        WhiteMenu.AddItem(new MenuItem("insec.whitelist." + enemy.ChampionName, "" + enemy.ChampionName).SetValue(true));
                    }
                    InsecMenu.AddSubMenu(WhiteMenu);
                }
                InsecMenu.AddItem(new MenuItem("insec.status", "Insec Status").SetValue(new StringList(new[] { "Enabled", "Disabled" }))); // +
                InsecMenu.AddItem(new MenuItem("insec.to", "Insec to ?").SetValue(new StringList(new[] { "Ally", "Tower", "Cursor Position" }, 2)));
                //InsecMenu.AddItem(new MenuItem("max.enemy.count", "Max. Enemy for Insec").SetValue(new Slider(3, 1, 5))); // +
                InsecMenu.AddItem(new MenuItem("max.enemy.count.distance", "Enemy Search Range").SetValue(new Slider(1000, 1, 2000))); // +
                //InsecMenu.AddItem(new MenuItem("min.mana", "Min. Energy for Insec").SetValue(new Slider(50, 1, 99))); //+
                InsecMenu.AddItem(new MenuItem("insec.distance", "Min. Insec Distance").SetValue(new Slider(300, 1, 374)));
                //InsecMenu.AddItem(new MenuItem("flash.insec", "Flash Insec").SetValue(new StringList(new[] { "Enabled", "Disabled" }, 1))); //+
               
            }
            HarassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                HarassMenu.AddItem(new MenuItem("q.harass", "Use Q").SetValue(true));
                HarassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
                HarassMenu.AddItem(new MenuItem("e2.harass", "Use E2").SetValue(true));
            }

            ClearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                ClearMenu.AddItem(new MenuItem("q.clear", "Use Q").SetValue(true));
                ClearMenu.AddItem(new MenuItem("q2.clear", "> Use Q2").SetValue(true));
                ClearMenu.AddItem(new MenuItem("e.clear", "Use E").SetValue(true));
                ClearMenu.AddItem(new MenuItem("e2.clear", "> Use E2").SetValue(true));
                ClearMenu.AddItem(new MenuItem("e.minion.count", "E Minion Count").SetValue(new Slider(3, 1, 5)));
                //ClearMenu.AddItem(new MenuItem("passive.usage.clear", "Passive Usage?").SetValue(new StringList(new[] { "Enabled", "Disabled" }))); // +
                
            }
            JungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                JungleMenu.AddItem(new MenuItem("q.jungle", "Use Q").SetValue(true));
                JungleMenu.AddItem(new MenuItem("q2.jungle", "> Use Q2").SetValue(true));
                JungleMenu.AddItem(new MenuItem("w.jungle", "Use W").SetValue(true));
                JungleMenu.AddItem(new MenuItem("w2.jungle", "> Use W2").SetValue(true));
                JungleMenu.AddItem(new MenuItem("e.jungle", "Use E").SetValue(true));
                JungleMenu.AddItem(new MenuItem("e2.jungle", "> Use E2").SetValue(true));
                JungleMenu.AddItem(new MenuItem("passive.usage.jungle", "Passive Usage?").SetValue(new StringList(new[] { "Enabled", "Disabled" }))); // +
                
            }
            StealMenu = new Menu("Jungle Steal Settings", "Jungle Steal Settings");
            {
                var stealSkill = new Menu("Jung Steal Skill", "Jung Steal Skill");
                {
                    stealSkill.AddItem(new MenuItem("q.steal", "Use Q").SetValue(true));
                    stealSkill.AddItem(new MenuItem("q2.steal", "Use Q2").SetValue(true));
                    StealMenu.AddSubMenu(stealSkill);
                }
                StealMenu.AddItem(new MenuItem("steal.dragon", "Steal Dragon").SetValue(true));
                StealMenu.AddItem(new MenuItem("steal.baron", "Steal Baron").SetValue(true));
            }
            KillStealMenu = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                KillStealMenu.AddItem(new MenuItem("q.ks", "Use Q").SetValue(true));
                KillStealMenu.AddItem(new MenuItem("q2.ks", "> Use Q2").SetValue(true));
                KillStealMenu.AddItem(new MenuItem("e.ks", "Use E").SetValue(true));
                KillStealMenu.AddItem(new MenuItem("r.ks", "Use R").SetValue(true));
                KillStealMenu.AddItem(new MenuItem("killsteal", "Kill Steal?").SetValue(new StringList(new[] { "Enabled", "Disabled" }))); // +
                
            }
            DrawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                DrawMenu.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, Color.White)));
                DrawMenu.AddItem(new MenuItem("q2.draw", "Q Range").SetValue(new Circle(true, Color.DarkSeaGreen)));
                DrawMenu.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, Color.Gold)));
                DrawMenu.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, Color.DodgerBlue)));
                DrawMenu.AddItem(new MenuItem("e2.draw", "E Range").SetValue(new Circle(true, Color.SeaGreen)));
                DrawMenu.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, Color.GreenYellow)));

                var insecDraw = new Menu("Insec Draws", "Insec Draws");
                {
                    insecDraw.AddItem(new MenuItem("insec.circle", "Insec Circle").SetValue(new Circle(true, Color.Gold)));
                    insecDraw.AddItem(new MenuItem("insec.line", "Insec Line").SetValue(new Circle(true, Color.Gold)));
                    insecDraw.AddItem(new MenuItem("thickness", "Thickness").SetValue(new Slider(5, 1, 5)));
                    DrawMenu.AddSubMenu(insecDraw);
                }
                var objectDraws = new Menu("Object Draws", "Object Draws");
                {
                    objectDraws.AddItem(new MenuItem("ward.draw", "Ward Draw").SetValue(new Circle(true, Color.Red)));
                    DrawMenu.AddSubMenu(objectDraws);
                }
                
            }

            m_evader = new Evader(out Evade, EvadeMethods.LeeSinW);

            Config.AddSubMenu(ComboMenu);
            Config.AddSubMenu(SMenu);
            Config.AddSubMenu(HikiMenu);
            Config.AddSubMenu(InsecMenu);
            Config.AddSubMenu(HarassMenu);
            Config.AddSubMenu(ClearMenu);
            Config.AddSubMenu(JungleMenu);
            Config.AddSubMenu(StealMenu);
            Config.AddSubMenu(KillStealMenu);
            Config.AddSubMenu(DrawMenu);
            Config.AddSubMenu(Evade);
            
            
            Config.AddItem(new MenuItem("masterracec0mb0", "                  Hikigaya Lee Sin Keys"));
            Config.AddItem(new MenuItem("insec.active", "Insec!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            Config.AddItem(new MenuItem("star.active", "Star Combo!").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            Config.AddItem(new MenuItem("hiki.active", "Hikigaya Combo!").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.AddItem(new MenuItem("wardjump.active", "Ward Jump!").SetValue(new KeyBind("S".ToCharArray()[0], KeyBindType.Press)));
            Config.AddToMainMenu();
            DamageIndicator.DamageToUnit = (t) => (float)CalculateComboDamage(t);
            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Drawing.OnDraw += OnDraw;
            
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (WardJump.WardCastable || !(sender is Obj_AI_Base) || sender.IsEnemy)
            {
                return;
            }
            var wardObject = (Obj_AI_Base)sender;
            if (wardObject.Name.IndexOf("ward", StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                Vector3.DistanceSquared(sender.Position, WardJump.WardCastPosition) <= 150 * 150)
            {
                Spells[W].Cast(wardObject);
            }
        }
        private void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (Config.Item("q.draw").GetValue<Circle>().Active && Spells[Q].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[Q].Range, Config.Item("q.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("q2.draw").GetValue<Circle>().Active && Spells[Q].IsReady() && Spells[Q2].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[Q2].Range, Config.Item("q2.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("w.draw").GetValue<Circle>().Active && Spells[W].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[W].Range, Config.Item("w.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("e.draw").GetValue<Circle>().Active && Spells[E].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[E].Range, Config.Item("e.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("e2.draw").GetValue<Circle>().Active && Spells[E2].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[E2].Range, Config.Item("e2.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("r.draw").GetValue<Circle>().Active && Spells[R].IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells[R].Range, Config.Item("r.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("insec.circle").GetValue<Circle>().Active && Config.Item("insec.active").GetValue<KeyBind>().Active && Spells[Q].IsReady() && Spells[W].IsReady() && Spells[R].IsReady())
            {
                Helper.InsecDrawCircle(Config.Item("insec.circle").GetValue<Circle>().Color, Helper.SliderCheck("thickness"));
            }
            if (Config.Item("insec.line").GetValue<Circle>().Active && Config.Item("insec.active").GetValue<KeyBind>().Active && Spells[Q].IsReady() && Spells[W].IsReady() && Spells[R].IsReady())
            {
                Helper.InsecDrawLine(Config.Item("insec.line").GetValue<Circle>().Color);
            }
            if (Config.Item("ward.draw").GetValue<Circle>().Active)
            {
                Helper.WardDraw(Config.Item("ward.draw").GetValue<Circle>().Color);
            }

        }
        private void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    Helper.DragonStealQ();
                    Helper.BaronStealQ();
                    Jung();
                    break;
            }

            if (Config.Item("wardjump.active").GetValue<KeyBind>().Active)
            {
                WardJump.HikiJump(Spells[W],Game.CursorPos);
            }

            if (Config.Item("insec.active").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Helper.SliderCheck("max.enemy.count.distance"))))
                {
                    Insec.HikiInsec(enemy);
                }   
            }
            if (Config.Item("star.active").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                StarCombo();
            }
            if (Config.Item("hiki.active").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                HikiCombo();
            }
            if (Config.Item("killsteal").GetValue<StringList>().SelectedIndex == 0)
            {
                KillSteal();
            }
            
        }

        private void Harass()
        {
            if (Spells[Q].IsReady() && Helper.QOne(Spells[Q]) && Config.Item("q.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    if (Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High && Spells[Q].GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        Spells[Q].Cast(enemy);
                    }
                }
            }
            if (Spells[Q2].IsReady() && Helper.QTwo(Spells[Q]) && Config.Item("q2.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q2].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    if (Helper.HasQBuff(enemy))
                    {
                        Spells[Q2].Cast();
                    }
                }
            }
            if (Spells[E].IsReady() && Helper.EOne(Spells[E]) && Config.Item("e.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[E].Cast();
                }
            }

        }
        private void Combo()
        {
            if (Spells[Q].IsReady() && Helper.QOne(Spells[Q]) && Config.Item("q.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells[Q].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    if (Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High && Spells[Q].GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        Spells[Q].Cast(enemy);
                    }
                }
            }
            if (Spells[Q2].IsReady() && Helper.QTwo(Spells[Q]) && Config.Item("q2.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q2].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    if (Helper.HasQBuff(enemy))
                    {
                        Spells[Q2].Cast();
                    }
                }
            }

            if (Spells[E].IsReady() && Helper.EOne(Spells[E]) && Config.Item("e.combo").GetValue<bool>() && !Helper.PassiveUsage("passive.usage"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[E].Cast();
                }
            }
            if (Spells[E2].IsReady() && Helper.ETwo(Spells[E]) && Config.Item("e2.combo").GetValue<bool>() && !Helper.PassiveUsage("passive.usage"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E2].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[E2].Cast();
                }
            }
            if (Spells[W].IsReady() && Helper.WOne(Spells[W]) && Config.Item("w.combo").GetValue<bool>() && !Helper.PassiveUsage("passive.usage"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[W].Cast(ObjectManager.Player);
                }
            }
            if (Spells[W2].IsReady() && Helper.WTwo(Spells[W]) && Config.Item("w2.combo").GetValue<bool>() && !Helper.PassiveUsage("passive.usage"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[W].Cast();
                }
            }
            if (Spells[R].IsReady() && Config.Item("r.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[R].Range) && !x.IsDead && x.IsVisible && !x.IsZombie && Spells[R].GetDamage(x) > x.Health))
                {
                    Spells[R].Cast(enemy);
                }
            }
        }
        private void Jung()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mob == null || mob.Count == 0)
            {
                return;
            }
            if (Spells[Q].IsReady() && Helper.QOne(Spells[Q]) && Config.Item("q.jungle").GetValue<bool>() && !Helper.PassiveUsage("passive.usage.jungle"))
            {
                if (mob[0].Distance(ObjectManager.Player.Position) < Spells[Q].Range)
                {
                    Spells[Q].Cast(mob[0]);
                }
            }
            if (Spells[Q2].IsReady() && Helper.QTwo(Spells[Q2]) && Config.Item("q2.jungle").GetValue<bool>() && !Helper.PassiveUsage("passive.usage.jungle"))
            {
                if (mob[0].Distance(ObjectManager.Player.Position) < Spells[Q2].Range)
                {
                    Spells[Q2].Cast(mob[0]);
                }
            }
            if (Spells[E].IsReady() && Helper.EOne(Spells[E]) && Config.Item("e.jungle").GetValue<bool>() && !Helper.PassiveUsage("passive.usage.jungle"))
            {
                if (mob[0].Distance(ObjectManager.Player.Position) < Spells[E].Range)
                {
                    Spells[E].Cast();
                }
            }
            if (Spells[E2].IsReady() && Helper.ETwo(Spells[E]) && Config.Item("e2.jungle").GetValue<bool>() && !Helper.PassiveUsage("passive.usage.jungle"))
            {
                if (mob[0].Distance(ObjectManager.Player.Position) < Spells[E2].Range)
                {
                    Spells[E2].Cast();
                }
            }
            if (Spells[W].IsReady() && Helper.WOne(Spells[W]) && Config.Item("w.jungle").GetValue<bool>() && !Helper.PassiveUsage("passive.usage.jungle"))
            {
                if (mob[0].Distance(ObjectManager.Player.Position) < Spells[E2].Range)
                {
                    Spells[W].Cast(ObjectManager.Player);
                }
            }
            if (Spells[W2].IsReady() && Helper.WTwo(Spells[W]) && Config.Item("w2.jungle").GetValue<bool>() && !Helper.PassiveUsage("passive.usage.jungle"))
            {
                if (mob[0].Distance(ObjectManager.Player.Position) < Spells[E2].Range)
                {
                    Spells[W2].Cast();
                }
            }
        }
        private void Clear()
        {
            var qMinion = MinionManager.GetMinions(ObjectManager.Player.Position,Spells[Q].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (Spells[Q].IsReady() && Helper.QOne(Spells[Q]))
            {
                foreach (var minion in qMinion.Where(x=> x.Health < Calculator.QDamage(x,Spells[Q])))
                {
                    Spells[Q].Cast(minion);
                }
            }
            if (Spells[Q2].IsReady() && Helper.QTwo(Spells[Q2]))
            {
                foreach (var minion in qMinion.Where(x => x.Health < Calculator.Q2DamageMinion(x, Spells[Q])))
                {
                    Spells[Q2].Cast();
                }
            }
            if (Spells[E].IsReady() && Helper.EOne(Spells[E]))
            {
                var countMinion = qMinion.Where(x => x.IsValidTarget(Spells[E].Range)).Count();
                if (countMinion >= Helper.SliderCheck("e.minion.count"))
                {
                    Spells[E].Cast();
                }

            }
            
            
        }
        private void HikiCombo()
        {
            if (Spells[Q].IsReady() && Helper.QOne(Spells[Q]) && Config.Item("q.hiki").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    if (Spells[Q].CanCast(enemy) && Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High)
                    {
                        Spells[Q].Cast(enemy);
                    }
                }
            }
            if (Spells[E].IsReady() && Helper.EOne(Spells[E]) && Config.Item("e.hiki").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && Helper.HasQBuff(x) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[E].Cast();
                }
            }
            if (Spells[R].IsReady() && Config.Item("r.hiki").GetValue<bool>() && Helper.QTwo(Spells[Q]) && Helper.ETwo(Spells[E]))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[R].Range) && !x.IsDead && x.IsVisible && !x.IsZombie && Helper.HasQBuff(x)))
                {
                    Spells[R].Cast(enemy);
                }
            }
            if (Spells[Q2].IsReady() && Helper.QTwo(Spells[Q]) && Config.Item("q2.hiki").GetValue<bool>() && !Spells[R].IsReady() && Helper.ETwo(Spells[E]))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q2].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    if (Helper.HasQBuff(enemy))
                    {
                        Spells[Q2].Cast();
                    }
                }
            }
            
            if (Spells[E2].IsReady() && Helper.ETwo(Spells[E]) && Config.Item("e2.hiki").GetValue<bool>() && !Spells[Q].IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E2].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[E2].Cast();
                }
            }
            
        }
        private void StarCombo()
        {
            if (Spells[Q].IsReady() && Helper.QOne(Spells[Q]) && Config.Item("q.star").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    if (Spells[Q].CanCast(enemy) && Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High)
                    {
                        Spells[Q].Cast(enemy);
                    }
                }
            }
            if (Spells[R].IsReady() && Helper.QTwo(Spells[Q]) && Config.Item("r.star").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells[R].Range) && Helper.HasQBuff(x) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[R].Cast(enemy);
                }
            }
            if (Spells[Q2].IsReady() && Helper.QTwo(Spells[Q]) && Config.Item("q2.star").GetValue<bool>() && !Spells[R].IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q2].Range) && !x.IsDead && x.IsVisible && !x.IsZombie && Helper.HasQBuff(x)))
                {
                    Spells[Q2].Cast();
                }
            }
            if (Spells[E].IsReady() && Helper.EOne(Spells[E]) && Config.Item("e.star").GetValue<bool>() && !Spells[Q].IsReady() && Spells[R].IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[E].Cast();
                }
            }
            if (Spells[E2].IsReady() && Helper.ETwo(Spells[E]) && Config.Item("e2.star").GetValue<bool>() && !Spells[Q].IsReady() && Spells[R].IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E2].Range) && !x.IsDead && x.IsVisible && !x.IsZombie))
                {
                    Spells[E2].Cast();
                }
            }
            
        }
        private void KillSteal()
        {
            if (Spells[Q].IsReady() && Config.Item("q.ks").GetValue<bool>() && Helper.QOne(Spells[Q]))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells[Q].Range) && x.Health < CalculateDamageQ(x) && !x.IsDead && !x.IsZombie))
                {
                    if (Spells[Q].GetPrediction(enemy).Hitchance > HitChance.High && Spells[Q].GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        Spells[Q].Cast(enemy);
                    }
                }
            }
            if (Spells[Q2].IsReady() && Config.Item("q2.ks").GetValue<bool>() && Helper.QTwo(Spells[Q]))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[Q2].Range) && x.Health < Calculator.Q2Damage(x,Spells[Q]) && !x.IsDead && !x.IsZombie && Helper.HasQBuff(x)))
                {
                    Spells[Q2].Cast();
                }
            }
            if (Spells[E].IsReady() && Config.Item("e.ks").GetValue<bool>() && Helper.EOne(Spells[E]))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells[E].Range) && x.Health < CalculateDamageE(x) && !x.IsDead && !x.IsZombie))
                {
                    Spells[E].Cast();
                }
            }
        }
        public override void SetSpells()
        {

            Spells[Q] = new Spell(SpellSlot.Q, 1100f);
            Spells[Q2] = new Spell(SpellSlot.Q, 1300);
            Spells[W] = new Spell(SpellSlot.W, 700f);
            Spells[W2] = new Spell(SpellSlot.W);
            Spells[E] = new Spell(SpellSlot.E, 330f);
            Spells[E2] = new Spell(SpellSlot.E);
            Spells[R] = new Spell(SpellSlot.R, 375f);

            Spells[Q].SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);
            m_evader.SetEvadeSpell(Spells[W]);
        }
    }
}

