using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Test;

public class TestLessThanInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        ushort valueA = GetTokenValue(context, argumentTokens[0]);
        ushort valueB = GetTokenValue(context, argumentTokens[1]);

        ushort result = (ushort)(valueA < valueB ? 1 : 0);

        context.SetRegisterValue("t", result);
    }
}