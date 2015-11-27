using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

namespace Hikigaya_Lux.Core
{
    class LuxMenu
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void MenuInit()
        {
            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("w.combo", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("r.combo", "Use R").SetValue(true));
                Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                /*var eToggleMenu = new Menu("E Toggle", "E Toggle");
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValid))
                    {
                        eToggleMenu.AddItem(new MenuItem("toggle."+enemy.ChampionName, "(E) "+enemy.ChampionName).SetValue(true));
                    }
                    
                    harassMenu.AddSubMenu(eToggleMenu);
                }*/

                Config.AddSubMenu(harassMenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("q.clear", "Use Q").SetValue(true));
                clearMenu.AddItem(new MenuItem("e.clear", "Use E").SetValue(true));
                clearMenu.AddItem(new MenuItem("e.minion.hit.count", "(E) Min. Minion Hit").SetValue(new Slider(3, 1, 5)));
                clearMenu.AddItem(new MenuItem("clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                Config.AddSubMenu(clearMenu);
            }
            var qMenu = new Menu("(Q) Settings", "(Q) Settings");
            {
                qMenu.AddItem(new MenuItem("min.q.hit", "Min. (Q) Hit").SetValue(new Slider(1, 1, 2)));
                qMenu.AddItem(new MenuItem("q.hit.chance", "(Q) Hit Chance").SetValue<StringList>(new StringList(Priority.HitchanceNameArray, 2)));
                Config.AddSubMenu(qMenu);
            }
            /*var wMenu = new Menu("(W) Settings", "(W) Settings");
            {
                wMenu.AddItem(new MenuItem("w.ally.protector", "Protector?").SetValue(true));
                wMenu.AddItem(new MenuItem("min.mana.for.w", "Min. Mana").SetValue(new Slider(50,1,99)));
                Config.AddSubMenu(wMenu);
            }*/
            var eMenu = new Menu("(E) Settings", "(E) Settings");
            {
                eMenu.AddItem(new MenuItem("min.e.hit", "Min. (E) Hit").SetValue(new Slider(1, 1, 2)));
                eMenu.AddItem(new MenuItem("e.hit.chance", "(E) Hit Chance").SetValue<StringList>(new StringList(Priority.HitchanceNameArray, 2)));
                Config.AddSubMenu(eMenu);
            }
            var rMenu = new Menu("(R) Settings", "(R) Settings");
            {
                rMenu.AddItem(new MenuItem("r.style.x", "(R) Method").SetValue(new StringList(new[] { "Only Killable", "Hit X Target" })));
                rMenu.AddItem(new MenuItem("min.r.hit.x", "Min. (R) Hit").SetValue(new Slider(2, 1, 5)));
                rMenu.AddItem(new MenuItem("min.r.distance.x", "Min. (R) Distance").SetValue(new Slider(600, 500, 1000)));
                rMenu.AddItem(new MenuItem("r.hit.chance.x", "(R) Hit Chance").SetValue<StringList>(new StringList(Priority.HitchanceNameArray, 2)));
                Config.AddSubMenu(rMenu);
            }
            /*var invMenu = new Menu("Invisible (R)", "Invisible (R)");
            {
                invMenu.AddItem(new MenuItem("hiki.r.style", "(R) Method ").SetValue(new StringList(new[] { "Only Killable" })));
                invMenu.AddItem(
                    new MenuItem("hiki.r.distance", "Min. (R) Distance ").SetValue(new Slider(600, 500, 1000)));
                Config.AddSubMenu(invMenu);
            }*/
            var autoSpell = new Menu("Auto Spell Settings", "Auto Spell Settings");
            {
                autoSpell.AddItem(new MenuItem("auto.q.hit.two.enemy", "Auto (Q) If Hit 2 Enemy").SetValue(true));
                autoSpell.AddItem(new MenuItem("auto.q.if.enemy.immobile", "Auto (Q) If Enemy Immobile").SetValue(true));
                autoSpell.AddItem(new MenuItem("auto.q.if.enemy.killable", "Auto (Q) If Enemy Killable").SetValue(true));

                autoSpell.AddItem(new MenuItem("auto.e.hit.x.enemy", "Auto (E) If Hit X Enemy").SetValue(true));
                autoSpell.AddItem(new MenuItem("auto.min.e.hit", "Min. (E) Hit Count").SetValue(new Slider(2, 1, 5)));
                autoSpell.AddItem(new MenuItem("auto.e.if.enemy.immobile", "Auto (E) If Enemy Immobile").SetValue(true));
                autoSpell.AddItem(new MenuItem("auto.e.if.enemy.killable", "Auto (E) If Enemy Killable").SetValue(true));

                autoSpell.AddItem(new MenuItem("auto.r.if.enemy.killable.r", "Auto (R) If Enemy Killable").SetValue(true));
                autoSpell.AddItem(new MenuItem("auto.r.min.distance", "Min. (R) Distance").SetValue(new Slider(1000, 500, 1000)));
                Config.AddSubMenu(autoSpell);
            }

            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var skillDraw = new Menu("Skill Draws", "Skill Draws");
                {
                    skillDraw.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(skillDraw);
                }

                /*var invisibleMenu = new Menu("Invisible (R) Draws", "Invisible (R) Draws");
                {
                    invisibleMenu.AddItem(new MenuItem("r.text.draw", "Invisible (R) Notification Text Draw ").SetValue(true));
                    invisibleMenu.AddItem(new MenuItem("r.circle", "Invisible (R) Circle").SetValue(true));
                    drawMenu.AddSubMenu(invisibleMenu);
                }
                drawMenu.AddItem(new MenuItem("draw.damage", "Fill Combo Damage").SetValue(true));
                */
                Config.AddSubMenu(drawMenu);
            }

            Config.AddItem(new MenuItem("keysinfo", "                      Hikigaya Lux Keys").SetFontStyle(System.Drawing.FontStyle.Bold, Color.Gold));
            //Config.AddItem(new MenuItem("invisible.active", "Invisible (R)!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            //Config.AddItem(new MenuItem("manual.r.focus.adc", "Semi Manual (R) - (ADC FOCUS) !").SetValue(new KeyBind("S".ToCharArray()[0], KeyBindType.Press)));
            Config.AddItem(new MenuItem("calculator", "Damage Calculator").SetValue(new StringList(new[] { "Custom", "Common" }, 1)));

            Config.AddToMainMenu();
        }
        public static void OrbwalkerInit()
        {
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
        }
    }
}
