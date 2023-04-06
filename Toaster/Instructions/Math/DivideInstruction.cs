using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Math;

public class DivideInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        // case 1: multiple arg - divide arg0 by arg1
        // case 2: single arg - divide acc by arg0
        ushort valueA = argumentTokens.Count == 2 ? GetTokenValue(context, argumentTokens[0]) : context.GetRegisterValue("acc");
        ushort valueB = argumentTokens.Count == 2 ? GetTokenValue(context, argumentTokens[1]) : GetTokenValue(context, argumentTokens[0]);

        // divide with truncate - floors result
        ushort result = (ushort)(valueA / valueB);

        context.SetRegisterValue("acc", result);
    }
}