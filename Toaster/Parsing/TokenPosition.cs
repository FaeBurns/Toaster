using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Toaster.Parsing;

/// <summary>
/// A class responsible for defining a token's position in the program
/// </summary>
public readonly struct TokenPosition
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
    public readonly int Line;

    /// <summary>
    /// Gets the column the token's first character occurs on.
    /// </summary>
    public readonly int StartColumn;

    /// <summary>
    /// Gets the column the token's last character occurs on.
    /// </summary>
    public readonly int EndColumn;

    /// <summary>
    /// Gets the length of the token in characters.
    /// </summary>
    public int Length => (EndColumn - StartColumn) + 1;
}