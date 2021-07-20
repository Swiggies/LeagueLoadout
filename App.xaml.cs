using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        private readonly ServiceProvider _serviceProvider;

        public event EventHandler<LeagueEvent> GameFlowChanged;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var riotConnection = _serviceProvider.GetService<RiotConnectionService>();
            await riotConnection.RequestAuth();
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();

            GameFlowChanged += OnGameFlowChanged;
            riotConnection.Subscribe("​/lol-perks​/v1​/currentpage", GameFlowChanged);
        }

        private void OnGameFlowChanged(object sender, LeagueEvent e)
        {
            Debug.WriteLine(e.uri);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RiotConnectionService>();
            services.AddSingleton<MainWindow>();
        }
    }
}
