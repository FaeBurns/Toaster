using System;

namespace Toaster.Parsing.ValueExtractors;

public abstract class TokenValueExtractor<T>
{
    public abstract T ExtractValue(Token token);

    protected void Verify(Token token, TokenId validFlags)
    {
        // throw if none of the valid id flags is set
        if ((token.Id & validFlags) == 0)
            throw new InvalidCastException("Invalid ValueExtractor for provided token id");
    }
}