using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using SharpDX;

namespace HikiCarry_Caitlyn
{
    class Helper
    {
        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[Program.Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }
        public static bool Immobile(Obj_AI_Hero target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void BlueOrb(int level)
        {
            if (ObjectManager.Player.Level >= level && ObjectManager.Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                ObjectManager.Player.BuyItem(ItemId.Scrying_Orb_Trinket);
            }
        }
        public static bool MenuCheck(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<bool>();
        }
        public static void SkillDraw(float range, Color color, int width)
        {
            Render.Circle.DrawCircle(new Vector3(ObjectManager.Player.Position.X, ObjectManager.Player.Position.Y, ObjectManager.Player.Position.Z), range, color, width);
        }
        public static int CountCheckerino(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<Slider>().Value;
        }
        public static Color SkillColor(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<Circle>().Color;
        }
        public static bool SkillDrawActive(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<Circle>().Active;
        }
        
    }
}