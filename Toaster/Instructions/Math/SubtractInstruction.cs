using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Math;

public class SubtractInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        // if there are 2 args, select values from there
        // in first case, A - B. A is second
        // otherwise A is acc, B is arg
        ushort valueA;
        ushort valueB;
        if (argumentTokens.Count == 2)
        {
            valueA = GetTokenValue(context, argumentTokens[1]);
            valueB = GetTokenValue(context, argumentTokens[0]);
        }
        else
        {
            valueA = context.GetRegisterValue("acc");
            valueB = GetTokenValue(context, argumentTokens[0]);
        }

        ushort result = (ushort)(valueA - valueB);

        context.SetRegisterValue("acc", result);
    }
}