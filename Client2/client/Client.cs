using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Client2.client.builder;

namespace Client2.client;

public static partial class Client
{
    public const string AbsoluteDataPath = "/Users/v.nigmatullin/RiderProjects/Lab3/Client/client/data";
    private const int Port = 9999;

    private static Socket? _socket;

    private static readonly List<ICommandBuilder> CommandBuilders =
    [
        new PutCommandBuilder(),
        new GetCommandBuilder(),
        new DeleteCommandBuilder(),
        new ExitCommandBuilder()
    ];

    private static void Main()
    {
        try
        {
            var ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ipPoint);

            Console.Write($"Введите действие ({GetAllLabels()}): ");
            var command = Console.ReadLine();

            BuildAndSendCommand(command);
            _socket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static string GetAllLabels()
    {
        return string.Join("/", CommandBuilders.ConvertAll(executor => executor.GetLabel()));
    }

    private static void BuildAndSendCommand(string? command)
    {
        var executor = CommandBuilders.Find(executor => command == executor.GetLabel());
        if (executor == null)
        {
            Console.WriteLine("Ошибка: Команда не найдена!");
            return;
        }

        executor.Run();
    }

    public static string Read()
    {
        var builder = new StringBuilder();
        var data = new byte[256];

        do
        {
            var bytes = _socket!.Receive(data);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (_socket.Available > 0);

        return builder.ToString();
    }

    public static void Send(string response)
    {
        var data = Encoding.Unicode.GetBytes(response);
        _socket?.Send(data);
    }

    public static bool ValidateFileName(string? fileName)
    {
        if (fileName != null && FileNameRegex().IsMatch(fileName)) return true;
        Console.WriteLine("Ошибка: Невалидное имя файла!");
        return false;
    }

    [GeneratedRegex("^[^~)('!*<>:;,?\"*|/]+$")]
    private static partial Regex FileNameRegex();
}