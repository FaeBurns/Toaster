using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toaster.Definition;
using Toaster.Instructions;
using Toaster.Parsing;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Execution;

public class LineExecutor
{
    private readonly IExecutionContext _context;

    public LineExecutor(IExecutionContext context)
    {
        _context = context;
    }

    public ErrorCollection ErrorCollection { get; } = new ErrorCollection();

    private Instruction GetInstruction(TokenLine line)
    {
        // exit if not instruction
        if (!line.IsInstruction)
            throw new ArgumentException("TokenLine must be an instruction line", nameof(line));

        string instructionName = ValueExtractorSource.StringExtractor.ExtractValue(line.Tokens[0]);

        IEnumerable<Token> argumentTokens = line.Tokens.Skip(1);

        Instruction instruction = InstructionManager.TryFetchInstructionBySignature(instructionName, argumentTokens.Select(t => t.Id).ToArray());

        // should never have got to this point if no instruction can be found here
        // execution should never have begun
        Debug.Assert(instruction != null);

        return instruction;
    }

    /// <summary>
    /// Executes the line.
    /// </summary>
    /// <param name="line">The line to execute.</param>
    /// <returns>True if execution was okay, False if errors were found.</returns>
    public bool Execute(TokenLine line)
    {
        try
        {
            // get instruction
            Instruction instruction = GetInstruction(line);

            // get all tokens except the first to use as arguments
            Token[] arguments = new Token[line.Tokens.Count - 1];
            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = line.Tokens[i + 1];
            }

            // execute instruction
            instruction.Execute(_context, arguments);
        }
        catch (Exception e)
        {
            ErrorCollection.Raise(e.Message, line.LineIndex, 0, line.FullLine.Length - 1, ErrorLevel.ERROR);
        }

        return ErrorCollection.IsEmpty;
    }
}

