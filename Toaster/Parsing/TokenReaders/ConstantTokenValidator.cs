﻿using System;
using Toaster.Instructions;

namespace Toaster.Parsing.TokenReaders;

public class ConstantTokenValidator : TokenValidator
{
    public override void Validate(Token token, TokenValidationContext context)
    {
        try
        {
            _ = ValueExtractorSource.ConstantExtractor.ExtractValue(token);
        }
        catch (OverflowException e)
        {
            string maxValue = token.Id switch
            {
                TokenId.INTEGER => UInt16.MaxValue.ToString(),
                TokenId.HEX => "FFFF",
                TokenId.BINARY => "1111111111111111",
                _ => throw new ArgumentOutOfRangeException(),
            };

            Errors.RaiseError($"Value is higher than maximum ({maxValue})", token.Position);
        }
    }
}