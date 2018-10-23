using KingNetwork.Server;
using System;

namespace KingNetwork.TestServer
{
    class Program
    {
        static void Main(string[] _args)
        {
            var server = new KingServer(7171);

            server.Start();

            Console.ReadLine();
        }
    }
}
