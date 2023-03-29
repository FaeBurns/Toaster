using System;

namespace Toaster.Parsing.ValueExtractors;

public class MultiplePinValueExtractor : TokenValueExtractor<PinRange>
{
    public override PinRange ExtractValue(Token token)
    {
        Verify(token, TokenId.PIN | TokenId.PIN_RANGE | TokenId.PIN_RANGE_LENGTH);

        if (token.Id == TokenId.PIN)
        {
            int index = Int32.Parse(token.RegexResult.Groups[1].Value);
            return new PinRange(index, index);
        }

        // both PIN_RANGE and PIN_RANGE_LENGTH have the same group indexes
        int group1Value = Int32.Parse(token.RegexResult.Groups[1].Value);
        int group2Value = Int32.Parse(token.RegexResult.Groups[2].Value);

        // RANGE: start and end index
        // LENGTH: start index and length, end index must be calculated
        return token.Id == TokenId.PIN_RANGE ?
            new PinRange(group1Value, group2Value) :
            new PinRange(group1Value, (group1Value + group2Value) - 1, group2Value);
    }
}