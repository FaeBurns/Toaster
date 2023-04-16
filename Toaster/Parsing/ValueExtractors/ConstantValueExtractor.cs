using System;

namespace Toaster.Parsing.ValueExtractors;

public class ConstantValueExtractor : TokenValueExtractor<ushort>
{
    public override ushort ExtractValue(Token token)
    {
        Verify(token, TokenId.BINARY | TokenId.HEX | TokenId.INTEGER);

        string tokenContent = token.RegexResult.Groups[1].Value;

        switch (token.Id)
        {
            case TokenId.BINARY:
                return Convert.ToUInt16(tokenContent.Replace("_", ""), 2);
            case TokenId.HEX:
                return Convert.ToUInt16(tokenContent, 16);
            case TokenId.INTEGER:
                return ushort.Parse(tokenContent);
        }

        // should never occur
        throw new Exception();
    }
}