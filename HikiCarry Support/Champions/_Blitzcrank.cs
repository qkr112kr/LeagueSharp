using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace HikiCarry_Support.Champions
{
    public class _Blitzcrank
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player;

        public _Blitzcrank()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 150f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 600, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            comboMenu.AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("rCombo", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("renemyhit", "R Enemy Hit").SetValue(new Slider(2, 1, 5)));
            SSeries.Config.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            harassMenu.AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("eHarass", "Use E").SetValue(true));
            SSeries.Config.AddSubMenu(harassMenu);

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            clearMenu.AddItem(new MenuItem("qClear", "Use Q").SetValue(true));
            clearMenu.AddItem(new MenuItem("eClear", "Use E").SetValue(true));
            clearMenu.AddItem(new MenuItem("rClear", "Use R").SetValue(true));
            clearMenu.AddItem(new MenuItem("useRSlider", "R Minions Hit >").SetValue(new Slider(10, 1, 20)));
            SSeries.Config.AddSubMenu(clearMenu);

            var killstealMenu = new Menu("Killsteal Settings", "Killsteal Settings");
            killstealMenu.AddItem(new MenuItem("killsteal", "Killsteal").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealQ","Use Q").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealR", "Use R").SetValue(true));
            SSeries.Config.AddSubMenu(killstealMenu);

            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            miscMenu.AddItem(new MenuItem("interrupter", "Interrupt [Q]").SetValue(true));
            SSeries.Config.AddSubMenu(miscMenu);

            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            drawMenu.AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.Yellow)));
            drawMenu.AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.Green)));
            drawMenu.AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.White)));
            drawMenu.AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Brown)));
            drawMenu.AddItem(new MenuItem("qDraw", "AA Range").SetValue(new Circle(true, Color.Blue)));

            SSeries.Config.AddSubMenu(drawMenu);



            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;

        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsValidTarget(1000))
            {
                Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
            }

            if (Q.CanCast(gapcloser.Sender))
                Q.Cast(gapcloser.Sender);
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsValidTarget(1000))
            {
                Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                var targetpos = Drawing.WorldToScreen(sender.Position);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
            }

            if (Q.CanCast(sender))
                Q.Cast(sender);
        }
        internal static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
        }

        private static void Combo()
        {
            var useQ = SSeries.Config.Item("qCombo").GetValue<bool>();
            var useE = SSeries.Config.Item("eCombo").GetValue<bool>();
            var useR = SSeries.Config.Item("rCombo").GetValue<bool>();
            var rHit = SSeries.Config.Item("renemyhit").GetValue<Slider>().Value;
            var rTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical, true);

            if (useQ && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                {
                    if (Q.CanCast(enemy) && !Player.IsWindingUp && !Player.IsDashing() &&
                        Q.GetPrediction(enemy).Hitchance >= HitChance.High &&
                        Q.GetPrediction(enemy).UnitPosition.Distance(Player.Position) > 200
                        && Player.Distance(enemy.Position) <= 1000)
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            if (useE && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range)))
                {
                    if (Player.Distance(enemy.Position) < 100 & E.CanCast(enemy) && !enemy.HasBuffOfType(BuffType.Knockup))
                    {
                        E.Cast(enemy);
                    }
                }
            }
            if (R.IsReady() && useR && ObjectManager.Player.CountEnemiesInRange(R.Range) >= rHit
                && R.IsInRange(rTarget.ServerPosition))
            {
                R.CastIfHitchanceEquals(rTarget, HitChance.High, true);
            }

        }
        private void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = SSeries.Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = SSeries.Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = SSeries.Config.Item("eDraw").GetValue<Circle>();
            var menuItem4 = SSeries.Config.Item("rDraw").GetValue<Circle>();
            var menuItem5 = SSeries.Config.Item("aaRangeDraw").GetValue<Circle>();

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Yellow);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Yellow);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.White);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.White);
            }
            if (menuItem5.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 125, Color.White);
            }
        }


    }
}