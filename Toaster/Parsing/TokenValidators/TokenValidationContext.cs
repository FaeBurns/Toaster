using System.Collections.Generic;

namespace Toaster.Parsing.TokenValidators;

public class TokenValidationContext
{
    public TokenValidationContext()
    {
        PinCount = 0;
        Registers = new HashSet<string>();
    }

    public TokenValidationContext(IEnumerable<string> registers, int pinCount)
    {
        PinCount = pinCount;
        Registers = new HashSet<string>(registers);
    }

    public HashSet<string> Labels { get; } = new HashSet<string>();

    public HashSet<string> Registers { get; }

    public int PinCount { get; set; }
}