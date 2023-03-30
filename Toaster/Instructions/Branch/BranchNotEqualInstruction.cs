using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Branch;

public class BranchNotEqualInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        ushort targetLine = GetTokenValue(context, argumentTokens[0]);
        ushort valueA = GetTokenValue(context, argumentTokens[1]);
        ushort valueB = GetTokenValue(context, argumentTokens[2]);

        if (valueA != valueB)
            context.Jump(targetLine);
    }
}