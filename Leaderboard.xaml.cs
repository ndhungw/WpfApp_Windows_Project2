using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace WpfApp_Windows_Project2
{
    /// <summary>
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    public partial class Leaderboard : Window
    {
        public class Player
        {
            public String Name { get; set; }
            public int Time { get; set; }
        }

        BindingList<Player> listPlayer;
        public Leaderboard()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listPlayer = new BindingList<Player>();
            string Dir = $"{AppDomain.CurrentDomain.BaseDirectory}leaderboard.txt";
            if (!File.Exists(Dir))
                return;
            var reader = new StreamReader(Dir);
            LeaderboardListView.ItemsSource = listPlayer;

            while (true)
            {
                string result = reader.ReadLine();
                if (result == null)
                    break;
                var token = result.Split(new String[] { "|" }, StringSplitOptions.None);
                listPlayer.Add(new Player() { Name = token[0], Time = int.Parse(token[1]) });
            }


        }
    }
}
