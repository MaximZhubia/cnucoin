using CNUCoin.Common.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CNUCoin.Client
{
    class Program
    {
        // адрес и порт сервера, к которому будем подключаться
        private const int Port = 8005; // порт сервера
        private const string Address = "127.0.0.1"; // адрес сервера
        private const string UsersDirectory = "Users";

        static void Main(string[] args)
        {
            Thread.Sleep(4000);

            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);

                Console.WriteLine("[HELP] List of commands: gu, ct, mine, , exit");

                while (true)
                {
                    Console.Write("[:] ");
                    string command = Console.ReadLine();

                    if (command == "exit")
                    {
                        byte[] data = Encoding.Unicode.GetBytes("cmd_" + command);
                        socket.Send(data);

                        break;
                    }
                    else if (command == "gu")
                    {
                        byte[] data = Encoding.Unicode.GetBytes("cmd_" + command);
                        socket.Send(data);

                        StringBuilder response = new StringBuilder();

                        do
                        {
                            byte[] bytes = new byte[256];
                            int bytesCount = socket.Receive(data, data.Length, 0);
                            response.Append(Encoding.Unicode.GetString(data, 0, bytesCount));
                        }
                        while (socket.Available > 0);

                        object[] userData = JsonConvert.DeserializeObject<object[]>(response.ToString());
                        User user = new User
                        {
                            Id = (int)(long)userData[0],
                            CoinId = userData[1].ToString(),
                            Sign = userData[2].ToString(),
                            PrivateKey = userData[3].ToString()
                        };
                        string fileContent = JsonConvert.SerializeObject(user);

                        if (!Directory.Exists(UsersDirectory))
                        {
                            Directory.CreateDirectory(UsersDirectory);
                        }

                        File.WriteAllText(Path.Combine(UsersDirectory, user.Id.ToString()), fileContent);
                        Console.WriteLine("[RESPONSE] User was created. Check " + UsersDirectory + " directory.");
                    }
                    else if (command == "ct" || command == "mine")
                    {
                        Console.Write("[:] Please enter User Id: ");

                        string filePath = string.Empty;

                        if (int.TryParse(Console.ReadLine(), out int userId))
                        {
                            filePath = Path.Combine(UsersDirectory, userId.ToString());
                        }   
                        else
                        {
                            Console.WriteLine("[ERROR] Incorrect User Id format");
                            continue;
                        }

                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine("[ERROR] User does not exist");
                            continue;
                        }

                        if (command == "ct")
                        {
                            byte[] data = Encoding.Unicode.GetBytes("cmd_ct:" + File.ReadAllText(filePath));
                            socket.Send(data);

                            StringBuilder response = new StringBuilder();

                            do
                            {
                                byte[] bytes = new byte[256];
                                int bytesCount = socket.Receive(data, data.Length, 0);
                                response.Append(Encoding.Unicode.GetString(data, 0, bytesCount));
                            }
                            while (socket.Available > 0);

                            Console.WriteLine("[RESPONSE] Number of created transactions - " + response.ToString());
                        }
                        else if (command == "mine")
                        {
                            byte[] data = Encoding.Unicode.GetBytes("cmd_mine:" + File.ReadAllText(filePath));

                            while (true)
                            {
                                socket.Send(data);

                                StringBuilder response = new StringBuilder();

                                do
                                {
                                    byte[] bytes = new byte[256];
                                    int bytesCount = socket.Receive(bytes, bytes.Length, 0);
                                    response.Append(Encoding.Unicode.GetString(bytes, 0, bytesCount));
                                }
                                while (socket.Available > 0);

                                if (response.ToString() == "Transaction table contains no elements")
                                {
                                    Console.WriteLine("[RESPONSE] " + response.ToString());
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("[RESPONSE] Mining time - " + response.ToString() + " seconds");
                                }
                            }
                        }
                    }
                }

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}
