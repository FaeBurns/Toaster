using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Move;

public class MoveShiftRightInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        string targetRegister = StringExtractor.ExtractValue(argumentTokens[0]);

        ushort value = GetTokenValue(context, argumentTokens[1]);
        ushort shiftAmount = GetTokenValue(context, argumentTokens[2]);

        // >>> always performs logical shift
        value = (ushort)(value >>> shiftAmount);

        context.SetRegisterValue(targetRegister, value);
    }
}