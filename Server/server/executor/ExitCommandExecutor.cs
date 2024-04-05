namespace Server.server.executor;

internal class ExitCommandExecutor : ICommandExecutor
{
    public string GetLabel() => "EXIT";

    public void Execute(SocketClient client, string[] args)
    {
        Environment.Exit(0);
    }
}