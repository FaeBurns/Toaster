using System;

namespace Toaster.Parsing.TokenValidators;

public static class TokenValidatorSelector
{
    public static TokenValidator GetValidator(TokenId tokenId)
    {
        // labels and instructions are handled separately so use a DiscardedTokenValidator here to do nothing during validation.

        switch (tokenId)
        {
            case TokenId.LABEL:
                return new DiscardedTokenValidator();
            case TokenId.INSTRUCTION:
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