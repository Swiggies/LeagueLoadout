using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LeagueLoadout.Objects;

namespace LeagueLoadout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RiotConnectionService RiotConnection { get; set; }

        private const string spriteURL = "https://ddragon.leagueoflegends.com/cdn/11.14.1/img/champion/";

        public MainWindow(RiotConnectionService riotConnectionService)
        {
            InitializeComponent();
            RiotConnection = riotConnectionService;


        }
            
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //await RiotConnection.RequestAuth();
            await RiotConnection.SendRequest();
        }

        private async void Window_Loaded(object sender, EventArgs e)
        {
            var champs = await RiotConnection.RetrieveDragonData();
            int row = 0;
            int col = 0;

            foreach (var champ in champs)
            {
                ChampionButton championButton = new ChampionButton($"{spriteURL}{champ.First["image"]["full"]}");
                Grid.SetRow(championButton, row);
                Grid.SetColumn(championButton, col++);
                champGrid.Children.Add(championButton);

                if (col % champGrid.ColumnDefinitions.Count == 0) 
                {
                    champGrid.RowDefinitions.Add(new RowDefinition());
                    col = 0;
                    row++;
                };
            }
        }
    }
}
