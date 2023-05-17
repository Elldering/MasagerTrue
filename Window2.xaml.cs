using Newtonsoft.Json;
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
        string Name_podkl;
        private string ClientId;
        public Window2(string user, string ip)
        {
            InitializeComponent();
            MessageBox.Show(user);
            Name_podkl = user;
            ClientId = Guid.NewGuid().ToString();
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip, 8888);
                RecieveMessage(user);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения");
                this.Close();

            }
            send(" ");
        }

        private async Task RecieveMessage(string user)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await socket.ReceiveAsync(bytes, SocketFlags.None);

                string json = Encoding.UTF8.GetString(bytes);
                Dictionary<string, string> userNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                List<string> AlU = new List<string>();
                foreach (var kvp in userNames)
                {
                    string userName = kvp.Key;
                    string message_recieve = kvp.Value;

                    if (message_recieve == "/AllUsers")
                    {
                        if (!AllUsers.Items.Contains(userName))
                        {
                            AlU.Add(userName);
                        }
                    }
                    else
                    {
                        AllMessage.Items.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {userName}: {message_recieve}");
                    }

                    if (message_recieve == "/disconnect")
                    {
                        MessageBox.Show("Сервер завершил сессию");
                        this.Close();
                    }
                }

                for (int i = AllUsers.Items.Count - 1; i >= 0; i--)
                {
                    string item = AllUsers.Items[i].ToString();
                    if (!AlU.Contains(item))
                    {
                        AllUsers.Items.RemoveAt(i);
                    }
                }
                for (int i = 0; i < AlU.Count; i++)
                {
                    string userName = AlU[i];
                    AllUsers.Items.Add(userName);
                }
                AlU.Clear();

            }
        }


        private async Task send(string msg)
        {
            Dictionary<string, string> messageData = new Dictionary<string, string>();
            messageData.Add(Name_podkl, msg);

            string json = JsonConvert.SerializeObject(messageData);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await socket.SendAsync(bytes, SocketFlags.None);
            messageData.Clear();


        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void send_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (Massage_TextBox.Text == "/disconnect")
            {
                MainWindow mainWindow = new MainWindow();
                send(Massage_TextBox.Text);

                mainWindow.Show();
                this.Close();
            }
            else
            {
                send(Massage_TextBox.Text);
            }
        }

        private void out_BTN_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow mainWindow = new MainWindow();
            send("/disconnect");
            this.Close();
            return;
        }

        private void Client_window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            send("/disconnect");
            MainWindow dd = new MainWindow();
            dd.Show();
            e.Cancel = false;


        }
    }
}