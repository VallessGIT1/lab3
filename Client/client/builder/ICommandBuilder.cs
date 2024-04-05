namespace Client.client.builder;

internal interface ICommandBuilder
{
    string GetLabel();
    void Run();
}