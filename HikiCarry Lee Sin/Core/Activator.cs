using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Champions;
using HybridCommon;
using LeagueSharp;
using LeagueSharp.Common;

namespace HikiCarry_Lee_Sin.Core
{
    public class Activator : LeeSin
    {
        public static Items.Item Tiamat = new Items.Item(3077, 185);
        public static Items.Item Hydra = new Items.Item(3074, 185);
        public static Items.Item Titanic = new Items.Item(3748, 185);
        public static Items.Item Botrk = new Items.Item(3153, 550);
        public static Items.Item Bilgewater = new Items.Item(3153, 550);
        public static Items.Item Youmuu = new Items.Item(3142);
        public static Items.Item Randuin = new Items.Item(3143, 500);
        public static readonly Obj_AI_Hero LeeSin = ObjectManager.Player;

        public static void HikiTiamat(Obj_AI_Hero target)
        {

            if (!Tiamat.IsOwned() && !Tiamat.IsReady() && LeeSin.Distance(target.Position) > Tiamat.Range 
                && !Config.Item("use.hydra").GetValue<bool>())
            {
                return;
            }
            if (Tiamat.IsReady() && Tiamat.IsOwned() && LeeSin.Distance(target.Position) < Tiamat.Range &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.IsValid && Config.Item("use.tiamat").GetValue<bool>())
            {
                Tiamat.Cast();
            }
        }

        public static void HikiHydra(Obj_AI_Hero target)
        {
            if (!Hydra.IsOwned() && !Hydra.IsReady() && LeeSin.Distance(target.Position) > Hydra.Range && !Config.Item("use.hydra").GetValue<bool>())
            {
                return;
            }
            if (Hydra.IsReady() && Hydra.IsOwned() && LeeSin.Distance(target.Position) < Hydra.Range &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.IsValid && Config.Item("use.hydra").GetValue<bool>())
            {
                Hydra.Cast();
            }
            if (Hydra.IsReady() && Hydra.IsOwned() && LeeSin.Distance(target.Position) < Hydra.Range &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && target.IsValid && Config.Item("hydra.clear").GetValue<bool>())
            {
                Hydra.Cast();
            }
        }

        public static void HikiTitanic(Obj_AI_Hero target)
        {
            if (!Titanic.IsOwned() && !Titanic.IsReady() && LeeSin.Distance(target.Position) > Titanic.Range && !Config.Item("use.titanic").GetValue<bool>())
            {
                return;
            }
            if (Titanic.IsReady() && Titanic.IsOwned() && LeeSin.Distance(target.Position) < Titanic.Range &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.IsValid && Config.Item("use.titanic").GetValue<bool>())
            {
                Titanic.Cast();
            }
        }

        public static void HikiBlade(Obj_AI_Hero target)
        {
            if (!Botrk.IsOwned() && !Botrk.IsReady() && LeeSin.Distance(target.Position) > Botrk.Range &&
                LeeSin.HealthPercent > Helper.SliderCheck("botrk.hp") || target.HealthPercent > Helper.SliderCheck("botrk.enemy.hp"))
            {
                return;
            }
            if (Botrk.IsReady() && Botrk.IsOwned() && LeeSin.Distance(target.Position) < Botrk.Range  &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.IsValid && Config.Item("use.botrk").GetValue<bool>()
                && LeeSin.HealthPercent < Helper.SliderCheck("botrk.hp") || target.HealthPercent < Helper.SliderCheck("botrk.enemy.hp"))
            {
                Botrk.Cast(target);
            }
        }

        public static void HikiBilgewater(Obj_AI_Hero target)
        {
            if (!Bilgewater.IsOwned() && !Bilgewater.IsReady() && LeeSin.Distance(target.Position) > Bilgewater.Range &&
                LeeSin.HealthPercent > Helper.SliderCheck("bilgewater.hp") || target.HealthPercent > Helper.SliderCheck("bilgewater.enemy.hp")
                && !Config.Item("use.bilgewater").GetValue<bool>())
            {
                return;
            }
            if (Bilgewater.IsReady() && Bilgewater.IsOwned() && LeeSin.Distance(target.Position) < Bilgewater.Range &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.IsValid && Config.Item("use.bilgewater").GetValue<bool>()
                && LeeSin.HealthPercent < Helper.SliderCheck("bilgewater.hp") || target.HealthPercent < Helper.SliderCheck("bilgewater.enemy.hp"))
            {
                Bilgewater.Cast(target);
            }
        }

        public static void HikiYoumuu(Obj_AI_Hero target)
        {
            if (!Youmuu.IsOwned() && !Youmuu.IsReady() && 
                LeeSin.Distance(target.Position) > 500 && !Config.Item("use.youmuu").GetValue<bool>())
            {
                return;
            }
            if (Youmuu.IsReady() && Youmuu.IsOwned() && LeeSin.Distance(target.Position) < 500 &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.IsValid && Config.Item("use.youmuu").GetValue<bool>())
            {
                Youmuu.Cast();
            }
        }

        public static void HikiRanduin(Obj_AI_Hero target)
        {
            if (!Randuin.IsOwned() && !Randuin.IsReady() &&
                LeeSin.Distance(target.Position) > Randuin.Range && !Config.Item("use.randuin").GetValue<bool>() 
                && LeeSin.CountAlliesInRange(Randuin.Range) < Helper.SliderCheck("randuin.min.enemy.count"))
            {
                return;
            }
            if (Randuin.IsReady() && Randuin.IsOwned() && LeeSin.Distance(target.Position) < Randuin.Range &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && target.IsValid && Config.Item("use.randuin").GetValue<bool>()
                && LeeSin.CountAlliesInRange(Randuin.Range) >= Helper.SliderCheck("randuin.min.enemy.count"))
            {
                Randuin.Cast();
            }
        }

    }
}
