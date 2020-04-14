using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace SocketLib
{
    public static class Helper
    {
        public static List<string> GetActiveIP4s()
        {
            List<string> activeIps = new List<string>();
            activeIps.Add("127.0.0.1");
            var host = Dns.GetHostEntry(Dns.GetHostName());

            activeIps.AddRange(from ip in host.AddressList
                               where ip.AddressFamily == AddressFamily.InterNetwork
                               select ip.ToString());
            return activeIps;
        }
    }
}
