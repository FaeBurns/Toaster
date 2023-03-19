namespace Toaster;

public class ParsingError
{
    public string Message { get; }

    public ParsingErrorLevel ErrorLevel { get; }

    public ParsingError(string message, ParsingErrorLevel errorLevel)
    {
        Message = message;
        ErrorLevel = errorLevel;
    }
}

public enum ParsingErrorLevel
{
    ERROR,
    WARNING,
    INFO,
}