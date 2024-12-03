// Decompiled with JetBrains decompiler
// Type: RelicService.Service.Utils
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Config;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;

#nullable enable
namespace RelicService.Service
{
  internal class Utils
  {
    public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
    {
      float num1 = Math.Min((float) maxWidth / (float) image.Width, (float) maxHeight / (float) image.Height);
      int num2 = (int) ((double) image.Width * (double) num1 * (double) Program.DpiScaleFactor);
      int num3 = (int) ((double) image.Height * (double) num1 * (double) Program.DpiScaleFactor);
      Bitmap bitmap = new Bitmap(num2, num3);
      using (Graphics graphics = Graphics.FromImage((Image) bitmap))
      {
        graphics.InterpolationMode = (InterpolationMode) 7;
        graphics.DrawImage(image, 0, 0, num2, num3);
      }
      return (Image) bitmap;
    }

    public static string FormatFightProp(FightPropType type, float value)
    {
      float num = (float) Math.Round((double) value * 100.0, 1, (MidpointRounding) 1);
      value = (float) Math.Round((double) value, 0, (MidpointRounding) 1);
      string stringAndClear;
      switch (type)
      {
        case FightPropType.FIGHT_PROP_HP:
          DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler1.AppendLiteral("生命值+");
          interpolatedStringHandler1.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler1.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_HP_PERCENT:
          DefaultInterpolatedStringHandler interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(5, 1);
          interpolatedStringHandler2.AppendLiteral("生命值+");
          interpolatedStringHandler2.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler2.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler2.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ATTACK:
          DefaultInterpolatedStringHandler interpolatedStringHandler3 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler3.AppendLiteral("攻击力+");
          interpolatedStringHandler3.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler3.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ATTACK_PERCENT:
          DefaultInterpolatedStringHandler interpolatedStringHandler4 = new DefaultInterpolatedStringHandler(5, 1);
          interpolatedStringHandler4.AppendLiteral("攻击力+");
          interpolatedStringHandler4.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler4.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler4.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_DEFENSE:
          DefaultInterpolatedStringHandler interpolatedStringHandler5 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler5.AppendLiteral("防御力+");
          interpolatedStringHandler5.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler5.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_DEFENSE_PERCENT:
          DefaultInterpolatedStringHandler interpolatedStringHandler6 = new DefaultInterpolatedStringHandler(5, 1);
          interpolatedStringHandler6.AppendLiteral("防御力+");
          interpolatedStringHandler6.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler6.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler6.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_CRITICAL:
          DefaultInterpolatedStringHandler interpolatedStringHandler7 = new DefaultInterpolatedStringHandler(5, 1);
          interpolatedStringHandler7.AppendLiteral("暴击率+");
          interpolatedStringHandler7.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler7.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler7.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_CRITICAL_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler8 = new DefaultInterpolatedStringHandler(6, 1);
          interpolatedStringHandler8.AppendLiteral("暴击伤害+");
          interpolatedStringHandler8.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler8.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler8.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_CHARGE_EFFICIENCY:
          DefaultInterpolatedStringHandler interpolatedStringHandler9 = new DefaultInterpolatedStringHandler(8, 1);
          interpolatedStringHandler9.AppendLiteral("元素充能效率+");
          interpolatedStringHandler9.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler9.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler9.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_HEAL_ADD:
          DefaultInterpolatedStringHandler interpolatedStringHandler10 = new DefaultInterpolatedStringHandler(6, 1);
          interpolatedStringHandler10.AppendLiteral("治疗加成+");
          interpolatedStringHandler10.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler10.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler10.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ELEMENT_MASTERY:
          DefaultInterpolatedStringHandler interpolatedStringHandler11 = new DefaultInterpolatedStringHandler(5, 1);
          interpolatedStringHandler11.AppendLiteral("元素精通+");
          interpolatedStringHandler11.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler11.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_PHYSICAL_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler12 = new DefaultInterpolatedStringHandler(8, 1);
          interpolatedStringHandler12.AppendLiteral("物理伤害加成+");
          interpolatedStringHandler12.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler12.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler12.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_FIRE_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler13 = new DefaultInterpolatedStringHandler(9, 1);
          interpolatedStringHandler13.AppendLiteral("火元素伤害加成+");
          interpolatedStringHandler13.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler13.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler13.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ELEC_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler14 = new DefaultInterpolatedStringHandler(9, 1);
          interpolatedStringHandler14.AppendLiteral("雷元素伤害加成+");
          interpolatedStringHandler14.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler14.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler14.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_WATER_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler15 = new DefaultInterpolatedStringHandler(9, 1);
          interpolatedStringHandler15.AppendLiteral("水元素伤害加成+");
          interpolatedStringHandler15.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler15.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler15.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_GRASS_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler16 = new DefaultInterpolatedStringHandler(9, 1);
          interpolatedStringHandler16.AppendLiteral("草元素伤害加成+");
          interpolatedStringHandler16.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler16.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler16.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_WIND_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler17 = new DefaultInterpolatedStringHandler(9, 1);
          interpolatedStringHandler17.AppendLiteral("风元素伤害加成+");
          interpolatedStringHandler17.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler17.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler17.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ROCK_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler18 = new DefaultInterpolatedStringHandler(9, 1);
          interpolatedStringHandler18.AppendLiteral("岩元素伤害加成+");
          interpolatedStringHandler18.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler18.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler18.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ICE_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler19 = new DefaultInterpolatedStringHandler(9, 1);
          interpolatedStringHandler19.AppendLiteral("冰元素伤害加成+");
          interpolatedStringHandler19.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler19.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler19.ToStringAndClear();
          break;
        default:
          DefaultInterpolatedStringHandler interpolatedStringHandler20 = new DefaultInterpolatedStringHandler(0, 1);
          interpolatedStringHandler20.AppendFormatted<float>(value);
          stringAndClear = interpolatedStringHandler20.ToStringAndClear();
          break;
      }
      return stringAndClear;
    }

    public static string FormatFightPropShort(FightPropType type, float value)
    {
      float num = (float) Math.Round((double) value * 100.0, 1, (MidpointRounding) 1);
      value = (float) Math.Round((double) value, 0, (MidpointRounding) 1);
      string stringAndClear;
      switch (type)
      {
        case FightPropType.FIGHT_PROP_HP:
          DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(3, 1);
          interpolatedStringHandler1.AppendLiteral("生命+");
          interpolatedStringHandler1.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler1.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_HP_PERCENT:
          DefaultInterpolatedStringHandler interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler2.AppendLiteral("生命+");
          interpolatedStringHandler2.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler2.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler2.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ATTACK:
          DefaultInterpolatedStringHandler interpolatedStringHandler3 = new DefaultInterpolatedStringHandler(3, 1);
          interpolatedStringHandler3.AppendLiteral("攻击+");
          interpolatedStringHandler3.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler3.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ATTACK_PERCENT:
          DefaultInterpolatedStringHandler interpolatedStringHandler4 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler4.AppendLiteral("攻击+");
          interpolatedStringHandler4.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler4.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler4.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_DEFENSE:
          DefaultInterpolatedStringHandler interpolatedStringHandler5 = new DefaultInterpolatedStringHandler(3, 1);
          interpolatedStringHandler5.AppendLiteral("防御+");
          interpolatedStringHandler5.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler5.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_DEFENSE_PERCENT:
          DefaultInterpolatedStringHandler interpolatedStringHandler6 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler6.AppendLiteral("防御+");
          interpolatedStringHandler6.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler6.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler6.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_CRITICAL:
          DefaultInterpolatedStringHandler interpolatedStringHandler7 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler7.AppendLiteral("暴击+");
          interpolatedStringHandler7.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler7.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler7.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_CRITICAL_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler8 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler8.AppendLiteral("暴伤+");
          interpolatedStringHandler8.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler8.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler8.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_CHARGE_EFFICIENCY:
          DefaultInterpolatedStringHandler interpolatedStringHandler9 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler9.AppendLiteral("充能+");
          interpolatedStringHandler9.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler9.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler9.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_HEAL_ADD:
          DefaultInterpolatedStringHandler interpolatedStringHandler10 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler10.AppendLiteral("治疗+");
          interpolatedStringHandler10.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler10.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler10.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ELEMENT_MASTERY:
          DefaultInterpolatedStringHandler interpolatedStringHandler11 = new DefaultInterpolatedStringHandler(3, 1);
          interpolatedStringHandler11.AppendLiteral("精通+");
          interpolatedStringHandler11.AppendFormatted<int>((int) value);
          stringAndClear = interpolatedStringHandler11.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_PHYSICAL_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler12 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler12.AppendLiteral("物伤+");
          interpolatedStringHandler12.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler12.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler12.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_FIRE_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler13 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler13.AppendLiteral("火伤+");
          interpolatedStringHandler13.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler13.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler13.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ELEC_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler14 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler14.AppendLiteral("雷伤+");
          interpolatedStringHandler14.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler14.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler14.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_WATER_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler15 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler15.AppendLiteral("水伤+");
          interpolatedStringHandler15.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler15.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler15.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_GRASS_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler16 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler16.AppendLiteral("草伤+");
          interpolatedStringHandler16.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler16.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler16.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_WIND_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler17 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler17.AppendLiteral("风伤+");
          interpolatedStringHandler17.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler17.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler17.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ROCK_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler18 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler18.AppendLiteral("岩伤+");
          interpolatedStringHandler18.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler18.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler18.ToStringAndClear();
          break;
        case FightPropType.FIGHT_PROP_ICE_ADD_HURT:
          DefaultInterpolatedStringHandler interpolatedStringHandler19 = new DefaultInterpolatedStringHandler(4, 1);
          interpolatedStringHandler19.AppendLiteral("冰伤+");
          interpolatedStringHandler19.AppendFormatted<float>(num, "F1");
          interpolatedStringHandler19.AppendLiteral("%");
          stringAndClear = interpolatedStringHandler19.ToStringAndClear();
          break;
        default:
          DefaultInterpolatedStringHandler interpolatedStringHandler20 = new DefaultInterpolatedStringHandler(0, 1);
          interpolatedStringHandler20.AppendFormatted<float>(value);
          stringAndClear = interpolatedStringHandler20.ToStringAndClear();
          break;
      }
      return stringAndClear;
    }
  }
}
