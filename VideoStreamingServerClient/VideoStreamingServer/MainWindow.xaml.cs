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

namespace VideoStreamingServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string videoFolder;
        public IPAddress ipAddress;
        public int port;
        public Socket listener;
        private int numberOfClients = 20;

        public MainWindow()
        {
            InitializeComponent();

            DoStartup();
        }

        private void DoStartup()
        {
            cmbIp.ItemsSource = Helper.GetActiveIP4s();
            cmbIp.SelectedIndex = 0;
            videoFolder = lblVideoFolder.Content.ToString();
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

                tbkInfo.Text = $"Socket server started at : {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
                tbkInfo.Text = $"Listening to IP : {ipAddress}\n" + tbkInfo.Text;
                tbkInfo.Text = $"Endpoint : {serverEndPoint}\n" + tbkInfo.Text;
                tbkInfo.Text = $"Maximum number of connections : {numberOfClients}\n" + tbkInfo.Text;
            }
            catch (System.Exception ex)
            {

            }

        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.IsEnabled = true;
            btnStopServer.IsEnabled = false;
            grpConfig.IsEnabled = true;
            listener.Close();
            tbkInfo.Text = $"Socket server stopped at : {DateTime.Now:dd/MM/yyyy HH:mm:ss} \n" + tbkInfo.Text;
        }
    }
}
