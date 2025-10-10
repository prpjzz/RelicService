using RelicService.Data.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Service
{
    internal class Utils
    {
        public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            // Calculate the new dimensions while maintaining aspect ratio
            float ratioX = (float)maxWidth / image.Width;
            float ratioY = (float)maxHeight / image.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio * Program.DpiScaleFactor);
            int newHeight = (int)(image.Height * ratio * Program.DpiScaleFactor);

            // Create a new bitmap with the new dimensions
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);

            // Draw the original image onto the new bitmap
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        public static string FormatFightProp(FightPropType type, float value)
        {
            float percentValue = (float)Math.Round(value * 100, 1, MidpointRounding.AwayFromZero);
            value = (float)Math.Round(value, 0, MidpointRounding.AwayFromZero);

            return type switch
            {
                FightPropType.FIGHT_PROP_HP => $"生命值+{(int)value}",
                FightPropType.FIGHT_PROP_HP_PERCENT => $"生命值+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ATTACK => $"攻击力+{(int)value}",
                FightPropType.FIGHT_PROP_ATTACK_PERCENT => $"攻击力+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_DEFENSE => $"防御力+{(int)value}",
                FightPropType.FIGHT_PROP_DEFENSE_PERCENT => $"防御力+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_CRITICAL => $"暴击率+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_CRITICAL_HURT => $"暴击伤害+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_CHARGE_EFFICIENCY => $"元素充能效率+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_HEAL_ADD => $"治疗加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ELEMENT_MASTERY => $"元素精通+{(int)value}",
                FightPropType.FIGHT_PROP_PHYSICAL_ADD_HURT => $"物理伤害加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_FIRE_ADD_HURT => $"火元素伤害加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ELEC_ADD_HURT => $"雷元素伤害加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_WATER_ADD_HURT => $"水元素伤害加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_GRASS_ADD_HURT => $"草元素伤害加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_WIND_ADD_HURT => $"风元素伤害加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ROCK_ADD_HURT => $"岩元素伤害加成+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ICE_ADD_HURT => $"冰元素伤害加成+{percentValue:F1}%",
                _ => $"{value}",
            };

        }

        public static string FormatFightPropShort(FightPropType type, float value)
        {
            float percentValue = (float)Math.Round(value * 100, 1, MidpointRounding.AwayFromZero);
            value = (float)Math.Round(value, 0, MidpointRounding.AwayFromZero);

            return type switch
            {
                FightPropType.FIGHT_PROP_HP => $"生命+{(int)value}",
                FightPropType.FIGHT_PROP_HP_PERCENT => $"生命+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ATTACK => $"攻击+{(int)value}",
                FightPropType.FIGHT_PROP_ATTACK_PERCENT => $"攻击+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_DEFENSE => $"防御+{(int)value}",
                FightPropType.FIGHT_PROP_DEFENSE_PERCENT => $"防御+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_CRITICAL => $"暴击+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_CRITICAL_HURT => $"暴伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_CHARGE_EFFICIENCY => $"充能+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_HEAL_ADD => $"治疗+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ELEMENT_MASTERY => $"精通+{(int)value}",
                FightPropType.FIGHT_PROP_PHYSICAL_ADD_HURT => $"物伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_FIRE_ADD_HURT => $"火伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ELEC_ADD_HURT => $"雷伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_WATER_ADD_HURT => $"水伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_GRASS_ADD_HURT => $"草伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_WIND_ADD_HURT => $"风伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ROCK_ADD_HURT => $"岩伤+{percentValue:F1}%",
                FightPropType.FIGHT_PROP_ICE_ADD_HURT => $"冰伤+{percentValue:F1}%",
                _ => $"{value}",
            };
        }

    }
}
