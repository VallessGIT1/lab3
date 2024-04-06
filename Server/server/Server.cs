﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyServer
{
    public static class Server
    {
        static void Main()
        {
            TcpListener server = null;
            try
            {
                Console.WriteLine("Server started!");

                // Установка сервера
                var port = 8888;
                var localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);
                server.Start();

                var dataPath = Path.Combine("C:\\Users\\Мой повелитель\\RiderProjects\\lab33\\Server\\server\\data");

                while (true)
                {
                    Console.WriteLine("Waiting for a client to connect...");
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected!");
                    var stream = client.GetStream();
                    

                    var data = new byte[256];
                    var bytesRead = stream.Read(data, 0, data.Length);
                    var request = Encoding.ASCII.GetString(data, 0, bytesRead);

                    if (request.StartsWith("PUT"))
                    {
                        string[] requestData = request.Split(' ');
                        string fileName = requestData[1];
                        string fileContent = requestData[2];
                        string filePath = Path.Combine(dataPath, fileName);
                        Console.WriteLine("\n" + fileName + "\n");

                        if (File.Exists(filePath))
                        {
                            byte[] response = Encoding.ASCII.GetBytes("403");
                            stream.Write(response, 0, response.Length);
                        }
                        else
                        {
                            File.WriteAllText(filePath, fileContent);
                            byte[] response = Encoding.ASCII.GetBytes("200");
                            stream.Write(response, 0, response.Length);
                        }
                    }
                    else if (request.StartsWith("GET"))
                    {
                        string[] requestData = request.Split(' ');
                        string fileName = requestData[1];
                        string filePath = Path.Combine(dataPath, fileName);

                        if (File.Exists(filePath))
                        {
                            string fileContent = File.ReadAllText(filePath);
                            byte[] response = Encoding.ASCII.GetBytes($"200 {fileContent}");
                            stream.Write(response, 0, response.Length);
                        }
                        else
                        {
                            byte[] response = Encoding.ASCII.GetBytes("404");
                            stream.Write(response, 0, response.Length);
                        }
                    }
                    else if (request.StartsWith("DELETE"))
                    {
                        string[] requestData = request.Split(' ');
                        string fileName = requestData[1];
                        string filePath = Path.Combine(dataPath, fileName);

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            byte[] response = Encoding.ASCII.GetBytes("200");
                            stream.Write(response, 0, response.Length);
                        }
                        else
                        {
                            byte[] response = Encoding.ASCII.GetBytes("404");
                            stream.Write(response, 0, response.Length);
                        }
                    }
                    else if (request.StartsWith("EXIT"))
                    {
                        client.Close();
                        Environment.Exit(0);
                    }

                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}