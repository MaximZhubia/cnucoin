using CNUCoin.Common.Models;
using CNUCoin.Server.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CNUCoin.Server.Handlers
{
    class SocketHandler
    {
        public string Address { get; private set; }
        public int Port { get; private set; }

        public SocketHandler(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public void Start()
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                Console.WriteLine("[INFO] Waiting for connections...");

                Socket handler = listenSocket.Accept();
                Console.WriteLine("[INFO] Client connected");

                while (true)
                {
                    // получаем сообщение
                    string command = null;

                    do
                    {
                        byte[] data = new byte[4096 * 8]; // буфер для получаемых данных
                        int length = handler.Receive(data);
                        command = Encoding.Unicode.GetString(data, 0, length);
                    }
                    while (handler.Available > 0);

                    Console.WriteLine("[CMD] " + command);

                    if (command == "cmd_exit")
                    {
                        break;
                    }
                    // gu - generate user
                    else if (command == "cmd_gu")
                    {
                        CommandHandler.GenerateUser(out byte[] privateKey, out byte[] signature, out int userId, out string coinId);
                        object[] userData = new object[] { userId, coinId, BitConverter.ToString(signature), BitConverter.ToString(privateKey) };
                        string response = JsonConvert.SerializeObject(userData);
                        byte[] bytes = Encoding.Unicode.GetBytes(response);
                        handler.Send(bytes);
                    }
                    // ct - create transaction
                    else if (command.StartsWith("cmd_ct"))
                    {
                        string requestData = command.Substring(command.IndexOf('{'));
                        User user = JsonConvert.DeserializeObject<User>(requestData);

                        int transactionsCount = 0;

                        for (int i = 0; i < 100; i++)
                        {
                            CommandHandler.CreateTransaction(user.CoinId, new Random().Next(1, 100), user.Sign);
                            transactionsCount++;
                        }

                        byte[] bytes = Encoding.Unicode.GetBytes(transactionsCount.ToString());
                        handler.Send(bytes);
                    }
                    // mine - start mining
                    else if (command.StartsWith("cmd_mine"))
                    {
                        string requestData = command.Substring(command.IndexOf('{'));
                        User user = JsonConvert.DeserializeObject<User>(requestData);

                        try
                        {
                            double totalSeconds = CommandHandler.Mine(user.Id, user.Sign);
                            byte[] bytes = Encoding.Unicode.GetBytes(totalSeconds.ToString());
                            handler.Send(bytes);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "Sequence contains no elements")
                            {
                                byte[] bytes = Encoding.Unicode.GetBytes("Transaction table contains no elements");
                                handler.Send(bytes);
                            }
                        }
                    }
                }

                // закрываем сокет
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
