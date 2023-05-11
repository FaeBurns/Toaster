using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Jump;

public class ReturnInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        // if optional argument is found
        // set $rv
        if (argumentTokens.Count == 1)
        {
            ushort value = GetTokenValue(context, argumentTokens[0]);
            context.SetRegisterValue("rv", value);
        }
        
        // pop and jump after setting return value otherwise stack registers don't work
        int targetLineIndex = context.PopFrame();
        context.Jump(targetLineIndex);
    }
}