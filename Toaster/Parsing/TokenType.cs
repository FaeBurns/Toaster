namespace Toaster.Parsing;

/// <summary>
/// Defines the types of tokens that can be parsed
/// </summary>
public enum TokenType
{
    /// <summary>
    /// <para>A label defines a line that the program can jump to.</para>
    /// <para>:name</para>
    /// </summary>
    [TokenRule(@":([a-zA-Z]+[a-zA-Z0-9_]+)", MustBeFirst = true)]
    LABEL = 0,

    /// <summary>
    /// <para>An instruction describes an instruction name. Instruction lines can only occur at the start of a line.</para>
    /// <para>name</para>
    /// </summary>
    [TokenRule(@"([a-zA-Z]+)", MustBeFirst = true)]
    INSTRUCTION = 1,

    /// <summary>
    /// <para>A register describes a 16-bit data source</para>
    /// <para>$reg, $reg0</para>
    /// </summary>
    [TokenRule(@"\$([a-zA-Z0-9]+)")]
    REGISTER = 2,

    /// <summary>
    /// <para>An instruction argument (usually label name).</para>
    /// <para>name</para>
    /// </summary>
    [TokenRule(@"([a-zA-Z]+[a-zA-Z0-9_]+)")]
    NAME = 3,

    /// <summary>
    /// <para>A pin range defines sequential range of pins that data can be sent/received from.</para>
    /// <para>.p0..p8</para>
    /// </summary>
    [TokenRule(@"\.p([0-9]+)\.\.p([0-9]+)")]
    PIN_RANGE = 10,

    /// <summary>
    /// <para>A pin range length is another way of defining a pin range <seealso cref="PIN_RANGE"/> in a way that may be more readable.</para>
    /// <para>.p0:8</para>
    /// </summary>
    [TokenRule(@"\.p([0-9]+):([0-9]+)")]
    PIN_RANGE_LENGTH = 11,

    /// <summary>
    /// <para>A pin defines a specific pin that data can be sent/received from.</para>
    /// <para>Pin must be ordered after other pin rules as those will also pick it up</para>
    /// <para>.p0, .p1</para>
    /// </summary>
    [TokenRule(@"\.p([0-9]+)")]
    PIN = 19,

    /// <summary>
    /// <para>A binary number. Maximum of 16 bits, minimum of 1. That single bit can be a 0.</para>
    /// <para>0b100, 0b0</para>
    /// </summary>
    [TokenRule(@"0b([01]+)")]
    BINARY = 20,

    /// <summary>
    /// <para>A hexadecimal number.</para>
    /// <para>0x0, 0xFFFF, 0xffff</para>
    /// </summary>
    [TokenRule(@"0x([0-9a-fA-F]+)")]
    HEX = 21,

    /// <summary>
    /// <para>An integer number.</para>
    /// <para>1234567890, 0987654321</para>
    /// </summary>
    [TokenRule(@"([0-9]+)")]
    INTEGER = 22,

    /// <summary>
    /// Whitespace, required in order to separate tokens.
    /// </summary>
    [TokenRule(@"[ \t]+", DiscardSelf = true)]
    WHITESPACE = 100,

    /// <summary>
    /// Comment, causes any proceeding characters to be collected and sent as one token.
    /// </summary>
    [TokenRule(@"([;#].*)", IsComment = true)]
    COMMENT = 200,
}