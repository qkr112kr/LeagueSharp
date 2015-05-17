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
        static List<Spells> SpellListt = new List<Spells>();

        public static Items.Item sOrb = new Items.Item(3342, 600f);
        static int Delay = 0;
        private static bool Vayne = false;


        public struct Spells
        {
            public string ChampionName;
            public string SpellName;
            public SpellSlot slot;
        }


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

            SpellListt.Add(new Spells { ChampionName = "akali", SpellName = "akalismokebomb", slot = SpellSlot.W });   //Akali W
            SpellListt.Add(new Spells { ChampionName = "shaco", SpellName = "deceive", slot = SpellSlot.Q }); //Shaco Q
            SpellListt.Add(new Spells { ChampionName = "khazix", SpellName = "khazixr", slot = SpellSlot.R }); //Khazix R
            SpellListt.Add(new Spells { ChampionName = "khazix", SpellName = "khazixrlong", slot = SpellSlot.R }); //Khazix R Evolved
            SpellListt.Add(new Spells { ChampionName = "talon", SpellName = "talonshadowassault", slot = SpellSlot.R }); //Talon R
            SpellListt.Add(new Spells { ChampionName = "monkeyking", SpellName = "monkeykingdecoy", slot = SpellSlot.W }); //Wukong W
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
            Config.SubMenu("Harass").AddItem(new MenuItem("RushQHarass", "Use Q", true).SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("RushEHarass", "Use E", true).SetValue(true));


            Config.AddSubMenu(new Menu("Items", "Items"));
            Config.SubMenu("Items").AddItem(new MenuItem("ghost", "Use GhostBlade!").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("agapcloser", "Anti-Gapcloser Active!", true).SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("ainterrupt", "Auto Interrupt Active!", true).SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("antirengo", "Anti Rengar Active!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoRQ", "Autocast Q When Using Ultimate", true).SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("bT", "Auto Scrying Buy Orb!").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
            Config.SubMenu("Misc").AddItem(new MenuItem("onevoneactive", "1v1 Active").SetValue(true));
            
           
            


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

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RushERange", "E Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));



            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            GameObject.OnCreate += OnCreateObject;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            _gapcloseSpell = GetSpell();


            Player = ObjectManager.Player;
            switch (ObjectManager.Player.ChampionName)
            {
                case "Vayne":
                    Vayne = true;
                    break;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
           
              
            if (sender.IsMe && args.SData.Name == "vayneinquisition" && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
            }


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
                    _rengarObj.Distance(Player.Position) <= _gapcloseSpell.Range)
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
            //coming
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
            float rangez = ObjectManager.Player.CountEnemiesInRange(1500);
            var target = TargetSelector.GetTarget(625, TargetSelector.DamageType.Physical);
            var pushDistance = 400;

            if (rangez <= 1 && Config.Item("onevoneactive").GetValue<bool>())
            {
               
                if (E.IsReady())
                {
                    foreach (
                           var qPosition in
                             GetPossibleQPositions()
                             .OrderBy(qPosition => qPosition.Distance(target.ServerPosition)))
                    {
                        if (qPosition.Distance(target.Position) < E.Range)
                            E.UpdateSourcePosition(qPosition, qPosition);
                        var targetPosition = E.GetPrediction(target).CastPosition;
                        var finalPosition = targetPosition.Extend(qPosition, -pushDistance);
                        if (finalPosition.IsWall())
                        {
                            Q.Cast(qPosition);
                            
                        }
                    }
                    foreach (var targetz in HeroManager.Enemies.Where(h => h.IsValidTarget(E.Range) && h.Path.Count() < 2))
                    {

                        E.UpdateSourcePosition(Player.Position, Player.Position);
                        var targetPosition = E.GetPrediction(targetz).CastPosition;
                        var finalPosition = targetPosition.Extend(Player.ServerPosition, -pushDistance);

                        if (finalPosition.IsWall())
                        {
                            Q.Cast(Game.CursorPos);
                            E.Cast(targetz);

                        }
                    }   
                }
               
                var yx = Drawing.WorldToScreen(ObjectManager.Player.Position);
                Drawing.DrawText(yx[0], yx[1], System.Drawing.Color.LimeGreen, "1v1 Mode Active!");


            }
            else
            {
                if (Config.Item("RushECombo").GetValue<bool>() && Config.Item("RushQCombo").GetValue<bool>())
                {
                    if (Q.IsReady() && E.IsReady())
                    {
                        if (target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count >= 2))
                        {
                            if (Items.CanUseItem(3142))
                            {
                                Items.UseItem(3142);
                            }
                            Q.Cast(Game.CursorPos);
                        }
                        foreach (var targetz in HeroManager.Enemies.Where(h => h.IsValidTarget(E.Range) && h.Path.Count() < 2))
                        {
                            var targetPosition = E.GetPrediction(targetz).CastPosition;
                            var finalPosition = targetPosition.Extend(Player.ServerPosition, -pushDistance);
                            if (finalPosition.IsWall())
                            {
                                E.Cast(targetz);
                            }
                        }
                    }
                }
                if (Config.Item("RushQCombo").GetValue<bool>() == false)
                {
                    if (E.IsReady())
                    {
                        foreach (var targetz in HeroManager.Enemies.Where(h => h.IsValidTarget(E.Range) && h.Path.Count() < 2))
                        {
                            var targetPosition = E.GetPrediction(targetz).CastPosition;
                            var finalPosition = targetPosition.Extend(Player.ServerPosition, -pushDistance);
                            if (finalPosition.IsWall())
                            {
                                E.Cast(targetz);
                            }
                        }
                    }
                }
                if (Config.Item("RushECombo").GetValue<bool>() == false)
                {
                    if (target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count >= 2))
                    {
                        if (Items.CanUseItem(3142))
                        {
                            Items.UseItem(3142);
                        }
                        Q.Cast(Game.CursorPos);
                    }
                }
            }
           
        }

        private static IEnumerable<Vector3> GetPossibleQPositions()
        {
            var pointList = new List<Vector3>();

            var j = 300;

            var offset = (int)(2 * Math.PI * j / 100);

            for (var i = 0; i <= offset; i++)
            {
                var angle = i * Math.PI * 2 / offset;
                var point = new Vector3((float)(ObjectManager.Player.Position.X + j * Math.Cos(angle)),
                    (float)(ObjectManager.Player.Position.Y - j * Math.Sin(angle)),
                    ObjectManager.Player.Position.Z);

                if (!NavMesh.GetCollisionFlags(point).HasFlag(CollisionFlags.Wall))
                    pointList.Add(point);
            }


            return pointList;
        }
        public static void Harass()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            var pT = HeroManager.Enemies.Find(enemy => enemy.IsValidTarget(E.Range));

            if (target.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count < 2) && Q.IsReady() && Config.Item("RushQHarass").GetValue<bool>())
            {
                Q.Cast(Game.CursorPos);
            }

            if (E.IsReady() && Config.Item("RushEHarass").GetValue<bool>())
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

            Utility.DrawCircle(Player.Position, 1500, Color.Blue);



            // credits blm95 start
            if (Config.Item("howaa").GetValue<bool>())
            {
                double temp = 0;
                foreach (
                    var c in
                        ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsVisible && x.IsEnemy && x.IsValid))//&& x.Distance(ObjectManager.Player) < 2500))
                {
                    var dmg = Player.GetAutoAttackDamage(c);
                    var howmanyautos = c.Health / dmg;
                    if (howmanyautos > 3 && Vayne && Player.Spellbook.GetSpell(SpellSlot.W).State != SpellState.NotLearned)
                    {
                        var g = new float[] { 0, 20, 30, 40, 50, 60 };
                        var h = new double[] { 0, .04, .05, .06, .07, .08 };
                        howmanyautos = (c.Health) / (dmg + ((g[Player.Spellbook.GetSpell(SpellSlot.W).Level] + (c.MaxHealth * h[Player.Spellbook.GetSpell(SpellSlot.W).Level]))) / 3);
                    }
                    if (howmanyautos >= 10)
                    {
                        Drawing.DrawText(c.HPBarPosition.X, c.HPBarPosition.Y - 22, System.Drawing.Color.Green, "" + "  How Many AA: " + String.Format("{0:0.00}", howmanyautos));
                    }
                    if (howmanyautos < 10)
                    {
                        Drawing.DrawText(c.HPBarPosition.X, c.HPBarPosition.Y - 22, System.Drawing.Color.Red, "" + "  How Many AA: " + String.Format("{0:0.00}", howmanyautos));
                    }
                }
            }
            // finish

        }
        private static void Drawing_OnDraw(EventArgs args)
        {

            var menuItem2 = Config.Item("RushERange").GetValue<Circle>();
            if (menuItem2.Active) Utility.DrawCircle(Player.Position, E.Range, menuItem2.Color);

        }
    }
}
