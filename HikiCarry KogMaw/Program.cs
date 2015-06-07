using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = LeagueSharp.Common.Geometry; 
using Color = System.Drawing.Color; 

namespace HikiCarry_KogMaw
{
    class Program
    {
        public const string ChampionName = "KogMaw"; 
        public static Orbwalking.Orbwalker Orbwalker; 
        public static List<Spell> SpellList = new List<Spell>(); 
        static int Delay = 0; 

        public static Menu Config;

        public static Spell Q; 
        public static Spell W; 
        public static Spell E;
        public static Spell R;
        

        private static Obj_AI_Hero Player; 
       
        static void Main(string[] args)
        {

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad; 
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;


            Q = new Spell(SpellSlot.Q, 980) ;
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 1800);

            Q.SetSkillshot(0.25f, 50f, 2000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);


            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //MENU
            Config = new Menu("HikiCarry - KogMaw", "HikiCarry - KogMaw", true);

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
            Config.SubMenu("Combo").AddItem(new MenuItem("RushWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushRCombo", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //LANECLEAR
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("RushWClear", "Use W").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("clearmana", "Clear Mana Percent").SetValue(new Slider(30, 0, 100)));

            //HARASS
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushWHarass", "Use W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushRHarass", "Use R").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassmana", "Harass Mana Percent").SetValue(new Slider(30, 0, 100)));
            
            //MISC
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksR", "KillSteal R!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AinR", "Auto R Inmobile Target!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("bT", "Auto Buy Scrying Orb!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));

            //DRAWINGS
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushQRange", "Q Range").SetValue(new Circle(true, Color.SkyBlue)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushWRange", "W Range").SetValue(new Circle(true, Color.Yellow)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushERange", "E Range").SetValue(new Circle(true, Color.Navy)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushRRange", "R Range").SetValue(new Circle(true, Color.MediumVioletRed)));
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Q Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "Q Damage Fill").SetValue(new Circle(true, Color.MediumSpringGreen));
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
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem2 = Config.Item("RushQRange").GetValue<Circle>();
            var menuItem3 = Config.Item("RushWRange").GetValue<Circle>();
            var menuItem4 = Config.Item("RushERange").GetValue<Circle>();
            var menuItem5 = Config.Item("RushRRange").GetValue<Circle>();
            float wrange = 760 + 20 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
            float rrange = 800 + 300*ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;




            if (Config.Item("RushQCombo").GetValue<bool>() && Q.IsReady())
            {
                if (menuItem2.Active) Utility.DrawCircle(Player.Position, Q.Range, Color.SkyBlue);
            }

            if (Config.Item("RushWCombo").GetValue<bool>() && W.IsReady())
            {
                if (menuItem3.Active) Utility.DrawCircle(Player.Position, wrange, Color.Yellow);
            }
            if (Config.Item("RushECombo").GetValue<bool>() && E.IsReady())
            {
                if (menuItem3.Active) Utility.DrawCircle(Player.Position, E.Range, Color.Navy);
            }
            if (Config.Item("RushRCombo").GetValue<bool>() && R.IsReady())
            {
                if (menuItem3.Active) Utility.DrawCircle(Player.Position, rrange, Color.MediumVioletRed);
            }
        }

        static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (R.IsReady())
                damage += R.GetDamage(enemy);

            return damage;
        }
        static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
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

            if (Config.Item("AinR").GetValue<bool>())
            {
                foreach (Obj_AI_Hero target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)))
                {
                    if (target != null)
                    {
                        if (R.CanCast(target) && UnitIsImmobileUntil(target) >= R.Delay - 0.5 && R.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                            {
                                if (Player.Buffs.Count(b => b.Name == "kogmawlivingartillery") < 6)
                                {
                                    R.Cast(target);
                                }
                            }
                    }
                }
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
                    var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    if (Q.CanCast(targetQ))
                    {
                        Q.Cast(targetQ);
                    }
                }
            }
            if (W.IsReady() && Config.Item("RushWCombo").GetValue<bool>())
            {
                float wrange = 760 + 20 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
                foreach (
                    var en in
                        HeroManager.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(wrange)))
                {
                    var targetW = TargetSelector.GetTarget(wrange, TargetSelector.DamageType.Physical);
                    if (W.CanCast(targetW))
                    {
                        W.Cast(targetW);
                    }
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
                    var targetE = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    if (E.CanCast(targetE))
                    {
                        E.Cast(targetE);
                    }
                }
            }
            
            if (R.IsReady() && Config.Item("RushRCombo").GetValue<bool>() && Player.Buffs.Count(x => x.Name == "kogmawlivingartillery") < 6)
            {
                
                foreach (
                    var en in
                        HeroManager.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(800 + 300 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level)))
                {
                    
                   var targetR = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                   var Rprediction = R.GetPrediction(targetR);
                   if (R.CanCast(targetR) && Rprediction.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast(targetR);
                    }
                }
            }
        }
        private static void Harass()
        {
            if (W.IsReady() && Config.Item("RushWHarass").GetValue<bool>() && Player.ManaPercent >= Config.Item("harassmana").GetValue<Slider>().Value)
            {
                float wrange = 760 + 20 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
                foreach (
                    var en in
                        HeroManager.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(wrange)))
                {
                    var targetW = TargetSelector.GetTarget(wrange, TargetSelector.DamageType.Physical);
                    if (W.CanCast(targetW))
                    {
                        W.Cast(targetW);
                    }
                }
            }
            if (R.IsReady() && Config.Item("RushRHarass").GetValue<bool>() && Player.Buffs.Count(x => x.Name == "kogmawlivingartillery") < 6 && Player.ManaPercent >= Config.Item("harassmana").GetValue<Slider>().Value)
            {

                foreach (
                    var en in
                        HeroManager.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(800 + 300 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level)))
                {

                    var targetR = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    var Rprediction = R.GetPrediction(targetR);
                    if (R.CanCast(targetR) && Rprediction.Hitchance >= HitChance.High)
                    {
                        R.Cast(targetR);
                    }
                }
            }
        }
        private static void Clear()
        {
            

            if (W.IsReady() && Config.Item("RushWClear").GetValue<bool>() && Player.ManaPercent >= Config.Item("clearmana").GetValue<Slider>().Value)
            {
                var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 1700, MinionTypes.All);
                var Wfarm = W.GetCircularFarmLocation(allMinionsW, 200);
                if (allMinionsW.Where(x => x.IsValidTarget(760 + 20 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level + 100)).Count() >= 3)
                    W.Cast();
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
