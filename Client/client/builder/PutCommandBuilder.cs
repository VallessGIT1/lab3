namespace Client.client.builder;

internal class PutCommandBuilder : ICommandBuilder
{
    public string GetLabel() => "PUT"; 
    
    public void Run()
    {
        Console.Write("Введите имя локального файла: ");
        var localFileName = Console.ReadLine()!;
        if (!Client.ValidateFileName(localFileName)) return;

        if (localFileName.Contains(' ') || localFileName.Contains("   "))
        {
            Console.WriteLine("Ошибка: Имя файла не должно содержать пробелы и табуляции!");
            return;
        }

        var filePath = $"{Client.AbsoluteDataPath}/{localFileName}";
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Ошибка: Файл не найден!");
            return;
        }
        
        Console.Write("Введите имя файла на сервере: ");
        var fileName = Console.ReadLine()!;
        if (!Client.ValidateFileName(fileName)) return;

        if (fileName.Contains(' ') || fileName.Contains("   "))
        {
            Console.WriteLine("Ошибка: Имя файла не должно содержать пробелы и табуляции!");
            return;
        }

        var bytes = File.ReadAllBytes(filePath);
        var fileContent = Convert.ToBase64String(bytes);

        Client.Send($"{GetLabel()}:#:{fileName}:#:{fileContent}");
        var response = Client.Read();
        
        var code = response[..3];
        var fileId = response.Length > 3 ? response[4..] : null;

        Console.WriteLine($"Статус запроса: {code}");

        var description = code switch
        {
            "200" => $"The response says that the file was created! (fileId={fileId})",
            "403" => "The response says that creating the file was forbidden!",
            _ => "Failed"
        };

        Console.WriteLine($"Описание: {description}");
    }
}