using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Client.client
{
    class Client
    {
        static void Main()
        {
            var client = new TcpClient("127.0.0.1", 5432);
            var stream = client.GetStream();

            Console.Write("Введите действие (GET/PUT/DELETE/EXIT): ");
            string? action = Console.ReadLine();
            string? fileName = null;
            string? fileContent = null;


            switch (action)
            {
                case "PUT":
                    Console.Write("Введите имя файла:");
                    fileName = Console.ReadLine();
                    Console.WriteLine("Введите содержимое файла:");
                    fileContent = Console.ReadLine();
                    break;
                case "DELETE":
                    Console.Write("Введите имя файла:");
                    fileName = Console.ReadLine();
                    break;
                case "GET":
                    Console.Write("Введите имя файла:");
                    fileName = Console.ReadLine();
                    break;
                case "EXIT":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неправильный запрос");
                    Environment.Exit(0);
                    break;
            }
            
            var request = $"{action} {fileName} {fileContent}";
            byte[] data = System.Text.Encoding.ASCII.GetBytes(request);

            // Отправка запроса на сервер
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Запрос отправлен");

            // Получение ответа от сервера
            data = new Byte[256];
            var responseData = String.Empty;
            var bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            // Обработка ответа
            if (responseData == "200")
            {
                Console.WriteLine("Ответ сервера: The response says that the file was created!");
            }
            else if (responseData == "403")
            {
                Console.WriteLine("Ответ сервера: The response says that creating the file was forbidden!");
            }
            else
            {
                Console.WriteLine("Неожиданный ответ сервера: " + responseData);
            }

            stream.Close();
            client.Close();
        }
    }
}

