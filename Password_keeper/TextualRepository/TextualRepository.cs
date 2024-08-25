using System.Text.Json;

public class StringsTextualRepository : ITextualRepository
{
    private static readonly string Separator = Environment.NewLine;

    public List<string> Read(string filePath)
    {
        var fileContents = File.ReadAllText(filePath);
        return fileContents.Split(Separator).ToList();
    }

    public void Write(string filePath, List<string> strings)
    {
        File.WriteAllText(filePath, string.Join(Separator, strings));
    }
}

public class StringsJsonRepository : ITextualRepository
{
    public List<string> Read(string filePath)
    {
        var fileContents = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<string>>(fileContents); ;
    }

    public void Write(string filePath, List<string> strings)
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(strings));
    }
}