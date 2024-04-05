using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Server.server.executor;

namespace Server.server;

public static class Server
{
    private const int Port = 9999;
    private const int MaxThreads = 20;
    
    public const string AbsoluteDataPath = "/Users/v.nigmatullin/RiderProjects/Lab3/Server/server/data";
    private const string FileMapPath = "/Users/v.nigmatullin/RiderProjects/Lab3/Server/server/filemap.json";
    
    private static WaitHandle[]? _waitHandles;
    private static Socket? _listener;

    private static readonly List<ICommandExecutor> CommandExecutors =
    [
        new DeleteCommandExecutor(),
        new ExitCommandExecutor(),
        new GetCommandExecutor(),
        new PutCommandExecutor()
    ];

    private static async Task Main()
    {
        _waitHandles = new WaitHandle[MaxThreads];
        for (var i = 0; i < MaxThreads; ++i)
            _waitHandles[i] = new AutoResetEvent(true);

        _listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(IPAddress.Any, Port));
        _listener.Listen(25);

        while (true)
        {
            Console.WriteLine("Ожидание подключений...");
            
            var sock = await _listener.AcceptAsync();
            var index = WaitHandle.WaitAny(_waitHandles);
            
            var clientIpAddress = ((IPEndPoint)sock.RemoteEndPoint).Address.ToString();
            Console.WriteLine($"Подключен клиент с IP-адресом: {clientIpAddress} (id={index})");

            var context = new SocketClient
            {
                ThreadHandle = (AutoResetEvent)_waitHandles[index],
                Socket = sock
            };

            ThreadPool.QueueUserWorkItem(ProcessSocketConnection, context);
        }
    }

    private static void ProcessSocketConnection(object? threadState)
    {
        var client = (SocketClient)threadState!;
        var socket = client.Socket;

        var command = Read(socket).Split(":#:");

        var label = command[0];
        var args = command[1..];

        ExecuteCommand(client, label, args);
        client.Close();
    }

    private static void ExecuteCommand(SocketClient client, string label, string[] args)
    {
        var executor = CommandExecutors.Find(executor => label == executor.GetLabel());
        executor?.Execute(client, args);
    }

    private static string Read(Socket handler)
    {
        var builder = new StringBuilder();
        var data = new byte[256];

        do
        {
            var bytes = handler.Receive(data, SocketFlags.None);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (handler.Available > 0);

        return builder.ToString();
    }

    public static void Send(Socket handler, string response)
    {
        var data = Encoding.Unicode.GetBytes(response);
        handler.Send(new ArraySegment<byte>(data), SocketFlags.None);
    }

    public static string? FindFileById(string id)
    {
        var fileMap = LoadFileMap();
        return fileMap.FileIds.ContainsKey(id) ? fileMap.FileIds[id] : null;
    }

    public static string GenerateFileId(string fileName)
    {
        var fileId = Guid.NewGuid().ToString();
        var fileMap = LoadFileMap();
        fileMap.FileIds[fileId] = fileName;
        SaveFileMap(fileMap);
        return fileId;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private static FileMap LoadFileMap()
    {
        if (!File.Exists(FileMapPath)) return new FileMap();
        var json = File.ReadAllText(FileMapPath);
        return JsonSerializer.Deserialize<FileMap>(json)!;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private static void SaveFileMap(FileMap fileMap)
    {
        var json = JsonSerializer.Serialize(fileMap);
        File.WriteAllText(FileMapPath, json);
    }

    public static void DeleteFileId(string fileName)
    {
        var fileMap = LoadFileMap();
        var fileId = fileMap.FileIds.Keys.FirstOrDefault(id => fileMap.FileIds[id] == fileName);
        if (fileId == null) return;

        fileMap.FileIds.Remove(fileId);
        SaveFileMap(fileMap);
    }
}

internal struct SocketClient
{
    public AutoResetEvent ThreadHandle;
    public Socket Socket;

    public void Close()
    {
        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
        Socket.Dispose();
        ThreadHandle.Set();
    }
}

internal interface ICommandExecutor
{
    string GetLabel();
    void Execute(SocketClient client, string[] args);
}

public class FileMap
{
    public Dictionary<string, string> FileIds { get; init; } = new();
}