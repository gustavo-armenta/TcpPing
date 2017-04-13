using System;
using System.Net;
using TcpPingLib;

namespace TcpPing
{
    class Program
    {
        static void Main(string[] args)
        {
            Runner runner = new Runner();
            runner.LoadEndpoints();
            int itemsPerPage = 1;
            for (int page = 1; page <= runner.Endpoints.Count; page++)
            {
                string s = runner.RunIt(page, itemsPerPage);
                Console.Write(s);
            }
        }
    }
}
