using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;

namespace HikiCarry_Kalista
{
    class Program
    {
        public const string ChampionName = "Kalista";
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


            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);

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
            Config = new Menu("HikiCarry - Kalista", "HikiCarry - Kalista", true);

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
            Config.SubMenu("Combo").AddItem(new MenuItem("RushECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //HARASS
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushQHarass", "Use Q", true).SetValue(true));

            //LANECLEAR
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("RushEClear", "Use E", true).SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("RushEClearSlider", "If Can Kill Minion >=", true).SetValue(new Slider(2, 1, 5)));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("RushEJClear", "Use E", true).SetValue(true));
            Config.SubMenu("JungleClear").AddSubMenu(new Menu("Mob Steal Set", "mobset"));
            Config.SubMenu("JungleClear").SubMenu("mobset").AddItem(new MenuItem("stealactive", "Steal Active!").SetValue(true));
            Config.SubMenu("JungleClear").SubMenu("mobset").AddItem(new MenuItem("dragonsteal", "Dragon Steal with E!").SetValue(true));
            Config.SubMenu("JungleClear").SubMenu("mobset").AddItem(new MenuItem("baronsteal", "Baron Steal with E!").SetValue(true));
           

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


            
            //MISC
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Scrying Orb Settings", "orbset"));
            Config.SubMenu("Misc").SubMenu("orbset").AddItem(new MenuItem("bT", "Auto Scrying Orb Buy!").SetValue(true));
            Config.SubMenu("Misc").SubMenu("orbset").AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
            Config.SubMenu("Misc").AddSubMenu(new Menu("KillSteal Settings", "killsetset"));
            Config.SubMenu("Misc").SubMenu("killsetset").AddItem(new MenuItem("ksQ", "KillSteal Q!").SetValue(true));
            
            


            

            //DRAWINGS
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushQRange", "Q Range").SetValue(new Circle(true, Color.SkyBlue)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushWRange", "W Range").SetValue(new Circle(true, Color.Yellow)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushERange", "E Range").SetValue(new Circle(true, Color.SpringGreen)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushRRange", "R Range").SetValue(new Circle(true, Color.Crimson)));
          
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "E Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "E Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
            Config.SubMenu("Drawings").AddItem(drawDamageMenu);
            Config.SubMenu("Drawings").AddItem(drawFill);

            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

            drawDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddToMainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }
        static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
           

            if (E.IsReady())
                damage += E.GetDamage(enemy);
            return damage;
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
                JClear();
            }
            
            if (Config.Item("ksQ").GetValue<bool>())
            {
                KillstealQ();
            }

            if (Config.Item("bT").GetValue<bool>() && Player.Level >= Config.Item("bluetrinketlevel").GetValue<Slider>().Value && Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Player.BuyItem(ItemId.Scrying_Orb_Trinket);
            }
            if (Config.Item("stealactive").GetValue<bool>())
            {
                if (Config.Item("dragonsteal").GetValue<bool>() && Config.Item("baronsteal").GetValue<bool>())
                {
                    var minionz = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly);
                    foreach (var m in minionz)
                    {
                        if ((m.BaseSkinName.Contains("Dragon") || m.BaseSkinName.Contains("Baron")) && E.IsKillable(m))
                        {
                            E.Cast();
                        }
                    }
                }

            }
            
        }

        private static void KillstealQ()
        {
            foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
            {
                if (Q.CanCast(target) && Q.GetPrediction(target).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing() && Q.IsKillable(target))
                    Q.Cast(target);
            }
            
        }

       

        private static void Combo()
        {
            if (Q.IsReady() && Config.Item("RushQCombo").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(Q.Range)))
                {
                    var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);
                    if (Q.CanCast(targetQ) && Q.GetPrediction(targetQ).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing())
                        
                        Q.Cast(targetQ);
                }

            }

            if (E.IsReady() && Config.Item("RushECombo").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(E.Range)))
                {
                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && !x.IsMe))
                    {
                        if (hero.Health<E.GetDamage(hero))
                        {
                            E.Cast(hero);
                        }
                    }
                }

            }
            

        }
        private static void Harass()
        {
            if (Q.IsReady() && Config.Item("RushQHarass").GetValue<bool>())
            {
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(Q.Range)))
                {
                    var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);
                    if (Q.CanCast(targetQ) && Q.GetPrediction(targetQ).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing())

                        Q.Cast(targetQ);
                }

            }
        }
        static void Clear()
        {
            var Minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy);

            if (Minions.Count <= 0)
                return;

            if (Config.Item("RushEClear").GetValue<bool>() && E.IsReady())
            {

                var mkc = 0;

                foreach (var minion in Minions.Where(x => E.CanCast(x) && x.Health <= E.GetDamage(x))) { mkc++; }

                if (mkc >= Config.Item("RushEClearSlider", true).GetValue<Slider>().Value)
                    E.Cast();
            }

            
            
           
           
        }

        static void JClear()
        {
            var Mobs = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Mobs.Count <= 0)
                return;

          

            if (Config.Item("RushEJClear", true).GetValue<Boolean>() && E.CanCast(Mobs[0]))
            {
                if (Mobs[0].Health + (Mobs[0].HPRegenRate / 2) <= E.GetDamage(Mobs[0]))
                    E.Cast();
            }


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

        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("RushQRange").GetValue<Circle>();
            var menuItem2 = Config.Item("RushWRange").GetValue<Circle>();
            var menuItem3 = Config.Item("RushERange").GetValue<Circle>();
            var menuItem4 = Config.Item("RushRRange").GetValue<Circle>();





            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.SkyBlue);
            }

            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Yellow);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.SpringGreen);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Crimson);
            }
        }
    }
}
