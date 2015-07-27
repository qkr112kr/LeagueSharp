using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace HikiCarry_Elise
{
    class Program
    {
        public const string ChampionName = "Elise";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Menu Config;
        private static Obj_AI_Hero Player = ObjectManager.Player;
        private static SpellSlot Flash;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell SpiderQ;
        public static Spell SpiderW;
        public static Spell SpiderE;

        public static float rangeQ;
        public static float rangeW;
        public static float rangeE;

        private static Items.Item IronSolari; 
        private static Items.Item Zhonya;
        private static Items.Item Randuin;

        private static bool humansexygirl;
        private static bool spidergirl;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1075f);
            R = new Spell(SpellSlot.R,0);
            SpiderQ = new Spell(SpellSlot.Q, 475f);
            SpiderW = new Spell(SpellSlot.W);
            SpiderE = new Spell(SpellSlot.E, 750f);

            W.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 55f, 1300, true, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            SpellList.Add(SpiderQ);
            SpellList.Add(SpiderW);
            SpellList.Add(SpiderE);

            Zhonya = new Items.Item(3157, 10);
            IronSolari = new Items.Item(3190, 590f);
            Randuin = new Items.Item(3143, 490f);

            Config = new Menu("HikiCarry - Elise", "HikiCarry - Elise", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Orbwalker Settings", "Orbwalker Settings"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            Config.AddSubMenu(new Menu("Combo Settings", "Combo Settings"));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("qCombo", "Use Q [Human]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("wCombo", "Use W [Human]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("eCombo", "Use E [Human]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("rCombo", "Use R [Auto Switch]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("qCombo.Spider", "Use Q [Spider]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("wCombo.Spider", "Use W [Spider]").SetValue(true));
            Config.SubMenu("Combo Settings").AddItem(new MenuItem("eCombo.Spider", "Use E [Spider]").SetValue(true));


            Config.AddSubMenu(new Menu("Harass Settings", "Harass Settings"));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("qHarass", "Use Q [Human]").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("wHarass", "Use W [Human]").SetValue(true));
            Config.SubMenu("Harass Settings").AddItem(new MenuItem("manaHarass", "Harass Mana Percent").SetValue(new Slider(30, 1, 100)));

            Config.AddSubMenu(new Menu("Jungle Settings", "Jungle Settings"));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("qjClear", "Use Q [Human]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("wjClear", "Use W [Human]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("qjClear.Spider", "Use Q [Spider]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("wjClear.Spider", "Use W [Spider]").SetValue(true));
            Config.SubMenu("Jungle Settings").AddItem(new MenuItem("autoswitch", "Auto Switch").SetValue(true));

            Config.AddSubMenu(new Menu("Misc Settings", "Misc Settings"));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("agapcloser", "Anti-Gapcloser [Human E]").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("agapcloser2", "Anti-Gapcloser [Spider E]").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("ainterrupt", "Auto Interrupt Active! [Human E]").SetValue(true));
            Config.SubMenu("Misc Settings").AddItem(new MenuItem("ainterrupt2", "Auto Interrupt Active! [Spider E]").SetValue(true));

            Config.AddSubMenu(new Menu("Items Settings", "Items Settings"));
            //////////
            Config.SubMenu("Items Settings").AddSubMenu(new Menu("Randuin Omen Settings", "Randuin Omen Settings"));
            Config.SubMenu("Items Settings").SubMenu("Randuin Omen Settings").AddItem(new MenuItem("useRanduin", "Use Randuin").SetValue(true));
            Config.SubMenu("Items Settings").SubMenu("Randuin Omen Settings").AddItem(new MenuItem("randuinCount", "If Enemy Count >=").SetValue(new Slider(2, 1, 5)));
            /////////
            Config.SubMenu("Items Settings").AddSubMenu(new Menu("Zhonya Settings", "Zhonya Settings"));
            Config.SubMenu("Items Settings").SubMenu("Zhonya Settings").AddItem(new MenuItem("useZhonya", "Use Zhonya").SetValue(true));
            Config.SubMenu("Items Settings").SubMenu("Zhonya Settings").AddItem(new MenuItem("zhonyaMyHp", "If My Hp >= %").SetValue(new Slider(10, 0, 100)));
            ///////////
            Config.SubMenu("Items Settings").AddSubMenu(new Menu("Iron Solari Settings", "Iron Solari Settings"));
            Config.SubMenu("Items Settings").SubMenu("Iron Solari Settings").AddItem(new MenuItem("useSolari", "Use Iron Solari").SetValue(true));
            Config.SubMenu("Items Settings").SubMenu("Iron Solari Settings").AddItem(new MenuItem("ironsolariAllyHp", "If Ally Hp >= %").SetValue(new Slider(20, 0, 100)));

            Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.White)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.White)));
            Config.SubMenu("Draw Settings").AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.White)));

            Config.AddItem(new MenuItem("elisebois", "             E L I S E B O I S"));
            Config.AddItem(new MenuItem("hitE", "E HitChance").SetValue(new StringList(new[] { HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(),
                    HitChance.VeryHigh.ToString() },4)));

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (humansexygirl)
            {
                if (Config.Item("ainterrupt").GetValue<bool>())
                {
                    if (sender.IsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
                    }
                    if (E.CanCast(sender))
                    {
                        E.Cast(sender);
                    }
                }
            }
            if (spidergirl)
            {
                if (Config.Item("ainterrupt2").GetValue<bool>())
                {
                    if (sender.IsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
                    }
                    if (SpiderE.CanCast(sender))
                    {
                        SpiderE.Cast(sender);
                    }
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (humansexygirl)
            {
                if (Config.Item("agapcloser").GetValue<bool>())
                {
                    if (gapcloser.Sender.IsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                    }
                    if (E.CanCast(gapcloser.Sender))
                    {
                        E.Cast(gapcloser.Sender);
                    }
                }
            }
            if (spidergirl)
            {
                if (Config.Item("agapcloser2").GetValue<bool>())
                {
                    if (gapcloser.Sender.IsValidTarget(1000))
                    {
                        Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                        var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                        Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                    }
                    if (E.CanCast(gapcloser.Sender))
                    {
                        E.Cast(gapcloser.Sender);
                    }
                } 
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            spiderCheck();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Jungle();
            }
            Items();
        }
        private static void Items()
        {
            if (Config.Item("useRanduin").GetValue<bool>())
            {
                if (Player.CountEnemiesInRange(400) >= Config.Item("randuinCount").GetValue<Slider>().Value)
                {
                    Randuin.Cast();
                }
            }
            if (Config.Item("useZhonya").GetValue<bool>())
            {
                if (Player.HealthPercent <= Config.Item("zhonyaMyHp").GetValue<Slider>().Value)
                {
                    Zhonya.Cast();
                }
            }
            if (Config.Item("useSolari").GetValue<bool>())
            {
                foreach (var ally in HeroManager.Allies)
                {
                    if (!ally.IsMe && !ally.IsRecalling() && ally.HealthPercent <= Config.Item("ironsolariAllyHp").GetValue<Slider>().Value)
                    {
                        IronSolari.Cast();
                    }
                }
            }
        }
        private static void Jungle()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs == null || (mobs != null && mobs.Count == 0))
            {
                return;
            }
            if (humansexygirl)
            {
                    if (Q.IsReady() && Config.Item("qjClear").GetValue<bool>())
                    {
                        Q.Cast(mobs[0]);
                    }
                    if (W.IsReady() && Config.Item("wjClear").GetValue<bool>())
                    {
                        W.Cast(mobs[0]);
                    }
                    if (!Q.IsReady() && !W.IsReady() && Config.Item("autoswitch").GetValue<bool>())
                    {
                        R.Cast();
                    }
            }
            if (spidergirl)
            {
                     if (W.IsReady() && Config.Item("wjClear.Spider").GetValue<bool>())
                     {
                         W.Cast();
                     }
                     if (Q.IsReady() && Config.Item("qjClear.Spider").GetValue<bool>())
                     {
                         Q.Cast(mobs[0]);
                     }
                     if (!SpiderQ.IsReady() && !SpiderW.IsReady() && Config.Item("autoswitch").GetValue<bool>())
                     {
                         R.Cast();
                     }
            }
        }
        private static HitChance preE(string name)
        {
            var qpred = Config.Item(name).GetValue<StringList>();
            switch (qpred.SList[qpred.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }
        private static void Combo()
        {
            if (humansexygirl)
            {
                if (E.IsReady() && Config.Item("eCombo").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range)))
                    {
                        if (E.CanCast(en) && E.GetPrediction(en).Hitchance >= preE("hitE") && !Player.IsWindingUp && !Player.IsDashing())
                        {
                            E.Cast(en);
                        }
                    }
                }
                if (Q.IsReady() && Config.Item("qCombo").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                    {
                        if (Q.CanCast(en))
                        {
                            Q.Cast(en);
                        }
                    }
                }
                if (W.IsReady() && Config.Item("wCombo").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player))))
                    {
                        if (W.CanCast(en))
                        {
                            W.Cast(en);
                        }
                    }
                }
                if (!Q.IsReady() && !W.IsReady() && !E.IsReady() && R.IsReady() &&
                    Config.Item("rCombo").GetValue<bool>())
                {
                    R.Cast();
                }
            }
            if (spidergirl)
            {
                if (SpiderW.IsReady() && Config.Item("wCombo.Spider").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                    {
                        W.Cast();
                    }
                }
                if (SpiderQ.IsReady() && Config.Item("qCombo.Spider").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                    {
                        Q.Cast(en);
                    }
                }
                if (SpiderE.IsReady() && Config.Item("qCombo.Spider").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range)))
	                {
                        if (Player.Distance(en, true) <= SpiderE.Range && Player.Distance(en, true) > SpiderQ.Range &&
                        Config.Item("eCombo.Spider").GetValue<bool>() && SpiderE.IsReady())
                        {
                            E.Cast(en);
                        }
                        if (Player.Distance(en, true) <= SpiderE.Range && Player.Distance(en, true) > SpiderQ.Range &&
                        Config.Item("eCombo.Spider").GetValue<bool>() && SpiderE.IsReady() && Player.CountAlliesInRange(E.Range) == 1 && en.HealthPercent < 5)
                        {
                            E.Cast(en);
                        }
	                }
                }
                if (!SpiderQ.IsReady() && !SpiderW.IsReady() && R.IsReady() &&
                    Config.Item("rCombo").GetValue<bool>())
                {
                    R.Cast();
                }
            }
            
        }
        private static void Harass()
        {
            if (humansexygirl)
            {
                if (ObjectManager.Player.ManaPercent > Config.Item("manaHarass").GetValue<Slider>().Value)
                {
                    if (Q.IsReady() && Config.Item("qHarass").GetValue<bool>())
                    {
                        foreach (Obj_AI_Hero qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                        {
                            Q.Cast(qTarget);
                        }
                    }
                    if (W.IsReady() && Config.Item("wHarass").GetValue<bool>())
	                {
                        foreach (Obj_AI_Hero wTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
	                    {
                            if (W.CanCast(wTarget) && W.GetPrediction(wTarget).Hitchance >= HitChance.High && !Player.IsWindingUp && !Player.IsDashing())
                            {
                                W.Cast(wTarget);
                            }
	                    }
	                }
                }
            }
        }
        private static void spiderCheck()
        {
            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW" ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE")
            {
                spidergirl    = false;
                humansexygirl = true;
                rangeQ = Q.Range;
                rangeW = W.Range;
                rangeE = E.Range;
                
            }
            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW"     ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial")
            {
                spidergirl    = true;
                humansexygirl = false;
                rangeQ = SpiderQ.Range;
                rangeW = SpiderW.Range;
                rangeE = SpiderE.Range;
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = Config.Item("eDraw").GetValue<Circle>();

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, rangeQ, Color.SpringGreen);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, rangeW , Color.White);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, rangeW, Color.White);
            }
        }
    }
}
