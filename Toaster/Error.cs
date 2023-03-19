namespace Toaster;

public class Error
{
    public string Message { get; }

    public int Line { get; }

    public int Column { get; }

    public ErrorLevel Level { get; }

    public Error(string message, int line, int column, ErrorLevel level)
    {
        Message = message;

        Line = line;
        Column = column;

        Level = level;
    }

    public override string ToString()
    {
        return $"(Line: {Line} | Column: {Column} | ErrorLevel: {Level} | Message: {Message})";
    }
}

public enum ErrorLevel
{
    INFO = 0,
    WARNING = 1,
    ERROR = 2,
}