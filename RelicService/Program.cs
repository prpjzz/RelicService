// Decompiled with JetBrains decompiler
// Type: RelicService.Program
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Service;
using RelicService.Tools;
using RelicService.View;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Vanara.PInvoke;

#nullable enable
namespace RelicService
{
  internal static class Program
  {
    public static readonly int Build = 10000;

    public static System.IServiceProvider ServiceProvider { get; private set; } = (System.IServiceProvider) Program.CreateServiceProvider();

    public static float DpiScaleFactor { get; set; } = 1f;

    [STAThread]
    private static void Main()
    {
      Kernel32.CreateMutex((SECURITY_ATTRIBUTES) null, true, "7F7EEAED-E191-4EE3-83DC-61D02BCCBDA1");
      if (Marshal.GetLastWin32Error() == 183)
      {
        int num = (int) MessageBox.Show("已有实例在运行", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        Program.ServiceProvider.GetRequiredService<SqliteContext>().Database.Migrate();
        // ISSUE: reference to a compiler-generated method
        ApplicationConfiguration.Initialize();
        Application.Run((Form) Program.ServiceProvider.GetRequiredService<MainForm>());
      }
    }

    private static Microsoft.Extensions.DependencyInjection.ServiceProvider CreateServiceProvider()
    {
      ServiceCollection serviceCollection = new ServiceCollection();
      serviceCollection.AddTransient<MainForm>();
      serviceCollection.AddTransient<AddPresetForm>();
      serviceCollection.AddTransient<ProfileDetails>();
      serviceCollection.AddTransient<AvatarSelectionForm>();
      serviceCollection.AddTransient<ProfileEditForm>();
      serviceCollection.AddTransient<AboutForm>();
      serviceCollection.AddSingleton<EventManager>();
      serviceCollection.AddSingleton<Network>();
      serviceCollection.AddSingleton<ResourceManager>();
      serviceCollection.AddDbContext<SqliteContext>();
      serviceCollection.AddSingleton<StatusService>();
      serviceCollection.AddSingleton<AvatarService>();
      serviceCollection.AddSingleton<EquipService>();
      serviceCollection.AddSingleton<AutoEquipService>();
      serviceCollection.AddSingleton<GameMessageService>();
      serviceCollection.AddSingleton<ApiService>();
      return serviceCollection.BuildServiceProvider();
    }
  }
}
