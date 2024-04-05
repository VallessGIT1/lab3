namespace Client2.client.builder;

internal interface ICommandBuilder
{
    string GetLabel();
    void Run();
}