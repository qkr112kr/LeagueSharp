using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using doubleH_Support.Utils;
using Color = System.Drawing.Color;


namespace doubleH_Support
{
    internal static class hLoader
    {
        internal static Menu Config;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Obj_AI_Hero Player = ObjectManager.Player;

        internal static void Load()
        {
            try
            {
                //Print the welcome message
                Game.PrintChat("doubleH Support Series Loaded");

                // Load the menu.
                Config = new Menu("doubleH Support", "doubleH Support", true);

                // Add the target selector.
                TargetSelector.AddToMenu(Config.SubMenu("Target Selector Settings"));

                // Add the orbwalking.
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                // Check if the champion is supported
                var type = Type.GetType("doubleH_Support.Champions." + Player.ChampionName);
                if (type != null)
                {
                    DynamicInitializer.NewInstance(type);
                }

                Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));
                Config.SubMenu("Draw Settings").AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.Yellow)));
                Config.SubMenu("Draw Settings").AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.Green)));
                Config.SubMenu("Draw Settings").AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.White)));
                Config.SubMenu("Draw Settings").AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Brown)));
                Config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}