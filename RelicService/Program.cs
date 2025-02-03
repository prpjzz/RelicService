using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Service;
using RelicService.Tools;
using RelicService.View;
using Vanara.PInvoke;

namespace RelicService;

internal static class Program
{
	public static readonly int Build = 10004;

	public static IServiceProvider ServiceProvider { get; private set; } = CreateServiceProvider();

	public static float DpiScaleFactor { get; set; } = 1f;

	[STAThread]
	private static void Main()
	{
		Kernel32.CreateMutex(null, bInitialOwner: true, "7F7EEAED-E191-4EE3-83DC-61D02BCCBDA1");
		if (Marshal.GetLastWin32Error() == 183)
		{
			MessageBox.Show("Relic Service Already Running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Directory.SetCurrentDirectory(AppContext.BaseDirectory);
		ServiceProvider.GetRequiredService<SqliteContext>().Database.Migrate();
		ApplicationConfiguration.Initialize();
		Application.Run(ServiceProvider.GetRequiredService<MainForm>());
	}

	private static ServiceProvider CreateServiceProvider()
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
