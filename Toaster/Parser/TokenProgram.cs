using System;
using System.Collections.Generic;
using System.Linq;

namespace Toaster;

/// <summary>
/// A class that contains all tokens and related information for a program
/// </summary>
public class TokenProgram
{
    public TokenProgram(IEnumerable<TokenLine> lines)
    {
        // create array copies
        Lines = lines.ToArray();

        // create non empty collection
        InstructionLines = Lines.Where(l => l.IsInstruction).ToArray();
    }

    /// <summary>
    /// Gets the collection of all token lines.
    /// </summary>
    public IReadOnlyList<TokenLine> Lines { get; }

    /// <summary>
    /// Gets the collection of all token lines that start with an instruction.
    /// </summary>
    public IReadOnlyList<TokenLine> InstructionLines { get; }
}