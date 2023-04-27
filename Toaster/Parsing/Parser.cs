using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Toaster.Parsing;

public class Parser
{
    private static readonly TokenRule[] _tokenRules;
    private static readonly string[] _lineSeparators = new string[] { "\r\n", "\r", "\n", };

    static Parser()
    {
        _tokenRules = GetTokenRules();
    }

    public ErrorCollection Errors { get; } = new ErrorCollection();

    /// <summary>
    /// Tokenizes the provided program string
    /// </summary>
    /// <param name="program">The program to tokenize.</param>
    /// <param name="includeComments">Should comment tokens be included in the TokenProgram output.</param>
    /// <returns>The tokenized program.</returns>
    public TokenProgram Tokenize(string program, bool includeComments = false)
    {
        // split lines without removing empty lines
        string[] lines = program.Split(_lineSeparators, StringSplitOptions.None);

        // create target array
        TokenLine[] tokenLines = new TokenLine[lines.Length];

        // offsetIndex is used to define the index of the line in an instruction-only context
        int offsetIndex = 0;

        int lastInstructionIndex = -1;

        // parse each line
        for (int i = 0; i < lines.Length; i++)
        {
            tokenLines[i] = ParseLine(lines[i], i, offsetIndex, includeComments);

            // if the newly parsed line is an instruction
            // increment offsetIndex
            if (tokenLines[i].IsInstruction)
            {
                offsetIndex++;
                lastInstructionIndex = i;
            }
        }

        // construct result and return
        return new TokenProgram(tokenLines, lines, lastInstructionIndex);
    }

    private TokenLine ParseLine(string line, int lineIndex, int offsetIndex, bool includeComments)
    {
        string searchingSection = line;

        int startPosition = 0;

        List<Token> tokens = new List<Token>();

        // loop while there is still text to search and a critical error has not occured
        while (searchingSection.Length > 0)
        {
            TokenRule matchingRule = null;
            Match validMatchResult = null;

            foreach (TokenRule rule in _tokenRules)
            {
                Match matchResult = rule.TryMatch(searchingSection);

                // if the match was successful and was at the start
                // and it is not restricted to first only or if it is restricted, there are no recorded tokens before it
                // use tokens.Count instead of startPosition as startPosition increases on whitespace
                if (matchResult.Success && matchResult.Index == 0 &&
                    (!rule.MustBeFirst || (rule.MustBeFirst && tokens.Count == 0)))
                {
                    // set required data
                    matchingRule = rule;
                    validMatchResult = matchResult;

                    // break out of this foreach loop
                    break;
                }
            }

            // if a match was found
            if (matchingRule != null)
            {
                // if this is a comment, and comments are being skipped
                // don't add to tokens list
                // exit out of the loop and return immediately - comments are always the last token on a line
                if (matchingRule.IsComment && !includeComments)
                    break;

                // skip if rule says to discard
                // or if rule is a comment and this is not including comments
                if (!matchingRule.IsDiscarded)
                {
                    // get position of token in program
                    TokenPosition position = new TokenPosition(lineIndex, startPosition, (startPosition + validMatchResult.Length) - 1);

                    // create token and add to list
                    Token token = new Token(validMatchResult.Value, matchingRule!.ResultingId, position, matchingRule.IsComment, validMatchResult);
                    tokens.Add(token);
                }

                // set search area to continue after current match
                searchingSection = searchingSection.Substring(validMatchResult.Length);
                startPosition += validMatchResult.Length;
            }
            // if no match was found
            else
            {
                // raise error and return
                Errors.RaiseError("Invalid token found in line.", lineIndex, startPosition, line.Length - 1);
                return new TokenLine(line, lineIndex, offsetIndex, tokens);
            }
        }

        // create result line and return
        return new TokenLine(line, lineIndex, offsetIndex, tokens);
    }

    private static TokenRule[] GetTokenRules()
    {
        List<TokenRule> rules = new List<TokenRule>();

        // get all enum values
        TokenId[] orderedTokenTypes = ((TokenId[])Enum.GetValues(typeof(TokenId))).OrderBy(t => t).ToArray();

        // loop through all values and get information from attribute
        foreach (TokenId tokenType in orderedTokenTypes)
        {
            TokenRuleAttribute attribute = GetEnumAttribute<TokenRuleAttribute>(tokenType);

            // ignore if no attribute found
            if (attribute != null)
                rules.Add(new TokenRule(attribute.Regex, tokenType , attribute.DiscardSelf, attribute.IsComment, attribute.MustBeFirst));
        }

        return rules.ToArray();
    }

    private static T GetEnumAttribute<T>(Enum enumVal) where T : Attribute
    {
        Type type = enumVal.GetType();
        MemberInfo[] memberInfos = type.GetMember(Enum.GetName(enumVal.GetType(), enumVal)!);
        return memberInfos[0].GetCustomAttribute<T>();
    }
}
