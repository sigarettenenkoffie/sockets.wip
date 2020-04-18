using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using SocketLib;

namespace VideoStreamingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client = new Client();

        public MainWindow()
        {
            InitializeComponent();
            StartUp();
        }

        private void StartUp()
        {
            btnDisconnect.IsEnabled = false;
            btnPlay.IsEnabled = false;
            btnPause.IsEnabled = false;
            btnStop.IsEnabled = false;
        }

        private void GetVideoFiles()
        {
            string[] videoFiles = client.GetFiles();

            foreach (var video in videoFiles)
            {
                cmbVideoFiles.Items.Add(video);
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(txtPort.Text);
            string ip = txtServerIP.Text.Trim();
            IPAddress serverIP;
            string connection;

            if (client.CheckIP(ip) == true)
            {
                serverIP = IPAddress.Parse(ip);
                connection = client.Send(serverIP, port);
                GetVideoFiles();

                btnDisconnect.IsEnabled = true;
                btnConnect.IsEnabled = false;
            }
            else
            {
                ip = "127.0.0.1";
                txtServerIP.Text = ip;
                connection = $"IP address was incorrect\nIp address has been set to: {ip}\nPlease reconnect or give another ip address";
            }
             
            lstClientBox.Items.Add(connection);         
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mdaVideoPlayer.Source = new System.Uri(cmbVideoFiles.SelectedItem.ToString());
            mdaVideoPlayer.Play();
            btnPlay.IsEnabled = false;
            btnPause.IsEnabled = true;
            btnStop.IsEnabled = true;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mdaVideoPlayer.Pause();
            btnPause.IsEnabled = false;
            btnPlay.IsEnabled = true;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mdaVideoPlayer.Stop();
            btnStop.IsEnabled = false;
            btnPlay.IsEnabled = true;
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            string disconnected;

            disconnected = client.Disconnect();
            lstClientBox.Items.Add(disconnected);
            btnConnect.IsEnabled = true;
        }
    }
}
