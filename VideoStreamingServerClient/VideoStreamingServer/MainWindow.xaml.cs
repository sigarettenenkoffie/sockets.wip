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
using System.Windows.Forms;

namespace VideoStreamingServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string videoFolder;
        public MainWindow()
        {
            InitializeComponent();
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

        }

        private void btnStopServer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
