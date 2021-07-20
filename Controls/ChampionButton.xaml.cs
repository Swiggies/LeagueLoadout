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

namespace LeagueLoadout.Objects
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ChampionButton : UserControl
    {
        public ChampionButton(string champURL)
        {
            InitializeComponent();
            LoadImage(champURL);
        }

        public void LoadImage(string champURL)
        {
            btn_champ.Content = new Image
            {
                Source = new BitmapImage(new Uri(champURL)),
                Stretch = Stretch.Fill
            };
            Debug.WriteLine(champURL);
        }
    }
}
