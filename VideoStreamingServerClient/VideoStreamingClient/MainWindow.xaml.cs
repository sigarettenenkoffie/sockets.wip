using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace VideoStreamingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int serverPort = 0;
        IPAddress serverIP;
        Socket clientSocket;
        string directory = "";


        public MainWindow()
        {
            InitializeComponent();
            StartUp();
        }

        private void StartUp()
        {
            btnDisconnect.IsEnabled = false;
        }

        private void ReadConfiguration()
        {

            int.TryParse(txtPort.Text, out serverPort);

            string ip = txtServerIP.Text.Trim();

            try
            {
                serverIP = IPAddress.Parse(ip);
            }
            catch
            {
                ip = "127.0.0.1";
                txtServerIP.Text = ip;
            }
        }

        private void GetVideoFiles()
        {
            string[] videoFiles = Directory.GetFiles(directory);

            foreach (var video in videoFiles)
            {
                cmbVideoFiles.Items.Add(video);
            }
        }

        private void SendToServer()
        {
            string connection;

            ReadConfiguration();

            IPEndPoint server = new IPEndPoint(serverIP, serverPort);
            IPHostEntry clientHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress clientIP = clientHostInfo.AddressList[0];

            foreach (var ip in clientHostInfo.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    clientIP = ip;
                    break;
                }
            }

            clientSocket = new Socket(clientIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(server);

                byte[] serverResponse = new byte[8019];
                int messageLength = clientSocket.Receive(serverResponse);
                directory = Encoding.ASCII.GetString(serverResponse, 0, messageLength).ToUpper().Trim();
                GetVideoFiles();

                connection = "Succesful connected to server";
                lstClientBox.Items.Add(connection);
                btnDisconnect.IsEnabled = true;
                btnConnect.IsEnabled = false;
            }
            catch
            {
                connection = "No response from the server";
                lstClientBox.Items.Add(connection);
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            SendToServer();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mdaVideoPlayer.Source = new System.Uri(cmbVideoFiles.SelectedItem.ToString());
            mdaVideoPlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mdaVideoPlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mdaVideoPlayer.Stop();
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            //string line = "shutdown";
            //byte[] buffer = Encoding.UTF8.GetBytes($"shutdown");
            clientSocket.Send(Encoding.UTF8.GetBytes($"shutdown"));
            //clientSocket.Disconnect(true);
            string disconnected = "Disconnected from the server";
            lstClientBox.Items.Add(disconnected);
            btnConnect.IsEnabled = true;
        }
    }
}
