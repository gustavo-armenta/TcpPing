using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TcpPingLib
{
    public class Runner
    {
        public List<Endpoint> Endpoints { get; private set; }

        public void LoadEndpoints()
        {
            Endpoints = new List<Endpoint>();
            foreach (string line in File.ReadAllLines("endpoints.txt"))
            {
                string value = line.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                string[] parts = value.Split(':');
                if (parts.Length == 1)
                {
                    throw new ArgumentException($"Port number is required: {value}");
                }

                Endpoint endpoint = new Endpoint();
                endpoint.Host = parts[0];
                int port = 0;
                if (!int.TryParse(parts[1], out port))
                {
                    throw new ArgumentException($"Port is not a number: {value}");
                }

                endpoint.Port = port;
                Endpoints.Add(endpoint);
            }
        }

        public string RunIt(int page, int itemsPerPage)
        {
            Regex regex = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
            StringBuilder sb = new StringBuilder();
            NetworkLib lib = new NetworkLib();
            for (int i = (page - 1) * itemsPerPage; i < page * itemsPerPage && i < Endpoints.Count; i++)
            {
                Endpoint endpoint = Endpoints[i];
                int ms = 0;
                bool success = lib.TryTcpPing(endpoint.Host, endpoint.Port, out ms);
                if (success)
                {
                    sb.AppendLine($"{endpoint.Host}:{endpoint.Port} connected in {ms} ms");
                }
                else
                {
                    sb.AppendLine($"{endpoint.Host}:{endpoint.Port} failed");
                }


                if (!success && !regex.IsMatch(endpoint.Host))
                {
                    lib.DNSLookup(sb, endpoint.Host);
                }
            }

            return sb.ToString();
        }
    }
}
