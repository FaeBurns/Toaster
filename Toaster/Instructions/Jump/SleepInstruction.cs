using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Jump;

public class SleepInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        ushort sleepLength = GetTokenValue(context, argumentTokens[0]);
        context.Sleep(sleepLength);
    }
}