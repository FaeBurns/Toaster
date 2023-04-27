using System.Diagnostics.CodeAnalysis;
using Toaster.Parsing;

namespace Toaster;

[ExcludeFromCodeCoverage]
public class Error
{
    public string Message { get; }

    public TokenPosition Position { get; }

    public ErrorLevel Level { get; }

    public Error(string message, TokenPosition position, ErrorLevel level)
    {
        Message = message;
        Position = position;
        Level = level;
    }

    public Error(string message, int line, int startColumn, int endColumn, ErrorLevel level)
    {
        Message = message;

        Position = new TokenPosition(line, startColumn, endColumn);

        Level = level;
    }

    public override string ToString()
    {
        return $"(Line: {Position.Line + 1} | Column: {Position.StartColumn} | Message: {Message})";
    }
}

public enum ErrorLevel
{
    INFO = 0,
    WARNING = 1,
    ERROR = 2,
}