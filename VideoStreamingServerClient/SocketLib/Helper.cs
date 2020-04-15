using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.IO;

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


        public static void CloneDirectory(string root, string dest)
        {
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                Directory
                    .GetFiles(root)
                    .ToList()
                    .ForEach(file => file.CopyTo(dest));
            }
        }

        private static void CopyTo(this string file, string dest)
        {
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
        }
    }
}
