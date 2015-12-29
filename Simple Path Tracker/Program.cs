using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;


namespace Simple_Path_Tracker
{
    class Program
    {
        public static Menu Config;
        public static Font HikiFont;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            HikiFont = new Font(Drawing.Direct3DDevice, new FontDescription
            {
                FaceName = "Calibri",
                Height = 20,
                Weight = FontWeight.Bold,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.ClearTypeNatural,    
            });

            Config = new Menu("Simple Path Tracker", "Simple Path Tracker", true);
            {
                Config.AddItem(new MenuItem("ally.path", "Ally Path ?").SetValue(false));
                Config.AddItem(new MenuItem("enemy.path", "Enemy Path ?").SetValue(true));
                Config.AddItem(new MenuItem("my.path", "My Path ?").SetValue(false));
                Config.AddItem(new MenuItem("path.search.distance", "Path Search Distance").SetValue(new Slider(666, 1, 3000)));
                Config.AddItem(new MenuItem("eta", "Eta ?").SetValue(true));
                Config.AddToMainMenu();
            }
            Drawing.OnDraw += OnDraw;
        }

        
        public static Vector3 WayPoint(Obj_AI_Hero hero)
        {
            return hero.GetWaypoints()[hero.GetWaypoints().Count - 1].To3D();
        }

        public static float Eta(Obj_AI_Hero hero)
        {
            var x1 = hero.Distance(WayPoint(hero));
            var x2 = x1 / hero.MoveSpeed;
            return x2;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("ally.path").GetValue<bool>())
            {
                foreach (var ally in HeroManager.Allies.Where(x => !x.IsMe && ObjectManager.Player.Distance(x.Position) < Config.Item("path.search.distance").GetValue<Slider>().Value))
                {
                    if (Config.Item("eta").GetValue<bool>())
                    {
                        HikiFont.DrawText(null, "" + Eta(ally), (int)Drawing.WorldToScreen(WayPoint(ally)).X+20, (int)Drawing.WorldToScreen(WayPoint(ally)).Y+20,
                            SharpDX.Color.Gold);
                    }
                    Drawing.DrawLine(Drawing.WorldToScreen(ally.Position), Drawing.WorldToScreen(WayPoint(ally)),2,Color.Gold);
                }
            }
            if (Config.Item("enemy.path").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Config.Item("path.search.distance").GetValue<Slider>().Value)))
                {
                    if (Config.Item("eta").GetValue<bool>())
                    {
                        HikiFont.DrawText(null, "" + Eta(enemy), (int)Drawing.WorldToScreen(WayPoint(enemy)).X + 20, (int)Drawing.WorldToScreen(WayPoint(enemy)).Y + 20,
                            SharpDX.Color.White);
                    }
                    Drawing.DrawLine(Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(WayPoint(enemy)), 2, Color.White);
                }
            }
            if (Config.Item("my.path").GetValue<bool>())
            {
                if (Config.Item("eta").GetValue<bool>())
                {
                    HikiFont.DrawText(null, "" + Eta(ObjectManager.Player), (int)Drawing.WorldToScreen(WayPoint(ObjectManager.Player)).X+20, (int)Drawing.WorldToScreen(WayPoint(ObjectManager.Player)).Y+20,
                        SharpDX.Color.White);
                }
                Drawing.DrawLine(Drawing.WorldToScreen(ObjectManager.Player.Position), Drawing.WorldToScreen(WayPoint(ObjectManager.Player)), 2, Color.White);   
            }
        }
    }
}
