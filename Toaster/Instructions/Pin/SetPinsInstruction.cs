using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Pin;

public class SetPinsInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        PinRange range = MultiplePinExtractor.ExtractValue(argumentTokens[0]);
        ushort value = GetTokenValue(context, argumentTokens[1]);

        int startBit = 1 << range.PinCount - 1;

        bool[] values = new bool[range.PinCount];
        for (int i = 0; i < range.PinCount; i++)
        {
            // get if value is set
            // by checking if the ith bit is set
            // use left to right
            // starting from pinCount - 1 as offset
            int valueMask = startBit >>> i;
            bool set = (value & valueMask) != 0;
            values[i] = set;
        }

        context.SetPins(range.StartIndex, values);
    }
}