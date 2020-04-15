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
        string directory = "";

        public MainWindow()
        {
            InitializeComponent();
            StartUp();
        }

        private void StartUp()
        {
            //string directory = System.IO.Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            //directory += "\\Video\\";
            string[] videoFiles = Directory.GetFiles(directory);

            foreach (var video in videoFiles)
            {
                cmbVideoFiles.Items.Add(video);
            }
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
            //string directory = System.IO.Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            //directory += "\\Video\\";
            string[] videoFiles = Directory.GetFiles(directory);

            foreach (var video in videoFiles)
            {
                cmbVideoFiles.Items.Add(video);
            }
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
        
        private string SendToServer(string command)
        {
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

            Socket clientSocket = new Socket(clientIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(server);

                byte[] serverResponse = new byte[8019];
                int messageLength = clientSocket.Receive(serverResponse);
                directory = Encoding.ASCII.GetString(serverResponse, 0, messageLength).ToUpper().Trim();
                GetVideoFiles();

                return null;
            }
            catch (Exception message)
            {
                return "no response from the server";
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            SendToServer(null);
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
    }
}
