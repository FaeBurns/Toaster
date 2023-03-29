using System;

namespace Toaster.Parsing.ValueExtractors;

public class SinglePinValueExtractor : TokenValueExtractor<int>
{
    public override int ExtractValue(Token token)
    {
        Verify(token, TokenId.PIN);

        return Int32.Parse(token.RegexResult.Groups[1].Value);
    }
}