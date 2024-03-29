﻿using System.Collections.Generic;
using System.Linq;

namespace Toaster.Parsing;

/// <summary>
/// A class that contains all tokens and related information for a program
/// </summary>
public class TokenProgram
{
    public TokenProgram(IEnumerable<TokenLine> lines, string[] fullProgramLines, int lastInstructionIndex)
    {
        FullProgramLines = fullProgramLines;
        LastInstructionIndex = lastInstructionIndex;

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

    /// <summary>
    /// Gets the full program as an array of its lines.
    /// </summary>
    public string[] FullProgramLines { get; }

    /// <summary>
    /// Gets the LineIndex of the last instruction line.
    /// </summary>
    public int LastInstructionIndex { get; }
}