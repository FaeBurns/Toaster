using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Move;

public class NotInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        string targetRegister = StringExtractor.ExtractValue(argumentTokens[0]);

        ushort value = context.GetRegisterValue(targetRegister);

        context.SetRegisterValue(targetRegister, (ushort)~value);
    }
}