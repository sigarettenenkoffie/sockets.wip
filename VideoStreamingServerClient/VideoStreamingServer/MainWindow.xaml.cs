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
        public string videoFolder = "c:\\AIT-PE-videos";
        public IPAddress ipAddress;
        public int port;
        public Socket listener;
        private int numberOfClients = 20;
        private int numberOfConnectionsLeft;
        private bool listening = true;

        public MainWindow()
        {
            InitializeComponent();
            server.VideoFolder = "c:\\AIT-PE-videos";
            this.DataContext = server;
            DoStartup();
        }

        private void DoStartup()
        {
            string videoPathInSolution = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Video");
            Helper.CloneDirectory(videoPathInSolution, videoFolder);

            //cmbIp.ItemsSource = Helper.GetActiveIP4s();
            //cmbIp.SelectedIndex = 0;
            //lblVideoFolder.Content = videoFolder;

        }

        private void btnSelectVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = lblVideoFolder.Content.ToString();
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                lblVideoFolder.Content = fbd.SelectedPath;
                server.VideoFolder = fbd.SelectedPath;
            }
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.IsEnabled = false;
            btnStopServer.IsEnabled = true;
            grpConfig.IsEnabled = false;

            server.IPAddress = IPAddress.Parse(cmbIp.SelectedItem.ToString());
            server.Port = int.Parse(txtPort.Text);
            //ipAddress = IPAddress.Parse(cmbIp.SelectedItem.ToString());
            //port = int.Parse(txtPort.Text);
            server.ExecuteServer();
        }

        private async void ExecuteServer()
        {
            numberOfConnectionsLeft = numberOfClients;
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(serverEndPoint);
                listener.Listen(backlog: numberOfClients);
                tbkInfo.Text = tbkInfo.Text.Insert(0, new string('-', 50) + "\n");
                tbkInfo.Text = tbkInfo.Text.Insert(0, $"Socket server started at : {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
                tbkInfo.Text = tbkInfo.Text.Insert(0, $"Listening to IP : {ipAddress}\n");
                tbkInfo.Text = tbkInfo.Text.Insert(0, $"Listening on port : {port}\n");
                tbkInfo.Text = tbkInfo.Text.Insert(0, $"Endpoint : {serverEndPoint}\n");
                tbkInfo.Text = tbkInfo.Text.Insert(0, $"Maximum number of connections : {numberOfClients}\n");

                while (listening)
                {
                    tbkInfo.Refresh();


                    tbkInfo.Text = tbkInfo.Text.Insert(0, $"Waiting for client to accept\n");
                    Socket client = await listener.AcceptAsync(); // Blocks the thread until client connects
                    if (!client.Connected)
                    {
                        tbkInfo.Text = tbkInfo.Text.Insert(0, $"Client failed to connect\n");
                        continue;
                    }
                    tbkInfo.Text = tbkInfo.Text.Insert(0, $"Client connected through address : {((IPEndPoint)client.RemoteEndPoint).Address} and port {((IPEndPoint)client.RemoteEndPoint).Port}\n");
                    tbkInfo.Text = tbkInfo.Text.Insert(0, $"Number of possible connections left: {--numberOfConnectionsLeft}\n");

                    var buffer = Encoding.UTF8.GetBytes(videoFolder);

                    //StartSending();
                    

                    client.Send(buffer);

                }

                listener.Dispose();

            }
            catch (System.Exception ex)
            {
                if (listening)
                {
                    tbkInfo.Text = $"Error : {ex.Message} \n" + tbkInfo.Text;
                }
            }
        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.IsEnabled = true;
            btnStopServer.IsEnabled = false;
            grpConfig.IsEnabled = true;
            listening = false;
            try
            {
                listener.Close();
            }
            catch
            { }
            listener = null;

            tbkInfo.Text = $"Socket server stopped at : {DateTime.Now:dd/MM/yyyy HH:mm:ss} \n" + tbkInfo.Text;
        }

        private void cmbIp_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
