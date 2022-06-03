using CNUCoin.Server.Handlers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CNUCoin.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();

            SocketHandler socketHandler = new SocketHandler("127.0.0.1", 8005);
            socketHandler.Start();
        }
    }
}
