using System;
using LeagueSharp.Common;

namespace doubleH_Support
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            hLoader.Load();
        }
    }
}