using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        public static ServerLog ServerLog { get; } = new ServerLog();

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

        private SynchronizationContext UContext { get; set; } = SynchronizationContext.Current;

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
                ServerLog.AddLogLine();
                ServerLog.AddLogLine($"Socket server started");
                ServerLog.AddLogLine($"Listening on IP {IPAddress} and port {Port}");
                //ServerLog.Add($"Listening on port : {Port}");
                //ServerLog.Add($"Endpoint : {serverEndPoint}");
                ServerLog.AddLogLine($"Maximum number of connections : {MaxConnections}");
                while (Listening)
                {
                    ServerLog.AddLogLine();
                    ServerLog.AddLogLine($"Waiting for client to accept");
                    Client = await Listener.AcceptAsync();

                    if (!Client.Connected)
                    {
                        ServerLog.AddLogLine($"Client failed to connect");
                        continue;
                    }
                    ServerLog.AddLogLine();
                    ServerLog.AddLogLine($"Client connected {((IPEndPoint)Client.RemoteEndPoint).Address} : {((IPEndPoint)Client.RemoteEndPoint).Port}");
                    ServerLog.AddLogLine($"Number of possible connections left: {MaxConnections - ++ClientsConnected}");

                    // TODO: communicate (receive/send) with client through async task
                    var buffer = Encoding.UTF8.GetBytes(ServerURL);
                    Client.Send(buffer);
                    CommunicateWithClient(Client);
                }
                Listener.Dispose();

            }
            catch (System.Exception ex)
            {
                if (Listening)
                {
                    ServerLog.AddLogLine();
                    ServerLog.AddLogLine($"Error : {ex.Message}");
                }
            }

        }

        private void CommunicateWithClient(Socket client)
        {
            Task.Run(() =>
            {
                try
                {
                    using (client)
                    {
                        bool completed = false;
                        do
                        {
                            byte[] readBuffer = new byte[1024];
                            int read = client.Receive(readBuffer, 0, 1024, SocketFlags.None);
                            string fromClient = Encoding.UTF8.GetString(readBuffer, 0, read);
                            ServerLog.AddLogLine($"Received from client {((IPEndPoint)client.RemoteEndPoint).Address} : {((IPEndPoint)client.RemoteEndPoint).Port}: {fromClient}");
                            if (string.Compare(fromClient, "shutdown", ignoreCase: true) == 0)
                            {
                                completed = true;
                                ClientsConnected--;
                            }
                            byte[] writeBuffer = Encoding.UTF8.GetBytes("message received");
                            client.Send(writeBuffer);
                            ServerLog.AddLogLine($"Sent to client: {writeBuffer}");
                        } while (!completed);
                    }

                    ServerLog.AddLogLine("Closed stream and client socket");
                }
                catch (Exception ex)
                {
                    ServerLog.AddLogLine(ex.Message);
                }
            });



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
        ServerLog.AddLogLine();
        ServerLog.AddLogLine($"Socket server halted");
    }

}

public class ServerLog : ObservableCollection<Server>
{
        private SynchronizationContext UContext { get; set; } = SynchronizationContext.Current;
        public void AddLogLine(string serverInfo = null)
    {
            if (serverInfo == null)
            {
                this.Insert(0, new Server { ServerInfo = serverInfo });

            }
            else
            {
                UContext.Send(x =>
                  this.Insert(0, new Server { ServerInfo = $"{DateTime.Now:HH:mm:ss.fff} > " + serverInfo }), null);

            }
    }
}
}
