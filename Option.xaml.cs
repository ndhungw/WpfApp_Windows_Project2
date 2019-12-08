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
using System.Windows.Shapes;

namespace WpfApp_Windows_Project2
{
    /// <summary>
    /// Interaction logic for Option.xaml
    /// </summary>
    public partial class Option : Window
    {
        public Option()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimerLimitTxtBox.Text = Business.defaultTimePlay.ToString();
            if (Business.hintHasPenalty) PenaltyTrue.IsChecked = true;
            else PenaltyFalse.IsChecked = true;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Business.SetOption(int.Parse(TimerLimitTxtBox.Text.ToString()), PenaltyTrue.IsChecked==true);
                this.Close();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                TimerLimitTxtBox.Text = Business.defaultTimePlay.ToString();
                if (Business.hintHasPenalty) PenaltyTrue.IsChecked = true;
                else PenaltyFalse.IsChecked = true;
            }
            
        }
    }
}
