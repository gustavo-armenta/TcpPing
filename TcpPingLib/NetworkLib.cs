using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpPingLib
{
    public class NetworkLib
    {
        public bool TryTcpPing(string endpoint, int port, out int ms)
        {
            Stopwatch stopwatch = new Stopwatch();
            bool success = false;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Blocking = true;
                stopwatch.Restart();
                IAsyncResult result = socket.BeginConnect(endpoint, port, null, null);
                success = result.AsyncWaitHandle.WaitOne(1000, true);
                stopwatch.Stop();
                socket.Close();
            }

            if (success)
            {
                ms = (int)Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);
            }
            else
            {
                ms = 0;
            }

            return success;
        }

        public void DNSLookup(StringBuilder sb, string hostname)
        {
            IPHostEntry host = null;
            sb.AppendLine(string.Format("  DNSLookup('{0}')", hostname));
            try
            {
                host = Dns.GetHostEntry(hostname);
            }
            catch (Exception ex)
            {
                sb.AppendLine(string.Format("  {0}:{1}", ex.GetType().FullName, ex.Message));
                return;
            }

            sb.AppendLine(string.Format("  HostName: {0}", host.HostName));
            sb.AppendLine("  IP Addresses:");
            foreach (IPAddress address in host.AddressList)
            {
                sb.AppendLine(string.Format("    {0}", address.ToString()));
            }
        }
    }
}
