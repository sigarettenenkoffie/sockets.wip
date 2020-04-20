using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Text;

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

        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate { }));
        }

        public static string[] GetFileList(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] filenames = new string[files.Length];
            for(int i = 0; i < files.Length; i++)
            {
                filenames[i] = Path.GetFileName(files[i]);
            }
            return filenames;
        }
        public static byte[] ToByteArray(string[] input)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                var rows = input.GetLength(0);
                writer.Write(rows);
                for (int i = 0; i < rows; i++)
                {
                    writer.Write(input[i]);
                }
                writer.Flush();
                return stream.ToArray();
            }
        }

        public static string[] FromByteArray(byte[] input)
        {
            using (var stream = new MemoryStream(input))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                var rows = reader.ReadInt32();
                var result = new string[rows];
                for (int i = 0; i < rows; i++)
                {
                    result[i] = reader.ReadString();
                }
                return result;
            }
        }
    }
}
