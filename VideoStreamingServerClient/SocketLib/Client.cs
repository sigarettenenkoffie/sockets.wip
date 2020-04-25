using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketLib
{
    public class Client
    {
        public int ServerPort { get; set; }
        public IPAddress ServerIP { get; set; }
        public Socket ClientSocket { get; set; }

        public Client()
        {

        }

        public string Send(IPAddress serverIP, int port)
        {
            string connection;

            ServerIP = serverIP;
            ServerPort = port;

            IPEndPoint server = new IPEndPoint(ServerIP, ServerPort);
            IPHostEntry clientHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress clientIP = clientHostInfo.AddressList[0];

            foreach (var ipAddress in clientHostInfo.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    clientIP = ipAddress;
                    break;
                }
            }

            ClientSocket = new Socket(clientIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive,  true);

            try
            {
                ClientSocket.BeginConnect(server, null, null);
                if (!ClientSocket.Connected)
                {
                    GetResponse();
                }
                connection = "Succesful connected to server";
            }         
            catch
            {
                connection = "No response from the server";                
            }
            return connection;
        }

        public string[] GetResponse()
        {
            byte[] serverResponse = new byte[8019];
            ClientSocket.Receive(serverResponse);
            return Helper.FromByteArray(serverResponse); 
        }

        public string SendPlay(string selectedItem)
        {
            ClientSocket.Send(Encoding.UTF8.GetBytes(selectedItem));
            int arrsize = 100000;
            byte[] buffer = new byte[arrsize];
            int readBytes = -1;
            SocketError errorCode;
            string outPath = Path.Combine(Path.GetTempPath(), selectedItem);
            if (File.Exists(outPath))
                File.Delete(outPath);
            Stream strm = new FileStream(outPath, FileMode.CreateNew);
            if (!ClientSocket.Connected)
                Send(ServerIP, ServerPort);
            while (readBytes != 0)
            {
                readBytes = ClientSocket.Receive(buffer, 0, arrsize, SocketFlags.None, out errorCode);
                strm.Write(buffer, 0, readBytes);
            }
            strm.Close();
            return outPath;
            
        }

        public string Disconnect()
        {
            string disconnected = "";

            if (ClientSocket.Connected)
            {
            ClientSocket.Send(Encoding.UTF8.GetBytes($"shutdown"));
                ClientSocket.Disconnect(true);
                disconnected = "Disconnected from the server";
            }

            return disconnected;
        }

        
    }

}
