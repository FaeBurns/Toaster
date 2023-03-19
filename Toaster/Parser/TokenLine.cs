using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Toaster;

public class TokenLine
{
    public TokenLine(string fullLine, int lineIndex, int offsetLineIndex, IEnumerable<Token> tokens)
    {
        FullLine = fullLine;
        LineIndex = lineIndex;
        OffsetLineIndex = offsetLineIndex;

        Tokens = tokens.ToArray();
    }

    /// <summary>
    /// Gets the full string the token line was constructed from.
    /// </summary>
    public string FullLine { get; }

    /// <summary>
    /// Gets the index of the line.
    /// </summary>
    public int LineIndex { get; }

    /// <summary>
    /// Gets the index of the line not including non-instruction lines.
    /// </summary>
    public int OffsetLineIndex { get; }

    /// <summary>
    /// Gets the amount of tokens in the line.
    /// </summary>
    public int TokenCount => Tokens.Count;

    /// <summary>
    /// Gets a value indicating whether the line is empty or not.
    /// </summary>
    public bool IsEmpty => TokenCount == 0;

    /// <summary>
    /// Gets a value indicating whether this line starts with an instruction.
    /// </summary>
    public bool IsInstruction => !IsEmpty && Tokens[0].Type == TokenType.INSTRUCTION;

    /// <summary>
    /// Gets a collection of all tokens in the line.
    /// </summary>
    public IReadOnlyList<Token> Tokens { get; }
}