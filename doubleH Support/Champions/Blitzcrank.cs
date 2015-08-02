using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace doubleH_Support
{
    public class Blitzcrank
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player;

        public Blitzcrank()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 150f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 600, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                var qsettings = new Menu("Q Settings", "Q Settings");
                {
                    qsettings.AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
                    qsettings.AddItem(new MenuItem("minqrange", "Minimum Range for Q").SetValue(new Slider(0, 0, 950)));
                    var qblacklist = new Menu("Q Black List", "Q Black List");
                    foreach (var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly
                                        && !x.IsMe))
                    {
                        qblacklist.AddItem(new MenuItem("pullset" + hero.ChampionName, hero.ChampionName))
                            .SetValue(new StringList(new[] { "Pull", "Do Not Pull" }));
                    }
                    qsettings.AddSubMenu(qblacklist);
                }
                comboMenu.AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("rCombo", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("renemyhit", "R Enemy Hit").SetValue(new Slider(2, 1, 5)));
                comboMenu.AddSubMenu(qsettings);
                hLoader.Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            harassMenu.AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("eHarass", "Use E").SetValue(true));
           hLoader.Config.AddSubMenu(harassMenu);

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            clearMenu.AddItem(new MenuItem("qClear", "Use Q").SetValue(true));
            clearMenu.AddItem(new MenuItem("eClear", "Use E").SetValue(true));
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

            hLoader.Config.AddItem(new MenuItem("UseQmanual", "Semi manual Q").SetValue(new KeyBind(66, KeyBindType.Press)));
            hLoader.Config.AddItem(new MenuItem("fleekey", "Use Flee Mode")).SetValue(new KeyBind(65, KeyBindType.Press));

            var drawMenu = new Menu("Draw Settings", "Draw Settings");
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
            Orbwalking.BeforeAttack += Blitz_BeforeAttack;
            CustomEvents.Unit.OnDash += Unit_OnDash;

        }

        internal static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();

            if (hLoader.Config.Item("UseQmanual").GetValue<KeyBind>().Active)
                ManualQ();

            if (hLoader.Config.Item("fleekey").GetValue<KeyBind>().Active)
                Flee();
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy)
                return;

            if (Q.IsReady()
                && args.EndPos.Distance(Player) < Q.Range
                && sender.NetworkId == target.NetworkId)
            {
                var delay = (int)(args.EndTick - Game.Time - Q.Delay - 0.1f);
                if (delay > 0)
                    Utility.DelayAction.Add(delay * 1000, () => Q.Cast(args.EndPos));
                else
                    Q.Cast(args.EndPos);
            }
        }


        private void Blitz_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var useE = hLoader.Config.Item("eCombo").GetValue<bool>();

            if (useE)
            {
                foreach (var enemy in HeroManager.Enemies.Where(hero => E.CanCast(hero)
                                                                        && !hero.HasBuffOfType(BuffType.Knockup)))
                {
                    if (enemy.IsValidTarget(110))
                        E.Cast(enemy);
                }
            }
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
                Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold);
                var targetpos = Drawing.WorldToScreen(sender.Position);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
            }

            if (Q.CanCast(sender))
                Q.Cast(sender);
        }

        private static void Flee()
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (W.IsReady())
                W.Cast();
        }

        private static void ManualQ()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            foreach (var hero in
                HeroManager.Enemies.Where(x => !x.IsDead
                && x.IsChampion()))
            {
                if (hLoader.Config.Item("pullset" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (Q.CanCast(hero)
                        && !Player.IsWindingUp
                        && !Player.IsDashing()
                        && Q.GetPrediction(hero).Hitchance >= HitChance.High
                        && Q.GetPrediction(hero).UnitPosition.Distance(Player.Position) > 200)
                        Q.Cast(hero);
                }
            }
        }

        private static void Combo()
        {
            var useQ = hLoader.Config.Item("qCombo").GetValue<bool>();
            var useR = hLoader.Config.Item("rCombo").GetValue<bool>();
            var rHit = hLoader.Config.Item("renemyhit").GetValue<Slider>().Value;
            var qslider = hLoader.Config.Item("minqrange").GetValue<Slider>().Value;
            var rTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (rTarget == null)
                return;

            foreach (var hero in
                HeroManager.Enemies.Where(x => !x.IsDead
                && x.IsChampion()))
            {
                if (hLoader.Config.Item("pullset" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0
                    && useQ)
                {
                    if (Q.CanCast(hero)
                        && !Player.IsWindingUp
                        && !Player.IsDashing()
                        && Q.GetPrediction(hero).Hitchance >= HitChance.High
                        && Q.GetPrediction(hero).UnitPosition.Distance(Player.Position) > 200
                        && hero.Distance(Player) >= qslider)
                        Q.Cast(hero);
                }
            }

            if (R.IsReady()
                && useR
                && ObjectManager.Player.CountEnemiesInRange(R.Range) >= rHit)
                R.Cast();
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