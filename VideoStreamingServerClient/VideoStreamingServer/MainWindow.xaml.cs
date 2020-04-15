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

namespace VideoStreamingServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string videoFolder = "c:\\AIT-PE-videos";
        public IPAddress ipAddress;
        public int port;
        public Socket listener;
        private int numberOfClients = 20;
        private int numberOfConnectionsLeft;
        private bool keepOnGoing = true;

        public MainWindow()
        {
            InitializeComponent();
            DoStartup();
        }

        private void DoStartup()
        {
            string videoPathInSolution = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Video");
            Helper.CloneDirectory(videoPathInSolution, videoFolder);

            cmbIp.ItemsSource = Helper.GetActiveIP4s();
            cmbIp.SelectedIndex = 0;
            lblVideoFolder.Content = videoFolder;
        }

        private void btnSelectVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = lblVideoFolder.Content.ToString();
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                lblVideoFolder.Content = fbd.SelectedPath;
                videoFolder = fbd.SelectedPath;
            }
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.IsEnabled = false;
            btnStopServer.IsEnabled = true;
            grpConfig.IsEnabled = false;

            ipAddress = IPAddress.Parse(cmbIp.SelectedItem.ToString());
            port = int.Parse(txtPort.Text);
            ExecuteServer();
        }

        private void ExecuteServer()
        {
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

                keepOnGoing = false;
                while (keepOnGoing)
                {
                    if (listener.Poll(1000000, SelectMode.SelectRead)) // lost hang op
                    {
                        tbkInfo.Text = tbkInfo.Text.Insert(0, $"Waiting for client to accept\n");
                        Socket client = listener.Accept(); // Blocks the thread until client connects
                        if (!client.Connected)
                        {
                            tbkInfo.Text = $"Client failed to connect\n" + tbkInfo.Text;
                            continue;
                        }
                        tbkInfo.Text = tbkInfo.Text.Insert(0, $"Client connected through address : {((IPEndPoint)client.LocalEndPoint).Address} and port {((IPEndPoint)client.LocalEndPoint).Port}");
                        tbkInfo.Text = tbkInfo.Text.Insert(0, $"Number of possible connections left: {numberOfConnectionsLeft--}");
                        
                        var buffer = Encoding.UTF8.GetBytes(videoFolder);
                        client.Send(buffer);

                        //CommunicateWithClientAsync(client);
                        keepOnGoing = false; // temporary !!!
                    }
                }

                listener.Dispose();



            }
            catch (System.Exception ex)
            {
                if (keepOnGoing)
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
            keepOnGoing = false;
            try
            {
                listener.Close();
            }
            catch
            { }
            listener = null;

            tbkInfo.Text = $"Socket server stopped at : {DateTime.Now:dd/MM/yyyy HH:mm:ss} \n" + tbkInfo.Text;
        }
    }
}
