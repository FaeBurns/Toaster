namespace Toaster.Parsing.ValueExtractors;

internal static class ValueExtractorSource
{
    public static readonly ConstantValueExtractor ConstantExtractor = new ConstantValueExtractor();
    public static readonly MultiplePinValueExtractor MultiplePinExtractor = new MultiplePinValueExtractor();
    public static readonly SinglePinValueExtractor SinglePinExtractor = new SinglePinValueExtractor();
    public static readonly StringValueExtractor StringExtractor = new StringValueExtractor();
}