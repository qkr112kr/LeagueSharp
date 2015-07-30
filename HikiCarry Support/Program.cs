using System;
using LeagueSharp.Common;

namespace HikiCarry_Support
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            SSeries.Load();
        }
    }
}