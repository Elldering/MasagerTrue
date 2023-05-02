using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Socket socket;
        private List<Socket> clients = new List<Socket>();
        string adress;
        List<string> AllMessage_Logs = new List<string>();
        public Window1()
        {
            InitializeComponent();

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(1000);
            ListenerToUsers();
        }
        private async Task ListenerToUsers()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                AllUsers.Items.Add(client);
                clients.Add(client);
                Recieved(client);
            }
        }
        private async Task Recieved(Socket client)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string massage = Encoding.UTF8.GetString(bytes);
                AllMessage.Items.Add($"<{client.RemoteEndPoint}>: {massage}");
                AllMessage_Logs.Add(massage);

                foreach (var item in clients)
                {
                    SendMessage(item, massage);
                }
            }



        }

        private async Task send(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            await socket.SendAsync(bytes, SocketFlags.None);
        }
        private async Task SendMessage(Socket UsEr, string soobsh)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(soobsh);
            await UsEr.SendAsync(bytes, SocketFlags.None);
        }

        private void send_BTN_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in clients)   
            {
                SendMessage(item, Massage_TextBox.Text);
            }
            AllMessage.Items.Add($"<{IPAddress.Any}>: {Massage_TextBox.Text}");
        }
    }
}
