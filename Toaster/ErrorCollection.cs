using System.Collections.Generic;

namespace Toaster;

public class ErrorCollection
{
    private List<Error> _errors = new List<Error>();

    public IReadOnlyList<Error> Errors => _errors;

    public ErrorLevel HighestErrorLevel { get; private set; }

    public void Raise(string message, int line, int column, ErrorLevel errorLevel)
    {
        Raise(new Error(message, line, column, errorLevel));
    }

    public void Raise(Error error)
    {
        _errors.Add(error);

        if (error.Level > HighestErrorLevel)
            HighestErrorLevel = error.Level;
    }

    public void RaiseError(string message, int line, int column)
    {
        Raise(message, line, column, ErrorLevel.ERROR);
    }

    public void RaiseWarning(string message, int line, int column)
    {
        Raise(message, line, column, ErrorLevel.WARNING);
    }

    public void RaiseInfo(string message, int line, int column)
    {
        Raise(message, line, column, ErrorLevel.INFO);
    }
}