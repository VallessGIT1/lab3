namespace Server.server.executor;

internal class DeleteCommandExecutor : ICommandExecutor
{
    public string GetLabel() => "DELETE";

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
            Console.WriteLine(filePath);
            Server.Send(client.Socket, "404");
            return;
        }

        try
        {
            File.Delete(filePath);
            Server.DeleteFileId(fileName);
            Server.Send(client.Socket, "200");
        }
        catch (Exception)
        {
            Server.Send(client.Socket, "403");
        }
    }
}