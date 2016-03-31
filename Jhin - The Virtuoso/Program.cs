using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += JhinOnGameLoad;
        }

        private static void JhinOnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Jhin")
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Jhin();
            }
            else
            {
                Console.WriteLine("sorry :roto2:");
            }
        }
    }
}
