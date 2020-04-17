using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketLib
{
    public class Server
    {
        public List<string> ActiveIP4s { get; } = Helper.GetActiveIP4s();
        public string ServerURL { get; set; }
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; } = 80;
        private Socket Listener { get; set; }
        private const int MaxConnections = 20;
        private int ClientsConnected { get; set; } = 0;
        public bool Listening { get; set; } = true;
        public ServerLog ServerLog {get; } = new ServerLog();

        private string _ServerInfo = "";
        public string ServerInfo
        {
            get { return _ServerInfo; }
            set 
            { 
                _ServerInfo = value;
                ServerInfoChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerInfo)));
            }
        }

        private Socket Client { get; set; }

        public event PropertyChangedEventHandler ServerInfoChanged;

        public Server()
        {         
        }

        public async void Start()
        {
            Listening = true;
            // TODO: start server through task
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress, Port);
            Listener = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Listener.Bind(serverEndPoint);
                Listener.Listen(backlog: MaxConnections);
                ServerLog.Add(null);
                ServerLog.Add($"Socket server started at : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
                ServerLog.Add($"Listening to IP : {IPAddress}");
                ServerLog.Add($"Listening on port : {Port}");
                ServerLog.Add($"Endpoint : {serverEndPoint}");
                ServerLog.Add($"Maximum number of connections : {MaxConnections}");
                while (Listening)
                {
                    ServerLog.Add(null);
                    ServerLog.Add($"Waiting for client to accept");
                    Client = await Listener.AcceptAsync();

                    if (!Client.Connected)
                    {
                        ServerLog.Add($"Client failed to connect");
                        continue;
                    }
                    ServerLog.Add(null);
                    ServerLog.Add($"Client connected {((IPEndPoint)Client.RemoteEndPoint).Address} : {((IPEndPoint)Client.RemoteEndPoint).Port}");
                    ServerLog.Add($"Number of possible connections left: {MaxConnections - ++ClientsConnected}");

                    // TODO: communicate (receive/send) with client through async task
                    Task t = CommunicateWithClientAsync(Client);
                    var buffer = Encoding.UTF8.GetBytes(ServerURL);
                    Client.Send(buffer);
                }
                Listener.Dispose();

            }
            catch (System.Exception ex)
            {
                if (Listening)
                {
                    ServerLog.Add(null);
                    ServerLog.Add($"Error : {ex.Message}");
                }
            }
            
        }

        private async Task CommunicateWithClientAsync(Socket client)
        {


        }

        public void Stop()
        {
            Listening = false;
            try
            {
                Listener.Close();
            }
            catch
            {
            	
            }
            Listener = null;
            ServerLog.Add(null);
            ServerLog.Add($"Socket server stopped at : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        }

    }

    public class ServerLog : ObservableCollection<Server>
    {
        public void Add(string serverInfo)
        {
            if (serverInfo == null)
                this.Insert(0, new Server { ServerInfo = serverInfo });
            else
                this.Insert(0,  new Server { ServerInfo = $"{DateTime.Now:HH:mm:ss.fff} > " + serverInfo });
        }
    }
}
