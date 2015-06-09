using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = LeagueSharp.Common.Geometry;
using Color = System.Drawing.Color;

namespace HikiCarry_Graves
{
    class Program
    {
        public const string ChampionName = "Graves";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        static List<Spells> SpellListt = new List<Spells>();
        static int Delay = 0;

        public static Menu Config;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        private static Obj_AI_Hero Player;
        public struct Spells
        {
            public string ChampionName;
            public string SpellName;
            public SpellSlot slot;
        }
        static void Main(string[] args)
        {

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;


            Q = new Spell(SpellSlot.Q, 720f);
            W = new Spell(SpellSlot.W, 850f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1100f);

            Q.SetSkillshot(0.25f, 15f * (float)Math.PI / 180, 2000f, false, SkillshotType.SkillshotCone);
            W.SetSkillshot(0.25f, 250f, 1650f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            SpellListt.Add(new Spells { ChampionName = "akali", SpellName = "akalismokebomb", slot = SpellSlot.W });   //Akali W
            SpellListt.Add(new Spells { ChampionName = "shaco", SpellName = "deceive", slot = SpellSlot.Q }); //Shaco Q
            SpellListt.Add(new Spells { ChampionName = "khazix", SpellName = "khazixr", slot = SpellSlot.R }); //Khazix R
            SpellListt.Add(new Spells { ChampionName = "khazix", SpellName = "khazixrlong", slot = SpellSlot.R }); //Khazix R Evolved
            SpellListt.Add(new Spells { ChampionName = "talon", SpellName = "talonshadowassault", slot = SpellSlot.R }); //Talon R
            SpellListt.Add(new Spells { ChampionName = "monkeyking", SpellName = "monkeykingdecoy", slot = SpellSlot.W }); //Wukong W

            //MENU
            Config = new Menu("HikiCarryGraves", "HikiCarry - Graves", true);

            //TARGET SELECTOR
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //ORBWALKER
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //INFO
            Config.AddSubMenu(new Menu("Info", "Info"));
            Config.SubMenu("Info").AddItem(new MenuItem("Author", "@Hikigaya"));

            //COMBO
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushQComboRange", "Use Q Combo Range").SetValue(new Slider(500, 0, 720)));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushRCombo", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("combotype", "Combo Mode").SetValue(new StringList(new[] { "E+W+Q", "W+E+Q"})));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //LANECLEAR
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("RushQClear", "Use Q", true).SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("clearmana", "Clear Mana Percent").SetValue(new Slider(30, 0, 100)));
           
            //INVISIBLE KICKER
            Config.AddSubMenu(new Menu("Invisible Kicker", "Invisiblez"));
            Config.SubMenu("Invisiblez").AddItem(new MenuItem("Use", "Use Vision Ward On Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                {
                    foreach (var spell in SpellListt.Where(x => x.ChampionName.ToLower() == hero.ChampionName.ToLower()))
                    {
                        Config.SubMenu("Invisiblez").AddItem(new MenuItem(hero.ChampionName.ToLower() + spell.slot.ToString(), hero.ChampionName + " - " + spell.slot.ToString()).SetValue(true));
                    }
                }

                if (HeroManager.Enemies.Any(x => x.ChampionName.ToLower() == "rengar"))
                {
                    Config.SubMenu("Invisiblez").AddItem(new MenuItem("RengarR", "Rengar R").SetValue(true));
                }


            }


            //HARASS
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushQHarass", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassmana", "Haras Mana Percent").SetValue(new Slider(30, 0, 100)));
            //MISC
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksR", "KillSteal R!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("bT", "Auto Scrying Orb!").SetValue(true));
           // Config.SubMenu("Misc").AddItem(new MenuItem("qEQ", "Quick E+Q").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Misc").AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));

            //DRAWINGS
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushQRange", "Max Q Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushQSelectRange", "Selected Q Range").SetValue(new Circle(true, Color.Gold)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushWRange", "W Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushRRange", "R Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));



            Config.AddToMainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += Orbwalking_OnAfterAttack;
           



        }

        private static void Orbwalking_OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Config.Item("RushQCombo").GetValue<bool>())
            {
                if (unit.IsMe && Q.IsReady())
                {
                    var targetQ = TargetSelector.GetTarget(Config.Item("RushQComboRange").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);
                    Q.Cast(targetQ);
                    Orbwalking.ResetAutoAttackTimer();
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (!Config.Item("Use").GetValue<KeyBind>().Active)
                return;

            var Rengar = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "rengar");

            if (Rengar == null)
                return;

            if (!Config.Item("RengarR").GetValue<bool>())
                return;

            if (ObjectManager.Player.Distance(sender.Position) < 1500)
            {
                Console.WriteLine("Sender : " + sender.Name);
            }

            if (sender.IsEnemy && sender.Name.Contains("Rengar_Base_R_Alert"))
            {
                if (ObjectManager.Player.HasBuff("rengarralertsound") &&
                !CheckWard() &&
                !Rengar.IsVisible &&
                !Rengar.IsDead &&
                    CheckSlot() != SpellSlot.Unknown)
                {
                    ObjectManager.Player.Spellbook.CastSpell(CheckSlot(), ObjectManager.Player.Position);
                }
            }
        }
        static SpellSlot CheckSlot()
        {
            SpellSlot slot = SpellSlot.Unknown;

            if (Items.CanUseItem(3362) && Items.HasItem(3362, ObjectManager.Player))
            {
                slot = SpellSlot.Trinket;
            }
            else if (Items.CanUseItem(2043) && Items.HasItem(2043, ObjectManager.Player))
            {
                slot = ObjectManager.Player.GetSpellSlot("VisionWard");
            }
            return slot;
        }

        static bool CheckWard()
        {
            var status = false;

            foreach (var a in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name == "VisionWard"))
            {
                if (ObjectManager.Player.Distance(a.Position) < 450)
                {
                    status = true;
                }
            }

            return status;
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Config.Item("Use").GetValue<KeyBind>().Active)
                return;

            if (!sender.IsEnemy || sender.IsDead || !(sender is Obj_AI_Hero))
                return;

            if (SpellListt.Exists(x => x.SpellName.Contains(args.SData.Name.ToLower())))
            {
                var _sender = sender as Obj_AI_Hero;

                if (!Config.Item(_sender.ChampionName.ToLower() + _sender.GetSpellSlot(args.SData.Name).ToString()).GetValue<bool>())
                    return;

                if (CheckSlot() == SpellSlot.Unknown)
                    return;

                if (CheckWard())
                    return;

                if (ObjectManager.Player.Distance(sender.Position) > 700)
                    return;

                if (Environment.TickCount - Delay > 1500 || Delay == 0)
                {
                    var pos = ObjectManager.Player.Distance(args.End) > 600 ? ObjectManager.Player.Position : args.End;
                    ObjectManager.Player.Spellbook.CastSpell(CheckSlot(), pos);
                    Delay = Environment.TickCount;
                }
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(true);



            //COMBO
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
               Clear();
            }
            if (Config.Item("ksR").GetValue<bool>())
            {
                Killsteal();
            }

            if (Config.Item("bT").GetValue<bool>() && Player.Level >= Config.Item("bluetrinketlevel").GetValue<Slider>().Value && Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Player.BuyItem(ItemId.Scrying_Orb_Trinket);
            }
          /* if (Config.Item("qEQ").GetValue<KeyBind>().Active)
            {
                Orbwalking.MoveTo(Game.CursorPos);
                if (E.IsReady() && Q.IsReady())
                {
                    foreach (
                        var en in
                            HeroManager.Enemies.Where(
                                hero =>
                                    hero.IsValidTarget(Q.Range)))
                    {
                        E.Cast(Game.CursorPos);
                        var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                        Q.Cast(targetQ);
                    }


                }
                


            }*/

        }

        private static void Combo()
        {
           switch (Config.Item("combotype").GetValue<StringList>().SelectedIndex)
            {
                case 0:

            if (E.IsReady() && Config.Item("RushECombo").GetValue<bool>())
            {
                foreach (
                    var en in
                        HeroManager.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(Q.Range)))
                {
                    E.Cast(Game.CursorPos);
                }
            }

            if (W.IsReady() && Config.Item("RushWCombo").GetValue<bool>())
            {
                var targetW = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                W.Cast(targetW);

            }

            if (Q.IsReady() && Config.Item("RushQCombo").GetValue<bool>())
            {
                var targetQ = TargetSelector.GetTarget(Config.Item("RushQComboRange").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);
                Q.Cast(targetQ);

            }

            if (R.IsReady() && Config.Item("RushRCombo").GetValue<bool>())
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero =>hero.IsValidTarget(R.Range) && ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R, 1) - 20 > hero.Health))
                    R.Cast(hero, false, true);
            }
            break;
                case 1:

            if (W.IsReady() && Config.Item("RushWCombo").GetValue<bool>())
            {
                var targetW = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                W.Cast(targetW);

            }
            if (E.IsReady() && Config.Item("RushECombo").GetValue<bool>())
            {
                foreach (
                    var en in
                        HeroManager.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(Q.Range)))
                {
                    E.Cast(Game.CursorPos);
                }
            }
            if (Q.IsReady() && Config.Item("RushQCombo").GetValue<bool>())
            {
                var targetQ = TargetSelector.GetTarget(Config.Item("RushQComboRange").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);
                Q.Cast(targetQ);

            }
            if (R.IsReady() && Config.Item("RushRCombo").GetValue<bool>())
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Range) && ObjectManager.Player.GetSpellDamage(hero, SpellSlot.R, 1) - 20 > hero.Health))
                    R.Cast(hero, false, true);
            }

            break;
            }
        }
        private static void Harass()
        {
            if (Q.IsReady() && Config.Item("RushQHarass").GetValue<bool>() && Player.ManaPercent >= Config.Item("harassmana").GetValue<Slider>().Value)
            {
                var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                Q.Cast(targetQ);
            }
        }
        private static void Clear()
        {
            //THANKS SEBBY 
            if (Q.IsReady() && Config.Item("RushQClear").GetValue<bool>() && Player.ManaPercent >= Config.Item("clearmana").GetValue<Slider>().Value)
            {
                var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All);
                var Qfarm = Q.GetCircularFarmLocation(allMinionsQ, 200);
                if (Qfarm.MinionsHit > 3 && Q.IsReady())
                    Q.Cast(Qfarm.Position);
            }
           
        }

     
      

       
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem2 = Config.Item("RushQRange").GetValue<Circle>();
            var menuItem3 = Config.Item("RushWRange").GetValue<Circle>();
            var menuItem4 = Config.Item("RushRRange").GetValue<Circle>();
            var menuItem5 = Config.Item("RushQSelectRange").GetValue<Circle>();
            

            if (Config.Item("RushQCombo").GetValue<bool>() && Q.IsReady())
            {
                if (menuItem2.Active) Utility.DrawCircle(Player.Position, Q.Range, Color.HotPink);
            }

            if (Config.Item("RushWCombo").GetValue<bool>() && W.IsReady())
            {
                if (menuItem3.Active) Utility.DrawCircle(Player.Position, W.Range, Color.LightSeaGreen);
            }
            if (Config.Item("RushRCombo").GetValue<bool>() && R.IsReady())
            {
                if (menuItem4.Active) Utility.DrawCircle(Player.Position, R.Range, Color.GreenYellow);
            }
            if (Config.Item("RushQCombo").GetValue<bool>() && Q.IsReady())
            {
                if (menuItem5.Active) Utility.DrawCircle(Player.Position, Config.Item("RushQComboRange").GetValue<Slider>().Value, Color.Gold);
            }

           
        }

        private static void Killsteal()
        {
            foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
            {
                if (R.CanCast(target) && R.IsKillable(target))
                    R.Cast(target);
            }
        }



    }
}
