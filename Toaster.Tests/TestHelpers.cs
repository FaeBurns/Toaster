using Toaster.Parsing;

namespace Toaster.Tests;

public static class TestHelpers
{
    public static Token GetSingleToken(string program)
    {
        Parser parser = new Parser();
        TokenProgram tokenProgram = parser.Tokenize(program);
        return tokenProgram.Lines[0].Tokens[0];
    }
}