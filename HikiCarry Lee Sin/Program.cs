using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HikiCarry_Lee_Sin.Champions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using HybridCommon;
//typedefs
using Color = System.Drawing.Color;

namespace HikiCarry_Lee_Sin
{
    class Program
    {
        public static BaseChamp Champion;
        static void Main(string[] args)
        {
            if (Game.Mode == GameMode.Running)
            {
                Game_OnGameLoad(null);
            }

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            switch (ObjectManager.Player.ChampionName.ToLowerInvariant())
            {
                case "leesin":
                    Champion = new LeeSin();
                    break;

            }

            Champion.CreateConfigMenu();
            Champion.SetSpells();
            #region Events
            Game.OnUpdate += Champion.Game_OnUpdate;
            Orbwalking.BeforeAttack += Champion.Orbwalking_BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += Champion.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Champion.Interrupter_OnPossibleToInterrupt;
            Obj_AI_Base.OnBuffAdd += Champion.Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnProcessSpellCast += Champion.Obj_AI_Base_OnProcessSpellCast;
            #endregion
            Notifications.AddNotification(String.Format("Hikigaya {0} Loaded !", ObjectManager.Player.ChampionName), 3000);
        }
    }
}
