using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using LeagueSharp;
using LeagueSharp.Common;
using HybridCommon;
using SharpDX;
using SharpDX.Direct3D9;

namespace HybridCommon
{
    public abstract class BaseChamp
    {
        
        public const int Q = 0, Q2 = 1, W = 2, E = 3, E2 = 4, R = 5, W2 = 6;
        
        public static int WardRange;
        public static Menu Config,ComboMenu,HikiMenu,InsecMenu,WhiteMenu,HarassMenu,ClearMenu,JungleMenu,KillStealMenu,DrawMenu,Evade,SMenu,StealMenu;
        public Orbwalking.Orbwalker Orbwalker;
        public static Spell[] Spells = new Spell[7];
        public static long LastCheck;
        public static List<Vector2> Points = new List<Vector2>();
        private static readonly double[] Q2Dmg = { 70, 120, 170, 220, 270, 320 };
        public Evader m_evader;
        public Font Text;

        public delegate void DVoidDelegate();
        public DVoidDelegate BeforeOrbWalking, BeforeDrawing;
        public DVoidDelegate[] OrbwalkingFunctions = new DVoidDelegate[4];

        public BaseChamp(string szChampName)
        {
            Text = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Malgun Gothic",
                    Height = 15,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural
                });

            Config = new Menu(String.Format("Hikigaya - Lee Sin!", szChampName), szChampName, true);
            
            TargetSelector.AddToMenu(Config.SubMenu("Target Selector Settings"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking Settings"));
            
            SpellDatabase.InitalizeSpellDatabase();
        }

       
        public virtual void CreateConfigMenu()
        {
            //
        }

        public virtual void SetSpells()
        {
            //
        }

        public virtual void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || args == null)
                return;

            if (BeforeOrbWalking != null) BeforeOrbWalking();

            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && OrbwalkingFunctions[(int)Orbwalker.ActiveMode] != null)
                OrbwalkingFunctions[(int)Orbwalker.ActiveMode]();
        }

        public virtual void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            //
        }


        public virtual void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //
        }

        public virtual void Interrupter_OnPossibleToInterrupt(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            //
        }

        public virtual void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            //
        }

        public virtual void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //
        }

        public void CastSkillshot(Obj_AI_Hero t, Spell s, HitChance hc = HitChance.High)
        {
            
        }

        public bool ComboReady()
        {
            return Spells[Q].IsReady() && Spells[W].IsReady() && Spells[E].IsReady() && Spells[R].IsReady();
        }

        #region Damage Calculation Funcitons
        public double CalculateComboDamage(Obj_AI_Hero target, int aacount = 0)
        {
            return CalculateSpellDamage(target) + CalculateSummonersDamage(target) + CalculateItemsDamage(target) + CalculateAADamage(target, aacount);
        }


        public virtual double CalculateDamageQ(Obj_AI_Hero target)
        {
            if (Spells[Q].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);

            return 0.0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageW(Obj_AI_Hero target)
        {
            if (Spells[W].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageQ2(Obj_AI_Hero target)
        {
            if (Spells[Q].IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" &&
                target.HasBuff("BlindMonkQOne"))
            {
                var total = Q2Dmg[Spells[Q].Level - 1] + 0.9 * ObjectManager.Player.FlatPhysicalDamageMod +
                            0.08 * (target.MaxHealth - target.Health);
                return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, total);
            }
            return 0.0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageE(Obj_AI_Hero target)
        {
            if (Spells[E].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageR(Obj_AI_Hero target)
        {
            if (Spells[R].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateSpellDamage(Obj_AI_Hero target)
        {
            return CalculateDamageQ(target) + CalculateDamageW(target) + CalculateDamageE(target) + CalculateDamageR(target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double CalculateSummonersDamage(Obj_AI_Hero target)
        {
            var ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
            if (ignite != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(ignite) == SpellState.Ready && ObjectManager.Player.Distance(target, false) < 550)
                return ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite); //ignite

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateItemsDamage(Obj_AI_Hero target)
        {
            double dmg = 0.0;

            if (Items.CanUseItem(3144) && ObjectManager.Player.Distance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //bilgewater cutlass

            if (Items.CanUseItem(3153) && ObjectManager.Player.Distance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk); //botrk

            if(Items.HasItem(3057))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, ObjectManager.Player.BaseAttackDamage); //sheen

            if (Items.HasItem(3100))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, (0.75 * ObjectManager.Player.BaseAttackDamage) + (0.50 * ObjectManager.Player.FlatMagicDamageMod)); //lich bane
            
            if(Items.HasItem(3285))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, 100 + (0.1 * ObjectManager.Player.FlatMagicDamageMod)); //luden

            return dmg;
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateAADamage(Obj_AI_Hero target, int aacount = 2)
        {
            double dmg = ObjectManager.Player.GetAutoAttackDamage(target) * aacount;

            if (Items.HasItem(3115))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, 15 + (0.15 * ObjectManager.Player.FlatMagicDamageMod)); //nashor

            return dmg;
        }
        #endregion
    }
}
