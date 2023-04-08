using System;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Parsing.TokenValidators;

public class SinglePinTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        try
        {
            int pinIndex = ValueExtractorSource.SinglePinExtractor.ExtractValue(token);
            if (pinIndex >= context.PinCount)
                ErrorCollection.RaiseError($"Pin exceeds allowed range (max {context.PinCount})", token.Position);
        }
        catch (OverflowException e)
        {
            ErrorCollection.RaiseError(e.Message, token.Position);
        }
    }
}