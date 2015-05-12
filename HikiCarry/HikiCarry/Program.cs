using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = LeagueSharp.Common.Geometry;
using Color = System.Drawing.Color;



namespace HikiCarry
{
    internal class Program
    {
        public const string ChampionName = "Vayne";

        //Orbwalker
        public static Orbwalking.Orbwalker Orbwalker;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static bool PacketCast;
        public static float LastMoveC;
        private static Spell _gapcloseSpell;
        private static Obj_AI_Hero _rengarObj;
        



        //Menu
        public static Menu Config;
        public static Obj_AI_Hero tar;
        private static Obj_AI_Hero Player;
     
       

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;


            //Create Spells
            Q = new Spell(SpellSlot.Q, 300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 550f);
            E.SetTargetted(0.25f, 1600f);
            R = new Spell(SpellSlot.R);


            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);





            //Menu
            Config = new Menu("HikiCarryVayne", "HikiCarry - Vayne", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            //Add orbwalker
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Info", "Info"));
            Config.SubMenu("Info").AddItem(new MenuItem("Author", "@Hikigaya"));


            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RushECombo", "Use E").SetValue(true));
            
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
                




            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushEHarass", "Use E", true).SetValue(true));


            Config.AddSubMenu(new Menu("Items", "Items"));
            Config.SubMenu("Items").AddItem(new MenuItem("ghost", "Use GhostBlade!").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("agapcloser", "Anti-Gapcloser Active!", true).SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("ainterrupt", "Auto Interrupt Active!", true).SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("antirengo", "Anti Rengar Active!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("bT", "Auto Scrying Orb!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
            Config.SubMenu("Misc").AddItem(new MenuItem("ARQ", "Autocast Q When Ultimate!", true).SetValue(true));






            Config.AddSubMenu(new Menu("Drawings", "Drawings"));

            Config.SubMenu("Drawings").AddItem(new MenuItem("RushERange", "E Range").SetValue(new Circle(true,System.Drawing.Color.FromArgb(255, 255, 255, 255))));



            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            _gapcloseSpell = GetSpell();
            GameObject.OnCreate += OnCreateObject;
           
          
        }

       

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender.IsEnemy)
            {
                foreach (Obj_AI_Hero enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(hero => hero.IsValidTarget(1500) && hero.ChampionName == "Rengar"))
                {
                    _rengarObj = enemy;
                }
            }
            if (_rengarObj != null && Player.Distance(_rengarObj, true) < 1000 * 1000 &&
                Config.Item("antirengo").GetValue<bool>())
            {
                DoButtFuck();
            }

            
        }

        private static void DoButtFuck()
        {
            if (_rengarObj.ChampionName == "Rengar")
            {
                if (_rengarObj.IsValidTarget(_gapcloseSpell.Range) && _gapcloseSpell.IsReady() &&
                    _rengarObj.Distance(Player) <= _gapcloseSpell.Range)
                {
                    _gapcloseSpell.Cast(_rengarObj);
                    
                }
            }
        }
      

        private static Spell GetSpell()
        {
            switch (Player.ChampionName)
            {
                case "Vayne":
                    return new Spell(SpellSlot.E, 550);
              
            }
            return null;
        }

        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            //credits xcsoft

            if (Config.Item("ainterrupt", true).GetValue<Boolean>() || Player.IsDead)
                return;

            if (sender.IsValidTarget(1000))
            {
                Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius, Color.Gold, 5);
                var targetpos = Drawing.WorldToScreen(sender.Position);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Interrupt");
            }

            if (E.CanCast(sender))
                E.Cast(sender);
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Config.Item("ARQ", true).GetValue<Boolean>())
            {
                if (sender.IsMe && args.SData.Name == "vayneinquisition" && Q.IsReady())
                    Q.Cast(Game.CursorPos);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            // credits xcsoft 

            if (Config.Item("antigapcloser", true).GetValue<Boolean>() || Player.IsDead)

                return;

            if (gapcloser.Sender.IsValidTarget(1000))
            {
                Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
            }

            if (E.CanCast(gapcloser.Sender))
                E.Cast(gapcloser.Sender);
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {



        }


        private static bool IsAllyFountain(Vector3 position)
        {
            float fountainRange = 750;
            var map = Utility.Map.GetMap();
            if (map != null && map.Type == Utility.Map.MapType.SummonersRift)
            {
                fountainRange = 1050;
            }
            return
                ObjectManager.Get<GameObject>()
                    .Where(spawnPoint => spawnPoint is Obj_SpawnPoint && spawnPoint.IsAlly)
                    .Any(spawnPoint => Vector2.Distance(position.To2D(), spawnPoint.Position.To2D()) < fountainRange);
        }

        public static void Combo()
        {
           
              var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

           
        // hikigaya normal combo start
          

                if (target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2) && Q.IsReady())
                {
                     if (Items.CanUseItem(3142))
                      {
                          Items.UseItem(3142);
                      }
                    Q.Cast(Game.CursorPos);
                }

                if (E.IsReady() && Config.Item("RushECombo").GetValue<bool>())
                {
                    foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range) && !hero.HasBuffOfType(BuffType.SpellShield) && !hero.HasBuffOfType(BuffType.SpellImmunity)))
                    {
                        //credits VayneHunterRework

                        var ePred = E.GetPrediction(en);
                        int pushDist = 425;
                        var FinalPosition = ePred.UnitPosition.To2D().Extend(Player.ServerPosition.To2D(), -pushDist).To3D();

                        for (int i = 1; i < pushDist; i += (int)en.BoundingRadius)
                        {
                            Vector3 loc3 = ePred.UnitPosition.To2D().Extend(Player.ServerPosition.To2D(), -i).To3D();

                            if (loc3.IsWall() || IsAllyFountain(FinalPosition))
                                E.Cast(en);
                        }
                    }
                }
                else
                {
                    if (target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2) && Q.IsReady())
                    {
                        Q.Cast(Game.CursorPos);
                    }

                    if (E.IsReady() && Config.Item("RushECombo").GetValue<bool>())
                    {
                        foreach (var en in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range) && !hero.HasBuffOfType(BuffType.SpellShield) && !hero.HasBuffOfType(BuffType.SpellImmunity)))
                        {
                            //credits VayneHunterRework

                            var ePred = E.GetPrediction(en);
                            int pushDist = 425;
                            var FinalPosition = ePred.UnitPosition.To2D().Extend(Player.ServerPosition.To2D(), -pushDist).To3D();

                            for (int i = 1; i < pushDist; i += (int)en.BoundingRadius)
                            {
                                Vector3 loc3 = ePred.UnitPosition.To2D().Extend(Player.ServerPosition.To2D(), -i).To3D();

                                if (loc3.IsWall() || IsAllyFountain(FinalPosition))
                                    E.Cast(en);
                            }
                        }
                    }
                }
            

           

           
            
                


        
        }
        public  static void Harass()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            var pT = HeroManager.Enemies.Find(enemy => enemy.IsValidTarget(E.Range));

            if (target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2) && E.IsReady())
            {
                if (pT != null && (pT is Obj_AI_Hero))
                {
                    E.Cast(pT);
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
               
                // maybe coming
            }


            if (Config.Item("bT").GetValue<bool>() && Player.Level >= Config.Item("bluetrinketlevel").GetValue<Slider>().Value && Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Player.BuyItem(ItemId.Scrying_Orb_Trinket);
            }

           

         

        }
      

        


        private static void Drawing_OnDraw(EventArgs args)
        {

            var menuItem2 = Config.Item("RushERange").GetValue<Circle>();
            if (menuItem2.Active) Utility.DrawCircle(Player.Position, E.Range, menuItem2.Color);

        }
    }
}