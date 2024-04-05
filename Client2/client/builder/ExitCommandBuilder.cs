namespace Client2.client.builder;

internal class ExitCommandBuilder : ICommandBuilder
{
    public string GetLabel() => "EXIT"; 
    
    public void Run()
    {
        Client.Send(GetLabel());
    }
}