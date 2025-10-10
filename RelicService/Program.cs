using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelicService.Data.Database;
using RelicService.Tools;
using RelicService.Service;
using RelicService.View;
using Vanara.PInvoke;

namespace RelicService
{
    internal static class Program
    {

        public static IServiceProvider ServiceProvider { get; private set; } = CreateServiceProvider();
        public static float DpiScaleFactor { get; set; } = 1.0f;
        public static readonly int Build = 10006;

        [STAThread]
        static void Main()
        {
            Kernel32.CreateMutex(null, true, "7F7EEAED-E191-4EE3-83DC-61D02BCCBDA1");
            if (Marshal.GetLastWin32Error() == 183) // ERROR_ALREADY_EXISTS
            {
                MessageBox.Show(@"已有实例在运行", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // enable high DPI support
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            var programDirectory = AppContext.BaseDirectory;
            Directory.SetCurrentDirectory(programDirectory); // ensure the current directory is the program directory

            var ctx = ServiceProvider.GetRequiredService<SqliteContext>();
#if DEBUG
            ctx.Database.EnsureCreated();
#else
            ctx.Database.Migrate();
#endif

            ApplicationConfiguration.Initialize();
            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }

        static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            // forms
            services.AddTransient<MainForm>();
            services.AddTransient<AddPresetForm>();
            services.AddTransient<ProfileDetails>();
            services.AddTransient<AvatarSelectionForm>();
            services.AddTransient<ProfileEditForm>();
            services.AddTransient<AboutForm>();

            // managers
            services.AddSingleton<EventManager>();
            services.AddSingleton<Network>();
            services.AddSingleton<ResourceManager>();

            // data
            services.AddDbContext<SqliteContext>();

            // services
            services.AddSingleton<StatusService>();
            services.AddSingleton<AvatarService>();
            services.AddSingleton<EquipService>();
            services.AddSingleton<AutoEquipService>();
            services.AddSingleton<GameMessageService>();
            services.AddSingleton<ApiService>();

            return services.BuildServiceProvider();
        }

    }
}