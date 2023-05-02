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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Create_chat_BTN.IsEnabled = IsEnabled;
            join_BTN.IsEnabled = IsEnabled;
            CheckMusicPlayingPeriodically();
            List<string> name_collection = new List<string>();
            name_collection.Add("Адмэн");
            name_collection.Add("ArcVarden");
            name_collection.Add("goblik");
            name_collection.Add("крутышка");
            predlojeniya.ItemsSource = name_collection;
        }

        private void Create_chat_BTN_Click(object sender, RoutedEventArgs e)
        {
            Window mainForm = new Window1();
            mainForm.Show();
            this.Close();
        }

        private void join_BTN_Click(object sender, RoutedEventArgs e)
        {
            Window mainForm = new Window2(UserName.Text, IP.Text);
            mainForm.Show();
            this.Close();
        }
        private async void CheckMusicPlayingPeriodically()
        {
            while (true)
            {
                if (string.IsNullOrEmpty(UserName.Text))
                {
                    Create_chat_BTN.IsEnabled = !IsEnabled;
                }
                else
                {
                    Create_chat_BTN.IsEnabled = IsEnabled;
                }
                if ((string.IsNullOrEmpty(UserName.Text) || string.IsNullOrEmpty(IP.Text)))
                {
                    join_BTN.IsEnabled = !IsEnabled;
                }
                else
                {
                    join_BTN.IsEnabled = IsEnabled;
                }
                await Task.Delay(10);

            }
        }

        private void predlojeniya_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserName.Text = predlojeniya.SelectedItem.ToString();
        }
    }
}
