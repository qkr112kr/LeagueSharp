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
        public static string cName = "Kalista";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Menu Config;
        private static Obj_AI_Hero Player = ObjectManager.Player;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;


        public static string[] Marksman = { "Kalista", "Jinx", "Lucian", "Quinn", "Draven",  "Varus", "Graves", "Vayne", 
                                            "Caitlyn","Urgot", "Ezreal", "KogMaw", "Ashe", "MissFortune", "Tristana", "Teemo", 
                                            "Sivir","Twitch", "Corki"};

        public static string[] tankySupport = { "Alistar", "Braum", "Leona", "Nunu", "Tahm Kench", "Taric", "Thresh" };

        public static string[] nontankySupport = { "Nami","Soraka","Janna","Sona","Lulu","Kayle","Bard","Karma","Lux","Morgana",
                                                 "Zilean","Zyra"};

        private static readonly float[] RRD = { 19, 29, 39, 49, 59 };
        private static readonly float[] RRDM = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] RRPS = { 10, 14, 19, 25, 32 };
        private static readonly float[] RRPSM = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.CharData.BaseSkinName != cName)
            {
                return;
            }

            SupportManager.drawSupport = true;

            Activators.QSS.hikiQSS = true;
            Activators.Ghostblade.hikiGhostBlade = true;
            Activators.BOTRK.hikiBOTRK = true;
            Activators.Bilgewater.hikiBilgewater = true;
            Activators.Potion.hikiPotion = true;

            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Config = new Menu("HikiCarry - Kalista", "HikiCarry - Kalista", true);
            CustomTargetSelector.AddToMenu(Config.SubMenu("Target Selector Settings"));

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
            
            //Orbwalker = new ModifiedOrbwalker.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("qCombo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("eCombo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("rCombo", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("rCount", "R on x Enemy").SetValue(new Slider(3, 0, 10)));
                Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("eHarass", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("eSpearCount", "If Enemy Spear Count >= ").SetValue(new Slider(3, 0, 10)));
                harassMenu.AddItem(new MenuItem("manaHarass", "Harass Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(harassMenu);
            }

            var laneMenu = new Menu("LaneClear Settings", "LaneClear Settings");
            {
                laneMenu.AddItem(new MenuItem("eClear", "Use E").SetValue(true));
                laneMenu.AddItem(new MenuItem("eClearCount", "If Can Kill Minion >= ").SetValue(new Slider(2, 1, 5)));
                laneMenu.AddItem(new MenuItem("manaClear", "Clear Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(laneMenu);
            }

            var lastMenu = new Menu("LastHit Settings", "LastHit Settings");
            {
                lastMenu.AddItem(new MenuItem("eLast", "Use E").SetValue(true));
                lastMenu.AddItem(new MenuItem("manaLast", "Clear Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(lastMenu);
            }

            var jungMenu = new Menu("JungleClear Settings", "JungleClear Settings");
            {
                jungMenu.AddItem(new MenuItem("qJungle", "Use Q").SetValue(true));
                jungMenu.AddItem(new MenuItem("eJungle", "Use E").SetValue(true));
                jungMenu.AddItem(new MenuItem("manaJungle", "Jungle Mana Manager").SetValue(new Slider(20, 0, 100)));
                Config.AddSubMenu(jungMenu);
            }

            var ksMenu = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                ksMenu.AddItem(new MenuItem("qKS", "Use Q").SetValue(true));
                ksMenu.AddItem(new MenuItem("eKS", "Use E").SetValue(true));
                Config.AddSubMenu(ksMenu);
            }

            var mobSteal = new Menu("Mob Steal Settings", "Mob Steal Settings");
            {
                var stealMobs = new Menu("Stealable Mobs", "Stealable Mobs");
                {
                    stealMobs.AddItem(new MenuItem("stealDragon", "Steal Dragon").SetValue(true));
                    stealMobs.AddItem(new MenuItem("stealBaron", "Steal Baron").SetValue(true));
                    stealMobs.AddItem(new MenuItem("stealRed", "Steal Red").SetValue(true));
                    stealMobs.AddItem(new MenuItem("stealBlue", "Steal Blue").SetValue(true));
                    mobSteal.AddSubMenu(stealMobs);
                }
                mobSteal.AddItem(new MenuItem("qSteal", "Use Q").SetValue(true));
                mobSteal.AddItem(new MenuItem("eSteal", "Use E").SetValue(true));
                Config.AddSubMenu(mobSteal);
            }

            var itemMenu = new Menu("Item Settings", "Item Settings");
            {
                var qssMenu = new Menu("QSS Settings", "QSS Settings");
                {
                    qssMenu.AddItem(new MenuItem("use.qss", "Use QSS").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.ignite", "Clear Ignite").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.exhaust", "Clear Exhaust").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.zedult", "Clear Zed R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.fizzult", "Clear Fizz R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.malzaharult", "Clear Malzahar R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.vladulti", "Clear Vladimir R").SetValue(true));
                    itemMenu.AddSubMenu(qssMenu);
                }

                var botrk = new Menu("BOTRK Settings", "BOTRK Settings");
                {
                    botrk.AddItem(new MenuItem("useBOTRK", "Use BOTRK").SetValue(true));
                    botrk.AddItem(new MenuItem("myhp", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    botrk.AddItem(new MenuItem("theirhp", "Use if enemy HP < %").SetValue(new Slider(20, 0, 100)));
                    itemMenu.AddSubMenu(botrk);
                }

                var ghostBlade = new Menu("GhostBlade Settings", "GhostBlade Settings");
                {
                    ghostBlade.AddItem(new MenuItem("gBlade", "Use GhostBlade").SetValue(true));
                    itemMenu.AddSubMenu(ghostBlade);
                }

                var bilgewater = new Menu("Bilgewater Settings", "Bilgewater Settings");
                {
                    bilgewater.AddItem(new MenuItem("useBilge", "Use BOTRK").SetValue(true));
                    bilgewater.AddItem(new MenuItem("myhpbilge", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    bilgewater.AddItem(new MenuItem("theirhpbilge", "Use if enemy HP < %").SetValue(new Slider(20, 0, 100)));
                    itemMenu.AddSubMenu(bilgewater);
                }
                var health = new Menu("Health Potion Settings", "Health Potion Settings");
                {
                    health.AddItem(new MenuItem("useHealth", "Use Health Potion").SetValue(true));
                    health.AddItem(new MenuItem("myhp", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    itemMenu.AddSubMenu(health);
                }
                var mana = new Menu("Mana Potion Settings", "Mana Potion Settings");
                {
                    mana.AddItem(new MenuItem("useMana", "Use Mana Potion").SetValue(true));
                    mana.AddItem(new MenuItem("mymana", "Use if my mana < %").SetValue(new Slider(20, 0, 100)));
                    itemMenu.AddSubMenu(mana);
                }

                Config.AddSubMenu(itemMenu);
            }

            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                var lastJoke = new Menu("Last Joke Settings", "Last Joke Settings");
                {
                    lastJoke.AddItem(new MenuItem("last.joke", "Last Joke").SetValue(true));
                    lastJoke.AddItem(new MenuItem("last.joke.hp", "Kalista HP Percent").SetValue(new Slider(2, 1, 99)));
                    miscMenu.AddSubMenu(lastJoke);
                }

                var orbSet = new Menu("Scrying Orb Settings", "Scrying Orb Settings");
                {
                    orbSet.AddItem(new MenuItem("bT", "Auto Scrying Orb Buy!").SetValue(true));
                    orbSet.AddItem(new MenuItem("bluetrinketlevel", "Scrying Orb Buy Level").SetValue(new Slider(6, 0, 18)));
                    miscMenu.AddSubMenu(orbSet);
                }
                miscMenu.AddItem(new MenuItem("qImmobile", "Auto Q to Immobile Target").SetValue(true));
                Config.AddSubMenu(miscMenu);
            }
            var wCombo = new Menu("Wombo Combo with R", "Wombo Combo with R"); // beta
            {
                var balista = new Menu("Balista", "Balista");
                {
                    balista.AddItem(new MenuItem("use.balista", "Balista Active").SetValue(true));
                    balista.AddItem(new MenuItem("balista.maxrange", "Balista Max Range").SetValue(new Slider(700, 100, 1500)));
                    balista.AddItem(new MenuItem("balista.minrange", "Balista Min Range").SetValue(new Slider(700, 100, 1500)));
                    wCombo.AddSubMenu(balista);
                }
                var skalista = new Menu("Skalista", "Skalista");
                {
                    skalista.AddItem(new MenuItem("use.skalista", "SKalista Active").SetValue(true));
                    skalista.AddItem(new MenuItem("skalista.maxrange", "SKalista Max Range").SetValue(new Slider(700, 100, 1500)));
                    skalista.AddItem(new MenuItem("skalista.minrange", "SKalista Min Range").SetValue(new Slider(700, 100, 1500)));
                    wCombo.AddSubMenu(skalista);
                }
            }
            Config.AddSubMenu(wCombo);

            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                drawMenu.AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.White)));
                drawMenu.AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.Silver)));
                drawMenu.AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.Yellow)));
                drawMenu.AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Gold)));
                drawMenu.AddItem(new MenuItem("ePercent", "E % On Enemy").SetValue(new Circle(true, Color.Gold)));
                drawMenu.AddItem(new MenuItem("e.percent.jungle.mobs", "E % On Jungle Mobs").SetValue(new Circle(true, Color.Chartreuse)));
                drawMenu.AddItem(new MenuItem("signal", "Support Signal").SetValue(true));
                drawMenu.AddItem(new MenuItem("circleSupport", "Draw Support on Circle").SetValue(true));
                Config.AddSubMenu(drawMenu);
            }

            Config.AddItem(new MenuItem("saveSupport", "Save Support [R]").SetValue(true));
            Config.AddItem(new MenuItem("savePercent", "Save Support Health Percent").SetValue(new Slider(10, 0, 100)));
            Config.AddItem(new MenuItem("calculator", "E Damage Calculator").SetValue(new StringList(new[] { "Custom Calculator", "Common Calculator" }))); //soontm

            var drawDamageMenu = new MenuItem("RushDrawEDamage", "E Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "E Damage Fill").SetValue(new Circle(true, Color.SeaGreen));

            drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

            DamageIndicator.DamageToUnit = GetTotalDamage;
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
        private static void Game_OnGameUpdate(EventArgs args)
        {
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
                JungleClear();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }
            if (Config.Item("use.balista").GetValue<bool>())
            {
                Balista();
            }
            if (Config.Item("use.skalista").GetValue<bool>())
            {
                Skalista();
            }
            stealJungle();
            KillSteal();
            immobileQ();
            Bitterlogic();

        }
        public static void immobileQ()
        {
            var useQ = Config.Item("qImmobile").GetValue<bool>();
            if (Q.IsReady() && useQ)
            {
                foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                {
                    if (immobileTarget(enemy) && Q.GetPrediction(enemy).Hitchance >= HitChance.High && Q.GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        Q.Cast(enemy);
                    }
                }
            }

        }
        public static bool immobileTarget(Obj_AI_Hero target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned)
            {
                return true;
            }
            else
                return false;
        }
        public static bool undyBuff(Obj_AI_Hero target)
        {
            if (target.ChampionName == "Tryndamere" &&
                target.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == "Undying Rage"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }
            if (target.ChampionName == "Poppy")
            {
                if (HeroManager.Allies.Any(o =>
                    !o.IsMe &&
                    o.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == "PoppyDITarget")))
                {
                    return true;
                }
            }
            return false;

        }
        private static void stealJungle()
        {
            var useQ = Config.Item("qSteal").GetValue<bool>();
            var useE = Config.Item("eSteal").GetValue<bool>();
            var stealDragon = Config.Item("stealDragon").GetValue<bool>();
            var stealBaron = Config.Item("stealBaron").GetValue<bool>();
            var stealRed = Config.Item("stealRed").GetValue<bool>();
            var stealBlue = Config.Item("stealBlue").GetValue<bool>();
            var mob = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Q.IsReady() && useQ)
            {
                if (stealDragon && mob[0].CharData.BaseSkinName.Contains("Dragon")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= Q.GetDamage(mob[0])) // Dragon Q
                {
                    Q.Cast(mob[0]);
                }

                if (stealBaron && mob[0].CharData.BaseSkinName.Contains("Baron")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= Q.GetDamage(mob[0])) // Baron Q
                {
                    Q.Cast(mob[0]);
                }

                if (stealBlue && mob[0].CharData.BaseSkinName.Contains("SRU_Blue")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= Q.GetDamage(mob[0])) // Blue Q
                {
                    Q.Cast(mob[0]);
                }

                if (stealRed && mob[0].CharData.BaseSkinName.Contains("SRU_Red")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= Q.GetDamage(mob[0])) // Red Q
                {
                    Q.Cast(mob[0]);
                }
            }
            if (E.IsReady() && useE)
            {
                if (stealDragon && mob[0].CharData.BaseSkinName.Contains("Dragon")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= E.GetDamage(mob[0])) // Dragon E
                {
                    E.Cast(mob[0]);
                }

                if (stealBaron && mob[0].CharData.BaseSkinName.Contains("Baron")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= E.GetDamage(mob[0])) // Baron E
                {
                    E.Cast();
                }

                if (stealBlue && mob[0].CharData.BaseSkinName.Contains("SRU_Blue")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= E.GetDamage(mob[0])) // Blue E
                {
                    E.Cast();
                }

                if (stealRed && mob[0].CharData.BaseSkinName.Contains("SRU_Red")
                    && mob[0].Health + 50 + (mob[0].HPRegenRate / 2) <= E.GetDamage(mob[0])) // Red E
                {
                    E.Cast();
                }
            }


        }
        private static void Combo()
        {
            var useQ = Config.Item("qCombo").GetValue<bool>();
            var useE = Config.Item("eCombo").GetValue<bool>();
            var useR = Config.Item("rCombo").GetValue<bool>();
            var supportPercent = Config.Item("savePercent").GetValue<Slider>().Value;
            var rCount = Config.Item("rCount").GetValue<Slider>().Value;
            var rAbleTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            float rEnemyCount = Player.CountEnemiesInRange(R.Range);

            if (Q.IsReady() && useQ)
            {
                foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                {
                    if (Q.GetPrediction(enemy).Hitchance >= HitChance.VeryHigh && 
                        Q.GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            if (E.IsReady() && useE)
            {
                foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range) &&
                    !undyBuff(hero) && !hero.HasBuffOfType(BuffType.SpellShield)))
                {
                    if (enemy.Health < GetTotalDamage(enemy))
                    {
                        E.Cast();
                    }
                }
            }
            if (R.IsReady() && useR)
            {
                foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValidTarget(R.Range) && ally.HasBuff("kalistacoopstrikeally")))
                {
                    if (ally.HealthPercent < supportPercent) // Save Support 
                    {
                        R.Cast();
                    }

                    if (rAbleTarget.Distance(Player.Position) > E.Range && rAbleTarget.Distance(Player.Position) < R.Range
                        && ally.HealthPercent > supportPercent && rEnemyCount <= 2 && nontankySupport.Contains(ally.ChampionName)) // Non-Tanky Champs
                    {
                        R.Cast();
                    }

                    if (rAbleTarget.Distance(Player.Position) > E.Range && rAbleTarget.Distance(Player.Position) < R.Range
                        && rEnemyCount <= 2 && Marksman.Contains(ally.ChampionName)
                        && ally.HealthPercent > supportPercent) // Adc Focus R
                    {
                        R.Cast();
                    }

                    if (rAbleTarget.Distance(Player.Position) > E.Range && rAbleTarget.Distance(Player.Position) < R.Range
                        && rEnemyCount >= rCount && tankySupport.Contains(ally.ChampionName) // Tanky vs Everyone
                        && ally.HealthPercent > supportPercent)
                    {
                        R.Cast();
                    }
                }
            }
        }
        private static void Bitterlogic()
        {
            foreach (var enemy in HeroManager.Enemies.Where(o=> o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
            {
                float spearDamage = GetTotalDamage(enemy);
                float killableSpearCount = enemy.Health/spearDamage;
                int totalSpear = (int)Math.Ceiling(killableSpearCount);
                if (ObjectManager.Player.Health < Config.Item("last.joke.hp").GetValue<Slider>().Value && KillableSpearCount(enemy) - 1 < totalSpear)
                {
                    E.Cast();
                }
            }
        }
        public static int KillableSpearCount(Obj_AI_Hero enemy)
        {
            float spearDamage = GetTotalDamage(enemy);
            float killableSpearCount = enemy.Health / spearDamage;
            int totalSpear = (int)Math.Ceiling(killableSpearCount) - 1;

            return totalSpear;
        }
        private static void Harass()
        {
            var manaSlider = Config.Item("manaHarass").GetValue<Slider>().Value;
            var eSpearCount = Config.Item("eSpearCount").GetValue<Slider>().Value;
            var useQ = Config.Item("qHarass").GetValue<bool>();
            var useE = Config.Item("eHarass").GetValue<bool>();

            if (Player.ManaPercent >= manaSlider)
            {
                if (Q.IsReady() && useQ)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Q.Range)))
                    {
                        if (Q.GetPrediction(enemy).Hitchance >= HitChance.VeryHigh && Q.GetPrediction(enemy).CollisionObjects.Count == 0)
                        {
                            Q.Cast(enemy);
                        }
                    }
                }
                if (E.IsReady() && useE)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range)))
                    {
                        int enemyStack = enemy.Buffs.FirstOrDefault(x => x.DisplayName == "KalistaExpungeMarker").Count;
                        if (enemyStack > eSpearCount)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }
        private static void Clear()
        {
            var manaSlider = Config.Item("manaHarass").GetValue<Slider>().Value;
            var eClearCount = Config.Item("eClearCount").GetValue<Slider>().Value;
            var useE = Config.Item("eClear").GetValue<bool>();

            if (Player.ManaPercent >= manaSlider)
            {
                if (E.IsReady() && useE)
                {
                    var mns = MinionManager.GetMinions(Player.ServerPosition, E.Range);
                    var mkc = mns.Count(x => E.CanCast(x) && x.Health <= E.GetDamage(x));
                    if (mkc >= eClearCount)
                    {
                        E.Cast();
                    }
                }
            }
        }
        private static void KillSteal()
        {
            var qKS = Config.Item("qKS").GetValue<bool>();
            var eKS = Config.Item("qKS").GetValue<bool>();
            if (Q.IsReady() && qKS)
            {
                foreach (var target in HeroManager.Enemies.OrderByDescending(x => x.Health))
                {
                    if (target.Distance(Player.Position) < Q.Range && Q.GetPrediction(target).Hitchance >= HitChance.VeryHigh
                        && Q.GetDamage(target) < target.Health)
                    {
                        Q.Cast(target);
                    }
                }
            }
            if (E.IsReady() && eKS)
            {
                foreach (var target in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range)))
                {
                    if (GetTotalDamage(target) > target.Health)
                    {
                        E.Cast();
                    }    
                }
            }
        }
        private static void JungleClear()
        {
            var useQ = Config.Item("qJungle").GetValue<bool>();
            var useE = Config.Item("eJungle").GetValue<bool>();
            var manaSlider = Config.Item("manaJungle").GetValue<Slider>().Value;
            var mob = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mob == null || (mob != null && mob.Count == 0))
            {
                return;
            }

            if (Player.ManaPercent > manaSlider)
            {
                if (Q.IsReady() && useQ)
                {
                    Q.Cast(mob[0]);
                }
                if (E.IsReady() && useE)
                {
                    if (mob[0].Health + (mob[0].HPRegenRate / 2) <= E.GetDamage(mob[0]))
                    {
                        E.Cast();
                    }
                }
            }
        }
        private static void Balista()
        {
            if (!R.IsReady())
            {
                return;
            }
            var blitz = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsAlly && x.CharData.BaseSkinName == "Blitzcrank" && x.HasBuff("kalistacoopstrikeally"));
            var balistamaxrange = Config.Item("balista.maxrange").GetValue<Slider>().Value;
            var balistaminrange = Config.Item("balista.minrange").GetValue<Slider>().Value;

            if (blitz != null && R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsEnemy && o.IsValidTarget(2450f)))
                {
                    if (SupportManager.Support.Distance(enemy.Position) <= 950f &&
                        SupportManager.Support.Distance(Player.Position) >= balistaminrange &&
                        SupportManager.Support.Distance(Player.Position) <= balistamaxrange)
                    {
                        if (enemy.Buffs != null && enemy.HasBuff("rocketgrab2"))
                        {
                            if (R.IsReady())
                            {
                                R.Cast();
                            }
                        }
                    }
                }
            }
        }
        private static void Skalista() 
        {
            if (!R.IsReady())
            {
                return;
            }
            var skarner = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.IsAlly && o.CharData.BaseSkinName == "Skarner" &&
                o.HasBuff("kalistacoopstrikeally"));
            var skarnermaxrange = Config.Item("balista.maxrange").GetValue<Slider>().Value;
            var skarnerminrange = Config.Item("balista.maxrange").GetValue<Slider>().Value;

            if (skarner != null && R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsEnemy && o.IsValidTarget(1849))) // Kalista R Range + Skarner R Range - 1
                {
                    if (SupportManager.Support.Distance(enemy.Position) <= 350 &&
                        SupportManager.Support.Distance(Player.Position) >= skarnerminrange &&
                        SupportManager.Support.Distance(Player.Position) <= skarnermaxrange)
                    {
                        if (enemy.Buffs != null && enemy.HasBuff("SkarnerImpale"))
                        {
                            if (R.IsReady())
                            {
                                R.Cast();
                            }
                        }
                    }
                }
            }

        }
        private static void LastHit()
        {
            var useE = Config.Item("eLast").GetValue<bool>();
            var manaSlider = Config.Item("manaLast").GetValue<Slider>().Value;
            var qMinion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);

            if (Player.ManaPercent >= manaSlider)
            {
                if (E.IsReady() && useE)
                {
                    foreach (var minyon in qMinion)
                    {
                        if (E.IsKillable(minyon))
                        {
                            E.CastOnUnit(minyon);
                        }
                    }
                }
            }

        }
        public static float CustomCalculator(Obj_AI_Base target, int customStacks = -1)
        {
            int buff = target.GetBuffCount("KalistaExpungeMarker");

            if (buff > 0 || customStacks > -1)
            {
                var tDamage = (RRD[E.Level - 1] + RRDM[E.Level - 1] * Player.TotalAttackDamage) +
                       ((customStacks < 0 ? buff : customStacks) - 1) *
                       (RRPS[E.Level - 1] + RRPSM[E.Level - 1] * Player.TotalAttackDamage);

                return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, tDamage);
            }

            return 0;
        }
        public static float JungleCalculator(Obj_AI_Minion minion, int customStacks = -1)
        {
            int buff = minion.GetBuffCount("KalistaExpungeMarker");

            if (buff > 0 || customStacks > -1)
            {
                var tDamage = (RRD[E.Level - 1] + RRDM[E.Level - 1] * Player.TotalAttackDamage) +
                       ((customStacks < 0 ? buff : customStacks) - 1) *
                       (RRPS[E.Level - 1] + RRPSM[E.Level - 1] * Player.TotalAttackDamage);

                return (float)ObjectManager.Player.CalcDamage(minion, Damage.DamageType.Physical, tDamage);
            }

            return 0;
        }
        public static float GetTotalDamage(Obj_AI_Hero target)
        {
            var damage = 0f;

            if (E.IsReady())
            {
                switch (Config.Item("calculator").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        damage += CustomCalculator(target);
                        break;
                    case 1:
                        damage += (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, E.GetDamage(target));
                        break;
                }

            }
            return (float)damage;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = Config.Item("eDraw").GetValue<Circle>();
            var menuItem4 = Config.Item("rDraw").GetValue<Circle>();
            var menuItem5 = Config.Item("ePercent").GetValue<Circle>();
            var menuItem6 = Config.Item("e.percent.jungle.mobs").GetValue<Circle>();

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X,Player.Position.Y,Player.Position.Z), Q.Range, menuItem1.Color,5);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), W.Range, menuItem2.Color,5);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), E.Range, menuItem3.Color,5);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), R.Range, menuItem4.Color,5);
            }
            if (menuItem5.Active && E.IsReady())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(o=> o.IsHPBarRendered && o.IsEnemy && o.IsValidTarget(1000)))
                {
                    float getTotalDamage = GetTotalDamage(enemy);
                    float tDamage = getTotalDamage*100/enemy.Health;
                    int totalDamage = (int)Math.Ceiling(tDamage);

                    if (totalDamage > 0)
                    {
                        Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, Color.Yellow, string.Format("{0}%", totalDamage));
                    }
                }
            }
            if (menuItem6.Active && E.IsReady())
            {
                foreach (var jungleMobs in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(E.Range) && o.Team == GameObjectTeam.Neutral && o.IsVisible && !o.IsDead))
                {
                    var damage = 0f;
                    switch (Config.Item("calculator").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            damage += JungleCalculator(jungleMobs);
                            break;
                        case 1:
                            damage += (float)ObjectManager.Player.CalcDamage(jungleMobs, Damage.DamageType.Physical, E.GetDamage(jungleMobs));
                            break;
                    }

                    float tDamage = damage * 100 / jungleMobs.Health;
                    int totalDamage = (int)Math.Ceiling(tDamage);
                    if (totalDamage >= 0)
                    {
                        switch (jungleMobs.CharData.BaseSkinName)
                        {
                            case "SRU_Razorbeak":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X+50, jungleMobs.HPBarPosition.Y, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "SRU_Red":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y-3, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "SRU_Blue":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "SRU_Dragon":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "SRU_Baron":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "SRU_Gromp":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "SRU_Krug":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X+53, jungleMobs.HPBarPosition.Y, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "SRU_Murkwolf":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X+50, jungleMobs.HPBarPosition.Y, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                            case "Sru_Crab":
                                Drawing.DrawText(jungleMobs.HPBarPosition.X+50, jungleMobs.HPBarPosition.Y+20, menuItem6.Color,
                                    string.Format("{0}%", totalDamage));
                                break;
                        }
                    }
                }
            }
        }
    }
}