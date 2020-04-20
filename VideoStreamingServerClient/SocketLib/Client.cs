﻿using System;
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

            try
            {
                ClientSocket.BeginConnect(server, null, null);                
                GetFiles();

                connection = "Succesful connected to server";               
            }
            catch
            {
                connection = "No response from the server";                
            }
            return connection;
        }

        public string[] GetFiles()
        {
            string files = GetResponse();

            string[] arrFiles = Directory.GetFiles(files);
            return arrFiles;
        }

        private string GetResponse()
        {
            string response;
            byte[] serverResponse = new byte[8019];
            int messageLength;

            messageLength = ClientSocket.Receive(serverResponse);
            response = Encoding.ASCII.GetString(serverResponse, 0, messageLength).ToUpper().Trim();

            return response;
        }

        public void SendPlay(string selectedItem)
        {
            ClientSocket.Send(Encoding.UTF8.GetBytes(selectedItem));
        }

        public string Disconnect()
        {
            string disconnected;

            ClientSocket.Send(Encoding.UTF8.GetBytes($"shutdown"));
            ClientSocket.Disconnect(true);
            disconnected = "Disconnected from the server";

            return disconnected;
        }

        public bool CheckIP(string ip)
        {
            try
            {
                if (ip == null || ip.Length == 0)
                {
                    return false;
                }

                string[] parts = ip.Split(new[] { "." }, StringSplitOptions.None);
                if (parts.Length != 4)
                {
                    return false;
                }

                foreach (string s in parts)
                {
                    int i = int.Parse(s);
                    if ((i < 0) || (i > 255))
                    {
                        return false;
                    }
                }
                if (ip.EndsWith("."))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}