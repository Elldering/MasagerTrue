using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Socket socket;
        private List<Socket> clients = new List<Socket>();
        public string adress;
        public string server_name;
        ObservableCollection<string> AllMessage_Logs = new ObservableCollection<string>();
        ObservableCollection<string> AllMessage_Osnova = new ObservableCollection<string>();
        Dictionary<string, string> Name_IP = new Dictionary<string, string>();
        private bool isShowLogs = true;
        private bool isAllMessage = true;
        public Window1(string server_nam)
        {
            server_name = server_nam;
            InitializeComponent();
            AllUsers.Items.Add(server_nam);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            MessageBox.Show(ipPoint.ToString());
            Name_IP[ipPoint.ToString()] = server_nam;
            socket.Listen(20);
            ListenerToUsers();
            AllMessage.ItemsSource = AllMessage_Osnova;
        }
        private async Task ListenerToUsers()
        {
            while (true)
            {

                var client = await socket.AcceptAsync();
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string name = Encoding.UTF8.GetString(bytes);
                Dictionary<string, string> messageData = JsonConvert.DeserializeObject<Dictionary<string, string>>(name);
                string userName = null;
                string message = null;
                foreach (var kvp in messageData)
                {
                    userName = kvp.Key;
                    message = kvp.Value;
                }
                string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();

                Name_IP[ip] = userName;
                AllMessage_Logs.Add(userName + " - пользователь подключился!");
                AllUsers.Items.Add(userName);
                clients.Add(client);

                Recieved(client);
                foreach (var item in clients)
                {
                    foreach (var item2 in Name_IP.Values)
                    {

                        SendMessage(item, "/AllUsers", item2.ToString());
                    }
                    
                }

            }
        }
        private async Task Recieved(Socket client)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string name = Encoding.UTF8.GetString(bytes);
                Dictionary<string, string> messageData = JsonConvert.DeserializeObject<Dictionary<string, string>>(name);
                string userName = null;
                string message = null;
                foreach (var kvp in messageData)
                {
                    userName = kvp.Key;
                    message = kvp.Value;

                }
                if (message == "/disconnect")
                {
                    string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
                    if (true)
                    {

                    }
                    string disconnectedUser = Name_IP[ip];
                    AllMessage_Logs.Add(disconnectedUser + " - пользователь отключился!");
                    AllUsers.Items.Remove(disconnectedUser);
                    clients.Remove(client);
                    Name_IP.Remove(ip);
                    break;
                }
                AllMessage_Osnova.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {userName}: {message}");
                foreach (var item in clients)
                {
                    SendMessage(item, message, userName);
                }
            }


        }

        private async Task send(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            await socket.SendAsync(bytes, SocketFlags.None);
        }
        private async Task SendMessage(Socket UsEr, string soobsh, string Name)
        {
            Dictionary<string, string> messageData = new Dictionary<string, string>();
            messageData.Add(Name, soobsh);
            string json = JsonConvert.SerializeObject(messageData);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await UsEr.SendAsync(bytes, SocketFlags.None);
        }

        private void send_BTN_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in clients)   
            {
                SendMessage(item, Massage_TextBox.Text , server_name);
            }
            AllMessage_Osnova.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}[{server_name}]: {Massage_TextBox.Text}");

            if (Massage_TextBox.Text == "/disconnect")
            {
                MainWindow mainWindow = new MainWindow();

                socket.Close();
                this.Close();
            }
        }

        private void seeusers_Click(object sender, RoutedEventArgs e)
        {
            foreach (var client in clients)
            {
                MessageBox.Show(((IPEndPoint)client.RemoteEndPoint).Address.ToString());
            }
            
        }

        private void out_BTN_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            socket.Close();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var item in clients)
            {
                SendMessage(item, "/disconnect", server_name);
            }
            MainWindow dd = new MainWindow();
            dd.Show();
            e.Cancel = false;
        }

        private void logs_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (isShowLogs)
            {
                AllUsers.Items.Clear();
                foreach (var kvp in Name_IP)
                {
                    string item = $"{kvp.Key} : {kvp.Value}";
                    AllUsers.Items.Add(item);
                }

                AllMessage.ItemsSource = AllMessage_Logs;
                isShowLogs = false;
            }
            else
            {
                AllUsers.Items.Clear();
                foreach (var kvp in Name_IP)
                {
                    AllUsers.Items.Add(kvp.Value);
                }

                AllMessage.ItemsSource = null; 
                AllMessage.ItemsSource = AllMessage_Osnova; 
                isShowLogs = true;
            }
        }
    }
}
