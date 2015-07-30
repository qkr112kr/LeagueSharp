using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using HikiCarry_Support.Utils;
using Color = System.Drawing.Color;


namespace HikiCarry_Support
{
    internal static class SSeries
    {
        internal static Menu Config;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Obj_AI_Hero Player = ObjectManager.Player;

        internal static void Load()
        {
            try
            {
                //Print the welcome message
                Game.PrintChat("Hiki Support Series Loaded!");

                // Load the menu.
                Config = new Menu("HikiCarry Support", "HikiCarry Support", true);

                // Add the target selector.
                TargetSelector.AddToMenu(Config.SubMenu("Target Selector Settings"));

                // Add the orbwalking.
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                // Check if the champion is supported
                var type = Type.GetType("HikiCarry_Support.Champions." + Player.ChampionName);
                if (type != null)
                {
                    DynamicInitializer.NewInstance(type);
                }

                Config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}