using System;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Parsing.TokenReaders;

public class MultiPinTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        try
        {
            PinRange range = ValueExtractorSource.MultiplePinExtractor.ExtractValue(token);

            if (range.EndIndex < range.StartIndex)
                Errors.RaiseError("Pin range end index cannot come before start index", token.Position);

            if (range.EndIndex >= context.PinCount)
                Errors.RaiseError($"Pin range ends outside of allowed range (max {context.PinCount})", token.Position);
        }
        catch (OverflowException e)
        {
            Errors.RaiseError(e.Message, token.Position);
        }
    }
}