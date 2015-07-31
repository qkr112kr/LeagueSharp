using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace doubleH_Support
{
    public class Leona
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player;

        public Leona()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 1200);

            E.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            comboMenu.AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("wCombo", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("rCombo", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("renemyhit", "R Enemy Hit").SetValue(new Slider(2, 1, 5)));
            hLoader.Config.AddSubMenu(comboMenu);


            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            miscMenu.AddItem(new MenuItem("gapclose", "Anti-Gapcloser [Q]").SetValue(true));
            miscMenu.AddItem(new MenuItem("interrupter", "Interrupt [Q]").SetValue(true));
            hLoader.Config.AddSubMenu(miscMenu);

            var drawMenu = new Menu("Misc Settings", "Misc Settings");
            drawMenu.AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.Yellow)));
            drawMenu.AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.Green)));
            drawMenu.AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.White)));
            drawMenu.AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Brown)));
            drawMenu.AddItem(new MenuItem("qDraw", "AA Range").SetValue(new Circle(true, Color.Blue)));

            hLoader.Config.AddSubMenu(drawMenu);



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
            var rHit = hLoader.Config.Item("renemyhit").GetValue<Slider>().Value;
            var rTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical, true);
            if (Q.IsReady() && hLoader.Config.Item("qCombo").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(E.Range)))
                {
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical, true);
                    if (Q.CanCast(target) && !Player.IsWindingUp && !Player.IsDashing())

                        Q.Cast();
                }
            }
            if (E.IsReady() && hLoader.Config.Item("eCombo").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(E.Range)))
                {
                    var targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical, true);
                    if (E.CanCast(targetE) && E.GetPrediction(targetE).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing())

                        E.Cast(targetE);
                }
            }

            if (W.IsReady() && hLoader.Config.Item("wCombo").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(E.Range)))
                {
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical, true);
                    if (W.CanCast(target) && !Player.IsWindingUp && !Player.IsDashing())

                        W.Cast();
                }
            }

            if (R.IsReady() && hLoader.Config.Item("rCombo").GetValue<bool>()
                && ObjectManager.Player.CountEnemiesInRange(R.Range) >= rHit
                && R.IsInRange(rTarget.ServerPosition))
            {
                R.CastIfHitchanceEquals(rTarget, HitChance.High, true);
            }
        }
        private void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = hLoader.Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = hLoader.Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = hLoader.Config.Item("eDraw").GetValue<Circle>();
            var menuItem4 = hLoader.Config.Item("rDraw").GetValue<Circle>();
            var menuItem5 = hLoader.Config.Item("aaRangeDraw").GetValue<Circle>();

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