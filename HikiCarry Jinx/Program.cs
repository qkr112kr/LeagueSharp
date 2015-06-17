using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;

namespace HikiCarry_Jinx
{
    class Program
    {
        public const string ChampionName = "Jinx";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        static List<Spells> SpellListt = new List<Spells>();
        static int Delay = 0;

        public static Obj_AI_Hero tar;

        public static Menu Config;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static Obj_AI_Hero Player = ObjectManager.Player;
        private static Obj_AI_Hero Playerz;
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


            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1450f); 
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 2500f); 

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, 140f, 1700f, false, SkillshotType.SkillshotLine);

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
            Config = new Menu("HikiCarry - Jinx", "HikiCarry - Jinx", true);

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
            Config.SubMenu("Combo").AddItem(new MenuItem("RushQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("combotype", "Q Combo Type").SetValue(new StringList(new[] { "Minigun", "Cannon" })));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushRCombo", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("MaxRDistan", "Max R Distance", true).SetValue(new Slider(1500, 0, 3000)));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //HARASS
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushQHarass", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushWHarass", "Use W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassmana", "Haras Mana Percent").SetValue(new Slider(30, 0, 100)));

            //LANECLEAR
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("RushQClear", "Use Q [Minigun]").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("clearmana", "Clear Mana Percent", true).SetValue(new Slider(2, 1, 5)));

           

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

            Config.AddSubMenu(new Menu("Items", "Items"));
            Config.SubMenu("Items").AddSubMenu(new Menu("BOTRK Settings", "BOTRKset"));
            Config.SubMenu("Items").SubMenu("BOTRKset").AddItem(new MenuItem("useBOTRK", "Use BOTRK").SetValue(true));
            Config.SubMenu("Items").SubMenu("BOTRKset").AddItem(new MenuItem("myhp", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
            Config.SubMenu("Items").SubMenu("BOTRKset").AddItem(new MenuItem("theirhp", "Use if enemy HP < %").SetValue(new Slider(20, 0, 100)));
            Config.SubMenu("Items").AddSubMenu(new Menu("Ghostblade Settings", "GBladeset"));
            Config.SubMenu("Items").SubMenu("GBladeset").AddItem(new MenuItem("useGBLADE", "Use Ghostblade").SetValue(true));

            //MISC
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("inmobileE", "Auto Inmobile Target E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("antigapcloser", "Anti-Gapcloser with E").SetValue(true));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Scrying Orb Settings", "orbset"));
            Config.SubMenu("Misc").SubMenu("orbset").AddItem(new MenuItem("bT", "Auto Scrying Orb Buy!").SetValue(true));
            Config.SubMenu("Misc").SubMenu("orbset").AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
            Config.SubMenu("Misc").AddSubMenu(new Menu("KillSteal Settings", "killsetset"));
            Config.SubMenu("Misc").SubMenu("killsetset").AddItem(new MenuItem("ksW", "KillSteal W!").SetValue(true));






            //DRAWINGS
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushQRange", "Q Range").SetValue(new Circle(true, Color.SkyBlue)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushQRangeX2", "Q Range [Cannon]").SetValue(new Circle(true, Color.Chartreuse)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushWRange", "W Range").SetValue(new Circle(true, Color.Yellow)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushERange", "E Range").SetValue(new Circle(true, Color.SpringGreen)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushRRange", "R Range").SetValue(new Circle(true, Color.Crimson)));
            

            var drawDamageMenu = new MenuItem("RushDrawEDamage", "R Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "R Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
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
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("inmobileE").GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(E.Range))
                {
                    Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                    var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                    Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                }

                if (E.IsReady() && Player.Distance(gapcloser.End, false) <= 200)
                    E.Cast(gapcloser.End);
            }
           
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            tar = (Obj_AI_Hero)target;
            if (Config.Item("useBOTRK").GetValue<bool>() && ((tar.Health / tar.MaxHealth) < Config.Item("theirhp").GetValue<Slider>().Value) && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < Config.Item("myhp").GetValue<Slider>().Value))
            {
                if (Items.CanUseItem(3153))
                {
                    Items.UseItem(3153, tar);
                }
            }
            if (Config.Item("useGBLADE").GetValue<bool>())
            {
                if (Items.CanUseItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }
        static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (R.IsReady())
                damage += R.GetDamage(enemy);

            if (damage > enemy.Health && R.IsReady())
            {
                var yx = Drawing.WorldToScreen(enemy.Position);
                Drawing.DrawText(yx[0], yx[1], System.Drawing.Color.SpringGreen, "ULT = KILL");
            }
            

            return damage;
        }

        public static bool CanMove(Obj_AI_Hero target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell())
            {
                return false;
            }
            else
                return true;
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


            if (Config.Item("ksW").GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
                {
                    if (W.CanCast(target) && W.IsKillable(target))
                        W.Cast(target);
                }
            }

            if (Config.Item("bT").GetValue<bool>() && Player.Level >= Config.Item("bluetrinketlevel").GetValue<Slider>().Value && Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Player.BuyItem(ItemId.Scrying_Orb_Trinket);
            }

            if (Config.Item("inmobileE").GetValue<bool>() && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !CanMove(enemy)))
                {
                    E.Cast(enemy.Position, true);
                    return;
                   
                }
            }

        }


        private static void Combo()
        {
            if (Q.IsReady() && Config.Item("RushQ").GetValue<bool>())
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                float distance = Player.Position.Distance(target.Position);

                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(distance)))
                {
                    switch (Config.Item("combotype").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (Player.HasBuff("JinxQ"))
                            {
                                Q.Cast();
                            }

                            break;

                        case 1:
                            if (Player.HasBuff("JinxQIcon"))
                            {
                                Q.Cast();
                            }

                            break;
                    }



                    
                }

            }

            if (W.IsReady() && Config.Item("RushWCombo").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(W.Range) && W.GetPrediction(hero).Hitchance >= HitChance.VeryHigh))
                {
                    W.Cast(en);
                }

            }

            if (E.IsReady() && Config.Item("RushECombo").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(E.Range) && E.GetPrediction(hero).Hitchance >= HitChance.VeryHigh))
                {
                    E.Cast(en);
                }

            }

            if (R.IsReady() && Config.Item("RushRCombo").GetValue<bool>())
            {
                var distance = Config.Item("MaxRDistan", true).GetValue<Slider>().Value;
                foreach (   
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(distance)))
                {
                    if (!en.IsZombie && en.Health <R.GetDamage(en))
                    {
                        R.Cast(en);
                    }
                }

            }


        }
        private static void Harass()
        {
            if (Q.IsReady() && Config.Item("RushRushQHarass").GetValue<bool>() && Playerz.ManaPercent >= Config.Item("harassmana").GetValue<Slider>().Value)
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                float distance = Player.Position.Distance(target.Position);

                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(distance)))
                {
                   
                            if (Player.HasBuff("JinxQ"))
                            {
                                Q.Cast();
                            }

                }
            }

            if (W.IsReady() && Config.Item("RushWHarass").GetValue<bool>()&& Playerz.ManaPercent >= Config.Item("harassmana").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                foreach (
                   var en in
                       HeroManager.Enemies.Where(
                           hero =>
                               hero.IsValidTarget(W.Range) && W.GetPrediction(hero).Hitchance >= HitChance.VeryHigh))
                {
                    W.Cast(en);
                }

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
            var menuItem5 = Config.Item("RushQRangeX2").GetValue<Circle>();




            if (menuItem1.Active)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.SkyBlue);
            }

            if (menuItem2.Active)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Yellow);
            }
            if (menuItem3.Active)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.SpringGreen);
            }
            if (menuItem4.Active)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Crimson);
            }
            if (menuItem5.Active)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Chartreuse);
            }
        }
    }
}