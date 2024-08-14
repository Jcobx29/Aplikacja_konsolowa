public interface ITextualRepository
{
    public List<string> Read(string filePath);
    public void Write(string filePath, List<string> strings);

}