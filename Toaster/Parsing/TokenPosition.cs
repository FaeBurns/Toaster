using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Toaster.Parsing;

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
    public int Length => (EndColumn - StartColumn) + 1;

    [ExcludeFromCodeCoverage]
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TokenPosition)obj);
    }

    [ExcludeFromCodeCoverage]
    protected bool Equals(TokenPosition other)
    {
        return Line == other.Line
               && StartColumn == other.StartColumn
               && EndColumn == other.EndColumn;
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Line;
            hashCode = (hashCode * 397) ^ StartColumn;
            hashCode = (hashCode * 397) ^ EndColumn;
            return hashCode;
        }
    }
}