using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using RelicService.Data.Config;

namespace RelicService.Service;

internal class Utils
{
	public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
	{
		float val = (float)maxWidth / (float)image.Width;
		float val2 = (float)maxHeight / (float)image.Height;
		float num = Math.Min(val, val2);
		int width = (int)((float)image.Width * num * Program.DpiScaleFactor);
		int height = (int)((float)image.Height * num * Program.DpiScaleFactor);
		Bitmap bitmap = new Bitmap(width, height);
		using Graphics graphics = Graphics.FromImage(bitmap);
		graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		graphics.DrawImage(image, 0, 0, width, height);
		return bitmap;
	}

	public static string FormatFightProp(FightPropType type, float value)
	{
		float value2 = (float)Math.Round(value * 100f, 1, MidpointRounding.AwayFromZero);
		value = (float)Math.Round(value, 0, MidpointRounding.AwayFromZero);
		return type switch
		{
			FightPropType.FIGHT_PROP_HP => $"HP+{(int)value}", 
			FightPropType.FIGHT_PROP_HP_PERCENT => $"HP+{value2:F1}%", 
			FightPropType.FIGHT_PROP_ATTACK => $"ATK+{(int)value}", 
			FightPropType.FIGHT_PROP_ATTACK_PERCENT => $"ATK+{value2:F1}%", 
			FightPropType.FIGHT_PROP_DEFENSE => $"DEF+{(int)value}", 
			FightPropType.FIGHT_PROP_DEFENSE_PERCENT => $"DEF+{value2:F1}%", 
			FightPropType.FIGHT_PROP_CRITICAL => $"Crit Rate+{value2:F1}%", 
			FightPropType.FIGHT_PROP_CRITICAL_HURT => $"Crit DMG+{value2:F1}%", 
			FightPropType.FIGHT_PROP_CHARGE_EFFICIENCY => $"Energy Recharge+{value2:F1}%", 
			FightPropType.FIGHT_PROP_HEAL_ADD => $"Healing Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_ELEMENT_MASTERY => $"Elemental Mastery+{(int)value}", 
			FightPropType.FIGHT_PROP_PHYSICAL_ADD_HURT => $"Physical DMG Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_FIRE_ADD_HURT => $"Pyro DMG Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_ELEC_ADD_HURT => $"Electro DMG Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_WATER_ADD_HURT => $"Hydro DMG Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_GRASS_ADD_HURT => $"Dendro DMG Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_WIND_ADD_HURT => $"Anemo DMG Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_ROCK_ADD_HURT => $"Geo DMG Bonus+{value2:F1}%", 
			FightPropType.FIGHT_PROP_ICE_ADD_HURT => $"Cyro DMG Bonus+{value2:F1}%", 
			_ => $"{value}", 
		};
	}

	public static string FormatFightPropShort(FightPropType type, float value)
	{
		float value2 = (float)Math.Round(value * 100f, 1, MidpointRounding.AwayFromZero);
		value = (float)Math.Round(value, 0, MidpointRounding.AwayFromZero);
		return type switch
		{
            FightPropType.FIGHT_PROP_HP => $"HP+{(int)value}",
            FightPropType.FIGHT_PROP_HP_PERCENT => $"HP+{value2:F1}%",
            FightPropType.FIGHT_PROP_ATTACK => $"ATK+{(int)value}",
            FightPropType.FIGHT_PROP_ATTACK_PERCENT => $"ATK+{value2:F1}%",
            FightPropType.FIGHT_PROP_DEFENSE => $"DEF+{(int)value}",
            FightPropType.FIGHT_PROP_DEFENSE_PERCENT => $"DEF+{value2:F1}%",
            FightPropType.FIGHT_PROP_CRITICAL => $"Crit Rate+{value2:F1}%",
            FightPropType.FIGHT_PROP_CRITICAL_HURT => $"Crit DMG+{value2:F1}%",
            FightPropType.FIGHT_PROP_CHARGE_EFFICIENCY => $"Energy Recharge+{value2:F1}%",
            FightPropType.FIGHT_PROP_HEAL_ADD => $"Healing Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_ELEMENT_MASTERY => $"Elemental Mastery+{(int)value}",
            FightPropType.FIGHT_PROP_PHYSICAL_ADD_HURT => $"Physical DMG Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_FIRE_ADD_HURT => $"Pyro DMG Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_ELEC_ADD_HURT => $"Electro DMG Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_WATER_ADD_HURT => $"Hydro DMG Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_GRASS_ADD_HURT => $"Dendro DMG Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_WIND_ADD_HURT => $"Anemo DMG Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_ROCK_ADD_HURT => $"Geo DMG Bonus+{value2:F1}%",
            FightPropType.FIGHT_PROP_ICE_ADD_HURT => $"Cyro DMG Bonus+{value2:F1}%",
            _ => $"{value}",
        };
	}
}
