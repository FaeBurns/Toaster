using System;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Tests;

public static class TestHelpers
{
    public static Token GetSingleToken(string program, bool includeComments = false)
    {
        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(program, includeComments);
        return tokenProgram.Lines[0].Tokens[0];
    }

    public static TokenLine GetSingleLine(string program, bool includeComments = false)
    {
        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(program, includeComments);
        return tokenProgram.Lines[0];
    }

    public static TokenProgram GetProgram(string program, bool includeComments = false)
    {
        return new Parser().Tokenize(program, includeComments);
    }

    public static ExecutionConfig GetGenericConfig()
    {
        return new ExecutionConfig()
        {
            NamedRegisters =
            {
                "named0",
                "named1",
                "named2",
                "named3",
            },
            PinCount = 16,
            MaxStackDepth = 5,
            StackRegisterCount = 4,
            BasicRegisterCount = 4,
        };
    }
}