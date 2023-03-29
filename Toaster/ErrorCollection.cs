using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Toaster.Parsing;

namespace Toaster;

[ExcludeFromCodeCoverage]
public class ErrorCollection
{
    private readonly List<Error> _errors = new List<Error>();

    public IReadOnlyList<Error> Errors => _errors;

    public bool IsEmpty => _errors.Count == 0;

    public bool IsOk => HighestErrorLevel != ErrorLevel.ERROR;

    public ErrorLevel HighestErrorLevel { get; private set; } = ErrorLevel.INFO;

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (_errors.Count > 1)
            stringBuilder.AppendFormat("Count: {0} | Highest Level: {1}", _errors.Count, HighestErrorLevel);

        foreach (Error error in _errors)
        {
            stringBuilder.AppendLine(error.ToString());
        }

        return stringBuilder.ToString();
    }

    public void Raise(string message, int line, int startColumn, int endColumn, ErrorLevel errorLevel)
    {
        Raise(new Error(message, line, startColumn, endColumn, errorLevel));
    }

    public void Raise(string message, TokenPosition position, ErrorLevel errorLevel)
    {
        Raise(new Error(message, position, errorLevel));
    }

    public void Raise(Error error)
    {
        _errors.Add(error);

        if (error.Level > HighestErrorLevel)
            HighestErrorLevel = error.Level;
    }

    public void RaiseError(string message, int line, int startColumn, int endColumn)
    {
        Raise(message, line, startColumn, endColumn, ErrorLevel.ERROR);
    }

    public void RaiseWarning(string message, int line, int startColumn, int endColumn)
    {
        Raise(message, line, startColumn, endColumn, ErrorLevel.WARNING);
    }

    public void RaiseInfo(string message, int line, int startColumn, int endColumn)
    {
        Raise(message, line, startColumn, endColumn, ErrorLevel.INFO);
    }

    public void RaiseError(string message, TokenPosition position)
    {
        Raise(message, position, ErrorLevel.ERROR);
    }

    public void RaiseWarning(string message, TokenPosition position)
    {
        Raise(message, position, ErrorLevel.WARNING);
    }

    public void RaiseInfo(string message, TokenPosition position)
    {
        Raise(message, position, ErrorLevel.INFO);
    }
}