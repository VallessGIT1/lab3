namespace Client.client.builder;

internal class GetCommandBuilder : ICommandBuilder
{
    public string GetLabel() => "GET";

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
                var name = Console.ReadLine();
                if (!Client.ValidateFileName(name)) return;
                value = name;
                break;

            default:
                Console.WriteLine("Ошибка: Тип выборки не распознан!");
                return;
        }

        Client.Send($"{GetLabel()}:#:{selectType}:#:{value}");
        var response = Client.Read().Split(":#:");

        var code = response[0];
        var fileName = response.Length > 1 ? response[1] : null;
        var content = response.Length > 2 ? response[2] : null;

        Console.WriteLine($"Статус запроса: {code}");

        var description = code switch
        {
            "200" => "The file successfully downloaded!",
            "404" => "The response says that the file was not found!",
            _ => "Failed"
        };

        if (code == "200")
        {
            var localPath = $"{Client.AbsoluteDataPath}/{fileName}";
            var fileContent = Convert.FromBase64String(content!);

            File.WriteAllBytes(localPath, fileContent);
        }

        Console.WriteLine($"Ответ: {description}");
    }
}