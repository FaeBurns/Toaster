using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Pin;

public class ClearPinsInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        PinRange range = MultiplePinExtractor.ExtractValue(argumentTokens[0]);

        // each one will initialize false
        bool[] clearedResult = new bool[range.PinCount];
        context.SetPins(range.StartIndex, clearedResult);
    }
}