﻿using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Jump;

public class ReturnIfFalseInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        // exit if $t holds anything other than 0
        ushort tValue = context.GetRegisterValue("t");
        if (tValue != 0)
            return;

        // if optional argument is found
        // set $rv
        if (argumentTokens.Count == 2)
        {
            ushort value = GetTokenValue(context, argumentTokens[1]);
            context.SetRegisterValue("rv", value);
        }
        
        int targetLineIndex = context.PopFrame();
        context.Jump(targetLineIndex);
    }
}