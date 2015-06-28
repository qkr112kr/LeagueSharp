using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace HikiCarry_Support
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameStart;
        }

        private static void Game_OnGameStart(EventArgs args)
        {
            string champz = LeagueSharp.ObjectManager.Player.ChampionName.ToLower();
            switch (champz)
            {
                case "Leona":
                    Champions.Leona.AIOLeona(args);
                    break;
                
                default:
                    return;
            }
        }
    }
}
