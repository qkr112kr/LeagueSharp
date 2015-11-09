using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace HikiCarry_Caitlyn
{
    class Program
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        private static readonly Obj_AI_Hero Caitlyn = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Caitlyn.ChampionName != "Caitlyn")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1240);
            W = new Spell(SpellSlot.W, 820);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.25f, 60f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            Config = new Menu("HikiCarry - Kalista", "HikiCarry - Kalista", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            var comboMenu = new Menu("• Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("w.combo", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("r.combo", "Use R").SetValue(true));
                Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("• Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(harassMenu);
            }
            var laneMenu = new Menu("• Clear Settings", "Clear Settings");
            {
                laneMenu.AddItem(new MenuItem("q.clear", "Use E").SetValue(true));
                laneMenu.AddItem(new MenuItem("q.minion.hit", "Min. Minion Hit").SetValue(new Slider(3, 1, 5)));
                laneMenu.AddItem(new MenuItem("clear.mana", "Clear Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(laneMenu);
            }
            var killstealMenu = new Menu("• KillSteal Settings", "KillSteal Settings");
            {
                killstealMenu.AddItem(new MenuItem("q.ks", "Use Q").SetValue(true));
                killstealMenu.AddItem(new MenuItem("r.ks", "Use R").SetValue(true));
                Config.AddSubMenu(killstealMenu);
            }
            var miscMenu = new Menu("• Miscellaneous", "Miscellaneous");
            {
                var orbSet = new Menu("• Scrying Orb Settings", "Scrying Orb Settings");
                {
                    orbSet.AddItem(new MenuItem("bT", "Auto Scrying Orb Buy!").SetValue(true));
                    orbSet.AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
                    miscMenu.AddSubMenu(orbSet);
                }
            }
            var drawMenu = new Menu("• Draw Settings", "Draw Settings");
            {
                drawMenu.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, Color.White)));
                drawMenu.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, Color.Silver)));
                drawMenu.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, Color.Yellow)));
                drawMenu.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, Color.Gold)));
                Config.AddSubMenu(drawMenu);
            }
            Config.AddItem(new MenuItem("eq.fast", "E+Q!", true).SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            Config.AddItem(new MenuItem("hikiChance", "Skillshot Hit Chance").SetValue<StringList>(new StringList(Helper.HitchanceNameArray, 2)));
            Config.AddToMainMenu();
            Game.PrintChat("<font color='#ff3232'>HikiCarry Caitlyn: </font> <font color='#d4d4d4'>If you like this assembly feel free to upvote on Assembly DB</font>");
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void OnGameUpdate(EventArgs args)
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
                    break;
            }
            if (Helper.MenuCheck("bT"))
            {
                Helper.BlueOrb(Config.Item("bluetrinketlevel").GetValue<Slider>().Value);
            }
            if (Config.Item("eq.fast").GetValue<KeyBind>().Active)
            {
                EqFast();
            }
        }

        private static void EqFast()
        {
            Caitlyn.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (!Q.IsReady() || !E.IsReady())
            {
                return;
            }

            if (Q.IsReady() && E.IsReady() && Helper.MenuCheck("q.combo") && Helper.MenuCheck("e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(E.Range) && !x.IsZombie && !x.IsDead))
                {
                    if (Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("hikiChance") && E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("hikiChance"))
                    {
                        Q.Cast(enemy);
                        if (!Q.IsReady())
                        {
                            E.Cast(enemy);
                        }
                    }
                }
            }
        }
        private static void Combo()
        {
            if (Helper.MenuCheck("q.combo") && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("hikiChance"))
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            if (Helper.MenuCheck("w.combo") && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.IsDead && !x.IsZombie && Helper.Immobile(x)))
                {
                    W.Cast(enemy.Position);
                }
            }
            if (Helper.MenuCheck("e.combo") && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("hikiChance"))
                    {
                        E.Cast(enemy);
                    }
                }
            }
            if (Helper.MenuCheck("r.combo") && R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (!Caitlyn.UnderTurret(true) && R.GetDamage(enemy) > enemy.Health)
                    {
                        R.Cast(enemy);
                    }
                }
            }
        }
        private static void Harass()
        {
            if(Caitlyn.ManaPercent < Helper.CountCheckerino("harass.mana"))
            {
                return;
            }
            if (Helper.MenuCheck("q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("hikiChance"))
                    {
                        Q.Cast(enemy);
                    }
                }
            }
        }
        private static void Clear()
        {
            if (Caitlyn.ManaPercent < Helper.CountCheckerino("clear.mana"))
            {
                return;
            }
            if (Helper.MenuCheck("q.clear") && Q.IsReady())
            {
                var minionQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                var lineLocation = Q.GetCircularFarmLocation(minionQ);
                if (lineLocation.MinionsHit >= Helper.CountCheckerino("q.minion.hit"))
                {
                    Q.Cast(lineLocation.Position);
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("q.draw").GetValue<Circle>().Active && Q.IsReady())
            {
                Helper.SkillDraw(Q.Range, Config.Item("q.draw").GetValue<Circle>().Color, 5);
            }
            if (Config.Item("w.draw").GetValue<Circle>().Active && W.IsReady())
            {
                Helper.SkillDraw(W.Range, Config.Item("w.draw").GetValue<Circle>().Color, 5);
            }
            if (Config.Item("e.draw").GetValue<Circle>().Active && E.IsReady())
            {
                Helper.SkillDraw(E.Range, Config.Item("e.draw").GetValue<Circle>().Color, 5);
            }
            if (Config.Item("r.draw").GetValue<Circle>().Active && R.IsReady())
            {
                Helper.SkillDraw(R.Range, Config.Item("r.draw").GetValue<Circle>().Color, 5);
            }
            
        }
    }
}
