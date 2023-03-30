using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Math;

public class AddInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        // select valueA from arguments
        // select valueB from arguments if it's there, otherwise choose acc
        ushort valueA = GetTokenValue(context, argumentTokens[0]);
        ushort valueB = argumentTokens.Count == 2 ? GetTokenValue(context, argumentTokens[1]) : context.GetRegisterValue("acc");

        // C# does not allow adding ushort together
        // only allowed values are int, long, IntPtr, float, double
        // no unsigned
        // is automatically casted to next largest
        // have to cast back manually
        ushort result = (ushort)(valueA + valueB);

        context.SetRegisterValue("acc", result);
    }
}