using Microsoft.Extensions.DependencyInjection;
using WinForm.Database;
using WinForm.Forms;
using WinForm.Services.ShowMessage.getShowMessage.DTOs;
using WinForm.Services.ShowMessage.getShowMessage.Model;
using static Dapper.SqlMapper;

namespace WinForm
{
    public static class Program
    {
        public static IServiceProvider ServiceProvider = null!;
        public static DatabaseContext Database { get; private set; } = null!;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);
            //var tempProvider = services.BuildServiceProvider();
            ServiceProvider = services.BuildServiceProvider();
            Database = ServiceProvider.GetRequiredService<DatabaseContext>();
            if(Database.CreateConnection()==false)
            {
                MessageBox.Show("اتصال برقرار نشد");
            }


            Application.Run(new Dashboard());

        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // =========================
            // Forms
            // =========================
            //services.AddTransient<VideoWall.WinForms.Forms.frmDashboard>();

            // =========================
            // UserControls
            // =========================
            //services.AddTransient<VideoWall.WinForms.UserControls.ucSwitch>();
            
            // =========================
            // Services
            // =========================
            //services.AddApplicationServices();

            //=======[ Singleton Services ]=======
            //مدیریت فرم دیسپلی
            //services.AddSingleton<IfrmDisplay, frmDisplayService>();
            //مدیریت تمام فرمها یکجا
            //services.AddSingleton<ISwitchUiCoordinator, SwitchUiCoordinatorService>();

            //services.AddSingleton<StudioEditorContext>();
            services.AddSingleton<DatabaseContext>();


        }//private static void ConfigureServices

        public static dtoShowMessageResult ShowMessage(modelShowMessage _showMessage)
        {
            MessageBox.Show(_showMessage.Message, _showMessage.Title);
            return new dtoShowMessageResult
            {
                isActive = true
            };
        }

    }



}