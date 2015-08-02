using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace doubleH_Support.Champions
{
    public class Skeleton
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        public static Obj_AI_Hero Player;

        public Skeleton()
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
            Drawing.OnDraw += Drawing_OnDraw;

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
            /*soon tm*/

        }
        private static void Clear()
        {

            /*soon tm*/
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