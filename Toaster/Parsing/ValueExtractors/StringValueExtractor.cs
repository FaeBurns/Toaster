namespace Toaster.Parsing.ValueExtractors;

public class StringValueExtractor : TokenValueExtractor<string>
{
    public override string ExtractValue(Token token)
    {
        const TokenId validTokenIds = TokenId.LABEL | TokenId.INSTRUCTION | TokenId.REGISTER | TokenId.LABEL_ARG;
        Verify(token, validTokenIds);

        return token.RegexResult.Groups[1].Value;
    }
}