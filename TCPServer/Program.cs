using Football;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Football server ready");

            TcpListener listener = new TcpListener(IPAddress.Any, 2121);
            listener.Start();

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("New Client");

                Task.Run(() => HandleClient(socket));
            }
        }

            public static List<FootballPlayer> playerList = new List<FootballPlayer>
            {
                new FootballPlayer("Mads", 100, 5),
                new FootballPlayer("Peter", 10, 8),
                new FootballPlayer("Mikkel", 200, 9)
            };

        private static void HandleClient(TcpClient socket)
        {
            bool run = true;
            
                NetworkStream ns = socket.GetStream();
                StreamReader reader = new StreamReader(ns);
                StreamWriter writer = new StreamWriter(ns);
            writer.WriteLine("commands\r\nforste linje hentalle og blank paa anden linje\r\nforste linje hent og saa id paa anden linje\r\nforste linje gem og spillerens properties paa anden linje");
            writer.Flush();
            while (run == true)
            {
                string function = reader.ReadLine();
                string objekt = reader.ReadLine();

                if (function.ToLower() == "hent")
                {
                    int tjek = 0;
                    int id = -1;
                    int.TryParse(objekt, out id);
                    foreach (var player in playerList)
                    {
                        if (player.Id == id)
                        {
                            writer.WriteLine($"id: {player.Id}, name: {player.Name}, shirtnumber: {player.ShirtNumber}, price: {player.Price}");
                            writer.WriteLine("Player returned");
                            writer.Flush();
                            tjek++;
                        }
                    }
                    if (tjek == 0)
                    {
                        writer.WriteLine("player not found");
                    }
                }
                else if (function.ToLower() == "hentalle")
                {
                    if (playerList.Count == 0)
                    {
                        writer.WriteLine("Der er ingen spillere i listen");
                        writer.Flush();
                    }
                    else
                    {
                        foreach (var player in playerList)
                        {
                            writer.WriteLine($"id: {player.Id}, name: {player.Name}, shirtnumber: {player.ShirtNumber}, price: {player.Price}");
                            writer.Flush();
                        }
                        writer.WriteLine("Player(s) returned");
                        writer.Flush();
                    }
                    
                }
                else if (function.ToLower() == "gem")
                {
                    FootballPlayer spiller = JsonSerializer.Deserialize<FootballPlayer>(objekt);
                    playerList.Add(spiller);
                    writer.WriteLine("person saved :)");
                    writer.Flush();
                }
                else if (function.ToLower() == "close")
                {
                    run = false;
                }
                else
                {
                    writer.WriteLine("din hat");
                    writer.Flush();
                }
            }
            socket.Close();
        }
    }
}

