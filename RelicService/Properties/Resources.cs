// Decompiled with JetBrains decompiler
// Type: RelicService.Properties.Resources
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace RelicService.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (RelicService.Properties.Resources.resourceMan == null)
          RelicService.Properties.Resources.resourceMan = new ResourceManager("RelicService.Properties.Resources", typeof (RelicService.Properties.Resources).Assembly);
        return RelicService.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable]
    internal static CultureInfo Culture
    {
      get => RelicService.Properties.Resources.resourceCulture;
      set => RelicService.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap dot_green
    {
      get
      {
        return (Bitmap) RelicService.Properties.Resources.ResourceManager.GetObject(nameof (dot_green), RelicService.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap dot_red
    {
      get
      {
        return (Bitmap) RelicService.Properties.Resources.ResourceManager.GetObject(nameof (dot_red), RelicService.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap dot_yellow
    {
      get
      {
        return (Bitmap) RelicService.Properties.Resources.ResourceManager.GetObject(nameof (dot_yellow), RelicService.Properties.Resources.resourceCulture);
      }
    }
  }
}
