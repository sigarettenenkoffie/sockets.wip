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
using SocketLib;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Path = System.IO.Path;
using System.Reflection;
using System.Windows.Threading;
using System.Net.NetworkInformation;

namespace VideoStreamingServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server server = new Server();
        public string videoServerURL = "c:\\AIT-PE-videos";
        public IPAddress ipAddress;
        public int port;
        public Socket listener;


        public MainWindow()
        {
            InitializeComponent();
            server.ServerURL = "c:\\AIT-PE-videos";
            this.DataContext = server;
            DoStartup();
        }

        private void DoStartup()
        {
            string videoPathInSolution = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Video");
            Helper.CloneDirectory(videoPathInSolution, videoServerURL);
        }

        private void btnSelectVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = lblVideoFolder.Content.ToString();
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                lblVideoFolder.Content = fbd.SelectedPath;
                server.ServerURL = fbd.SelectedPath;
            }
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.IsEnabled = false;
            btnStopServer.IsEnabled = true;
            grpConfig.IsEnabled = false;

            server.IPAddress = IPAddress.Parse(cmbIp.SelectedItem.ToString());
            server.Port = int.Parse(txtPort.Text);
            server.Start();
        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.IsEnabled = true;
            btnStopServer.IsEnabled = false;
            grpConfig.IsEnabled = true;
            server.Stop();
        }
    }
}
