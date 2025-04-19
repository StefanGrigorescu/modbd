namespace MODBD_Analiza;

internal class Output : IDisposable
{
    private const string _filePath = "analiza.txt";
    private readonly StreamWriter _writer = new (_filePath, true);

    public void WriteLine(string message) =>
        Console.Write(message);
        //_writer.WriteLine(message);

    public void Write(string message) =>
        Console.Write(message);
        //_writer.Write(message);

    public void Dispose()
    {
        _writer.Dispose();
    }
}
