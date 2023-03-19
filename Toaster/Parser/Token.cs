using System.Text.RegularExpressions;

namespace Toaster;

/// <summary>
/// A class representing a token found in a program.
/// </summary>
public class Token
{
    public Token(string value, TokenType type, TokenPosition position, Match regexResult)
    {
        Value = value;
        Type = type;
        Position = position;
        RegexResult = regexResult;
    }

    /// <summary>
    /// Gets information about the position of the token.
    /// </summary>
    public TokenPosition Position { get; }

    /// <summary>
    /// Gets what type of token this is.
    /// </summary>
    public TokenType Type { get; }

    /// <summary>
    /// Gets the string value that makes up this token.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the match data that came from the regex find during tokenization.
    /// </summary>
    public Match RegexResult { get; }
}