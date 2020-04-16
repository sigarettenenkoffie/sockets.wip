using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketLib
{
    public class Server
    {
        public List<string> ActiveIP4s { get; set; } = Helper.GetActiveIP4s();
        public string VideoFolder { get; set; }
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; } = 80;
        public Socket Listener { get; set; }
        private const int MaxConnections = 20;
        private int NumberOfConnectionsLeft { get; set; }
        private bool Listening { get; set; } = true;

        public Server()
        {

        }

        public async void ExecuteServer()
        {
            
        }

    }
}
