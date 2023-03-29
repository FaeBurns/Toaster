namespace Toaster;

public readonly struct PinRange
{
    public readonly int StartIndex;
    public readonly int EndIndex;
    public readonly int PinCount;

    public PinRange(int startIndex, int endIndex, int pinCount)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        PinCount = pinCount;
    }

    public PinRange(int startIndex, int endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;

        PinCount = (endIndex - startIndex) + 1;
    }

    public override string ToString()
    {
        return $"Pin Range ({StartIndex}..{EndIndex}) ({StartIndex}:{PinCount})";
    }
}