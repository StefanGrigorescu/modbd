namespace MODBD_Analiza.IO;

internal class Output : IDisposable
{
    private const string _filePath = "analiza.txt";
    private readonly StreamWriter _writer = new(_filePath, true);

    public void WriteLine(string message) =>
        Console.WriteLine(message);
    //_writer.WriteLine(message);

    public void Write(string message) =>
        Console.Write(message);
    //_writer.Write(message);

    public void Dispose()
    {
        _writer.Dispose();
    }
}
