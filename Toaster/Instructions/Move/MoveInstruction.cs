using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Move;

public class MoveInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        string targetRegister = StringExtractor.ExtractValue(argumentTokens[0]);

        ushort value = GetTokenValue(context, argumentTokens[1]);

        context.SetRegisterValue(targetRegister, value);
    }
}