using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


namespace adcUtility.Activator
{
    public static class Barrier
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiBarrier { get; set; }

        public static Obj_AI_Base adcBarrier
        {
            get
            {
                if (adCarry != null && adCarry.IsValid)
                {
                    return adCarry;
                }
                return null;
            }
        }
        static Barrier()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var useHeal = Program.Config.Item("use.barrier").GetValue<bool>();
            var healMyhp = Program.Config.Item("barrier.myhp").GetValue<Slider>().Value;

            if (useHeal && Program.Heal.IsReady())
            {
                if (ObjectManager.Player.HealthPercent < healMyhp)
                {
                    ObjectManager.Player.Spellbook.CastSpell(Program.Barrier);
                }
            }
        }



    }
}