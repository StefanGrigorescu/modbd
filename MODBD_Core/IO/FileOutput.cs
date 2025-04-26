namespace MODBD_Core.IO;

public interface IOutput
{
    void WriteLine(string message);
    void Write(string message);
}


public sealed class FileOutput : IOutput, IDisposable
{
    private readonly string _filePath;
    private readonly StreamWriter _writer;

    public void WriteLine(string message) =>
        _writer.WriteLine(message);

    public void Write(string message) =>
        _writer.Write(message);

    public void Dispose()
    {
        _writer.Dispose();
    }

    public FileOutput(string filePath)
    {
        _filePath = filePath;
        _writer = new(_filePath, true);
    }
}


public sealed class ConsoleOutput : IOutput
{
    public void WriteLine(string message) =>
        Console.WriteLine(message);

    public void Write(string message) =>
        Console.Write(message);
}
