using System;
using System.Collections.Generic;
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

namespace LeagueLoadout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RiotConnectionService RiotConnection { get; set; }

        public MainWindow(RiotConnectionService riotConnectionService)
        {
            InitializeComponent();
            RiotConnection = riotConnectionService;
        }
            
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await RiotConnection.RequestAuth();
        }
    }
}
