using System.Collections.Generic;
using Toaster.Parsing;
using Toaster.Parsing.ValueExtractors;

namespace Toaster.Instructions;

public abstract class Instruction
{
    protected ConstantValueExtractor ConstantExtractor => ValueExtractorSource.ConstantExtractor;
    protected MultiplePinValueExtractor MultiplePinExtractor => ValueExtractorSource.MultiplePinExtractor;
    protected SinglePinValueExtractor SinglePinExtractor = ValueExtractorSource.SinglePinExtractor;
    protected StringValueExtractor StringExtractor = ValueExtractorSource.StringExtractor;

    public abstract void Execute(LinkedList<Token> argumentTokens);
}

internal static class ValueExtractorSource
{
    public static readonly ConstantValueExtractor ConstantExtractor = new ConstantValueExtractor();
    public static readonly MultiplePinValueExtractor MultiplePinExtractor = new MultiplePinValueExtractor();
    public static readonly SinglePinValueExtractor SinglePinExtractor = new SinglePinValueExtractor();
    public static readonly StringValueExtractor StringExtractor = new StringValueExtractor();
}