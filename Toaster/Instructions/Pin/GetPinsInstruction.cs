using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;

namespace Toaster.Instructions.Pin;

public class GetPinsInstruction : Instruction
{
    public override void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens)
    {
        PinRange range = MultiplePinExtractor.ExtractValue(argumentTokens[1]);
        string targetRegister = StringExtractor.ExtractValue(argumentTokens[0]);

        ushort result = context.GetRegisterValue(targetRegister);

        // create startBit index and zero index
        // use zero so don't have to cast later getting bitValue
        ushort startBit = (ushort)(1 << range.PinCount - 1);
        ushort zero = 0;

        bool[] pinValues = context.GetPins(range.StartIndex, range.PinCount);

        for (int i = 0; i < pinValues.Length; i++)
        {
            // get the start bit value
            ushort bitValue = pinValues[i] ? startBit : zero;

            // or result with bitValue shifted right by i
            result |= (ushort)(bitValue >>> i);
        }

        // set register with result
        context.SetRegisterValue(targetRegister, result);
    }
}