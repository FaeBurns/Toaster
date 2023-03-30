using System;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Parsing.TokenReaders;

public class SinglePinTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        try
        {
            int pinIndex = ValueExtractorSource.SinglePinExtractor.ExtractValue(token);
            if (pinIndex >= context.PinCount)
                Errors.RaiseError($"Pin exceeds allowed range (max {context.PinCount})", token.Position);
        }
        catch (OverflowException e)
        {
            Errors.RaiseError(e.Message, token.Position);
        }
    }
}