using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private Socket socket;
        private CancellationTokenSource isWorking;
        public Window2(string user, string ip)
        {
            InitializeComponent();
            MessageBox.Show(user);

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip, 8888);
                RecieveMessage(user);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения");
                return;
            }
        }

        private async Task RecieveMessage(string user)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await socket.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                AllMessage.Items.Add(DateTime.Now.ToString() + " [" + user + "]: " + message);
            }
        }

        private async Task send(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            await socket.SendAsync(bytes, SocketFlags.None);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void send_BTN_Click(object sender, RoutedEventArgs e)
        {
            send(Massage_TextBox.Text);
        }
    }
}
