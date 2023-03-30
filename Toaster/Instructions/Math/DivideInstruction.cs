using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Math;

public class DivideInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        // select valueA from arguments
        // select valueB from arguments if it's there, otherwise choose acc
        ushort valueA = GetTokenValue(context, argumentTokens[0]);
        ushort valueB = argumentTokens.Count == 2 ? GetTokenValue(context, argumentTokens[1]) : context.GetRegisterValue("acc");

        ushort result = (ushort)(valueA / valueB);

        context.SetRegisterValue("acc", result);
    }
}