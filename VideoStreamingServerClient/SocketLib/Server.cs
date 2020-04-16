using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        public ServerInfoList ServerInfoList {get; } = new ServerInfoList();
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

        public event PropertyChangedEventHandler ServerInfoChanged;
        private Socket Client { get; set; }



        public Server()
        {
            ServerInfo.Insert(0, new string('-', 50) + "\n");
        }

        public async void Start()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress, Port);
            Listener = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Listener.Bind(serverEndPoint);
                Listener.Listen(backlog: MaxConnections);
                ServerInfoList.Add(new string('-', 50) + "\n");
                ServerInfoList.Add($"Socket server started at : {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
                ServerInfoList.Add($"Listening to IP : {IPAddress}\n");
                ServerInfoList.Add($"Listening on port : {Port}\n");
                ServerInfoList.Add($"Endpoint : {serverEndPoint}\n");
                ServerInfoList.Add($"Maximum number of connections : {MaxConnections}\n");
                while (Listening)
                {
                    ServerInfoList.Add($"Waiting for client to accept\n");
                    Client = await Listener.AcceptAsync();

                    if (!Client.Connected)
                    {
                        ServerInfoList.Add($"Client failed to connect\n");
                        continue;
                    }
                    ServerInfoList.Add($"Client connected {((IPEndPoint)Client.RemoteEndPoint).Address} : {((IPEndPoint)Client.RemoteEndPoint).Port}\n");
                    ServerInfoList.Add($"Number of possible connections left: {MaxConnections - ++ClientsConnected}\n");

                    var buffer = Encoding.UTF8.GetBytes(ServerURL);
                    Client.Send(buffer);
                }
                Listener.Dispose();

            }
            catch (System.Exception ex)
            {
                if (Listening)
                    ServerInfoList.Add($"Error : {ex.Message} \n");
            }
            
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
            ServerInfoList.Add($"Socket server stopped at : {DateTime.Now:dd/MM/yyyy HH:mm:ss} \n");
        }

    }

    public class ServerInfoList : ObservableCollection<Server>
    {
        public void Add(string serverInfo)
        {
            this.Insert(0, new Server { ServerInfo = serverInfo });
        }
    }
}
