namespace Toaster;

/// <summary>
/// A class responsible for defining a token's position in the program
/// </summary>
public class TokenPosition
{
    public TokenPosition(int line, int startColumn, int endColumn)
    {
        Line = line;
        StartColumn = startColumn;
        EndColumn = endColumn;
    }

    /// <summary>
    /// Gets the line the token occurs on.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column the token's first character occurs on.
    /// </summary>
    public int StartColumn { get; }

    /// <summary>
    /// Gets the column the token's last character occurs on.
    /// </summary>
    public int EndColumn { get; }

    /// <summary>
    /// Gets the length of the token in characters.
    /// </summary>
    public int Length => EndColumn - StartColumn;
}