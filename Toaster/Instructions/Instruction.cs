using System;
using System.Collections.Generic;
using Toaster.Execution;
using Toaster.Parsing;
using Toaster.Parsing.TokenReaders;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Instructions;

public abstract class Instruction
{
    protected ConstantValueExtractor ConstantExtractor => ValueExtractorSource.ConstantExtractor;
    protected MultiplePinValueExtractor MultiplePinExtractor => ValueExtractorSource.MultiplePinExtractor;
    protected SinglePinValueExtractor SinglePinExtractor => ValueExtractorSource.SinglePinExtractor;
    protected StringValueExtractor StringExtractor => ValueExtractorSource.StringExtractor;

    protected ushort GetTokenValue(IExecutionContext context, Token token)
    {
        switch (token.Id)
        {
            case TokenId.REGISTER:
                return context.GetRegisterValue(StringExtractor.ExtractValue(token));
            case TokenId.BINARY:
            case TokenId.HEX:
            case TokenId.INTEGER:
                return ConstantExtractor.ExtractValue(token);
            case TokenId.LABEL:
                string labelString = StringExtractor.ExtractValue(token);
                return context.GetLabelLineIndex(labelString);
        }

        throw new ArgumentException($"Invalid token id. Must be a register, constant, or label, received {token.Id}", nameof(token));
    }

    public abstract void Execute(IExecutionContext context, IReadOnlyList<Token> argumentTokens);
}