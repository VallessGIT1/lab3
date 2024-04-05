namespace Client.client.builder;

internal class DeleteCommandBuilder : ICommandBuilder
{
    public string GetLabel() => "DELETE"; 
    
    public void Run()
    {
        Console.Write("Выберите тип выборки (ID/NAME): ");
        var selectType = Console.ReadLine();
        string? value;
        
        switch (selectType)
        {
            case "ID":
                Console.Write("Введите айди файла: ");
                value = Console.ReadLine();
                break;
            
            case "NAME":
                Console.Write("Введите имя файла: ");
                var fileName = Console.ReadLine();
                if (!Client.ValidateFileName(fileName)) return;
                value = fileName;
                break;
            
            default:
                Console.WriteLine("Ошибка: Тип выборки не распознан!");
                return;
        }
        
        Client.Send($"{GetLabel()}:#:{selectType}:#:{value}");
        var response = Client.Read();

        Console.WriteLine($"Статус запроса: {response}");

        var description = response switch
        {
            "200" => "The response says that the file was successfully deleted!",
            "404" => "The response says that the file was not found!",
            _ => "Failed"
        };

        Console.WriteLine($"Ответ: {description}");
    }
}