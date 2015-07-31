using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace doubleH_Support.Champions
{
    public class Blitzcrank
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        public static Obj_AI_Hero Player;

        public Blitzcrank()
        {
            Q = new Spell(SpellSlot.Q, 995f);
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
            hLoader.Config.AddSubMenu(comboMenu);

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            clearMenu.AddItem(new MenuItem("rClear", "Use R").SetValue(true));
            clearMenu.AddItem(new MenuItem("useRSlider", "R Minions Hit >").SetValue(new Slider(10, 1, 20)));
            hLoader.Config.AddSubMenu(clearMenu);

            var killstealMenu = new Menu("Killsteal Settings", "Killsteal Settings");
            killstealMenu.AddItem(new MenuItem("killsteal", "Killsteal").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealQ", "Use Q").SetValue(true));
            killstealMenu.AddItem(new MenuItem("killstealR", "Use R").SetValue(true));
            hLoader.Config.AddSubMenu(killstealMenu);

            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            miscMenu.AddItem(new MenuItem("interrupter", "Interrupt [Q]").SetValue(true));
            hLoader.Config.AddSubMenu(miscMenu);

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
            switch (hLoader.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }

        }

        private static void Harass()
        {
            /*soon tm*/
        }
        private static void Combo()
        {
            var useQ = hLoader.Config.Item("qCombo").GetValue<bool>();
            var useE = hLoader.Config.Item("eCombo").GetValue<bool>();
            var useR = hLoader.Config.Item("rCombo").GetValue<bool>();
            var rHit = hLoader.Config.Item("renemyhit").GetValue<Slider>().Value;
            var rTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical, true);

            if (useQ && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                {
                    if (Q.GetPrediction(enemy).Hitchance >= HitChance.High)
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
        private static void Clear()
        {

            var rMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range, MinionTypes.All);
            var useR = hLoader.Config.Item("rClear").GetValue<bool>();
            var rHit = hLoader.Config.Item("useRSlider").GetValue<Slider>().Value;
            if (R.IsReady() && useR)
            {
                var rFarm = R.GetCircularFarmLocation(rMinions, R.Width);
                if (rFarm.MinionsHit >= rHit)
                {
                    R.Cast(rFarm.Position);
                }
            }
        }
        private static void KillSteal()
        {
            var ksEnabled = hLoader.Config.Item("killsteal").GetValue<bool>();
            var useQ = hLoader.Config.Item("killstealQ").GetValue<bool>();
            var useR = hLoader.Config.Item("killstealR").GetValue<bool>();
            var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical, true);
            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);
            if (ksEnabled)
            {
                if (Q.IsReady() && useQ && qTarget.IsValidTarget(Q.Range)
                    && qTarget.Health < Q.GetDamage(qTarget) && Q.CanCast(qTarget))
                {
                    if (Q.GetPrediction(qTarget).Hitchance >= HitChance.High &&
                        Q.GetPrediction(qTarget).UnitPosition.Distance(Player.Position) > 200 &&
                        Player.Distance(qTarget.Position) <= 1000)
                    {
                        Q.Cast(qTarget);
                    }
                }
                if (R.IsReady() && useR && rTarget.IsValidTarget(R.Range)
                    && rTarget.Health < R.GetDamage(qTarget) && R.CanCast(qTarget))
                {
                    R.Cast(rTarget);
                }
            }

        }
        private void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = hLoader.Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = hLoader.Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = hLoader.Config.Item("eDraw").GetValue<Circle>();
            var menuItem4 = hLoader.Config.Item("rDraw").GetValue<Circle>();

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Yellow);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Green);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.White);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Brown);
            }
        }


    }
}