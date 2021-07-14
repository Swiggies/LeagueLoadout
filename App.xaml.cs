using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueLoadout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<RiotConnectionService>();

                client.Riot = RiotSharp.RiotApi.GetDevelopmentInstance("***REMOVED***");
            }

            MainWindow window = new MainWindow();
            window.Title = "League Loadout";
            window.Show();
            Console.WriteLine("Testing");
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<RiotConnectionService>()
                .BuildServiceProvider();
        }
    }
}
