﻿using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Jump;

public class JumpInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        ushort targetLineIndex = GetTokenValue(context, argumentTokens[0]);

        context.Jump(targetLineIndex);
    }
}