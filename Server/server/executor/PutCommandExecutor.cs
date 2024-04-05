namespace Server.server.executor;

internal class PutCommandExecutor : ICommandExecutor
{
    public string GetLabel() => "PUT";

    public void Execute(SocketClient client, string[] args)
    {
        var fileName = args[0];
        var fileContent = args[1];

        if (string.IsNullOrEmpty(fileName) || fileName == "null")
            do
                fileName = Guid.NewGuid().ToString();
            while (File.Exists(fileName));

        var filePath = $"{Server.AbsoluteDataPath}/{fileName}";
        if (File.Exists(filePath))
        {
            Server.Send(client.Socket, "403");
            return;
        }

        var content = Convert.FromBase64String(fileContent);
        
        try
        {
            File.WriteAllBytes(filePath, content);
            var fileId = Server.GenerateFileId(fileName);
            Server.Send(client.Socket, $"200 {fileId}");
        }
        catch (Exception)
        {
            Server.Send(client.Socket, "403");
        }
    }
}