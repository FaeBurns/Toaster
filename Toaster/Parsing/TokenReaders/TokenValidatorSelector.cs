using System;

namespace Toaster.Parsing.TokenReaders;

public static class TokenValidatorSelector
{
    public static TokenValidator GetReader(TokenId tokenId)
    {
        switch (tokenId)
        {
            case TokenId.LABEL:
                return new LabelTokenValidator();
            case TokenId.INSTRUCTION:
                // instructions are handled differently
                // as they are defined by the tokens that follow them
                return new DiscardedTokenValidator();
            case TokenId.REGISTER:
                return new RegisterTokenValidator();
            case TokenId.LABEL_ARG:
                return new LabelArgumentTokenValidator();
            case TokenId.PIN_RANGE:
            case TokenId.PIN_RANGE_LENGTH:
                return new MultiPinTokenValidator();
            case TokenId.PIN:
                return new SinglePinTokenValidator();
            case TokenId.BINARY:
            case TokenId.HEX:
            case TokenId.INTEGER:
                return new ConstantTokenValidator();
            case TokenId.WHITESPACE:
            case TokenId.COMMENT:
                return new DiscardedTokenValidator();
            default:
                throw new ArgumentOutOfRangeException(nameof(tokenId), tokenId, null);
        }
    }
}