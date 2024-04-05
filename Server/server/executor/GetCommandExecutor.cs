namespace Server.server.executor;

internal class GetCommandExecutor : ICommandExecutor
{
    public string GetLabel() => "GET";

    public void Execute(SocketClient client, string[] args)
    {
        var selectType = args[0];
        var value = args[1];

        string? fileName;

        switch (selectType)
        {
            case "ID":
                fileName = Server.FindFileById(value);
                break;

            case "NAME":
                fileName = value;
                break;

            default:
                Server.Send(client.Socket, "403");
                return;
        }

        if (fileName == null)
        {
            Server.Send(client.Socket, "404");
            return;
        }
        
        var filePath = $"{Server.AbsoluteDataPath}/{fileName}";
        
        if (!File.Exists(filePath))
        {
            Server.Send(client.Socket, "404");
            return;
        }
        
        var bytes = File.ReadAllBytes(filePath);
        var fileContent = Convert.ToBase64String(bytes);
        
        Server.Send(client.Socket, $"200:#:{fileName}:#:{fileContent}");
    }
}