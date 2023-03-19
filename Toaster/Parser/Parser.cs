using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toaster;

public class Parser
{
    private static readonly TokenRule[] _tokenRules;
    private static readonly string[] _lineSeparators = new string[3] { "\r\n", "\r", "\n", };

    static Parser()
    {
        _tokenRules = GetTokenRules();
    }

    /// <summary>
    /// Tokenizes the provided program string
    /// </summary>
    /// <param name="program">The program to tokenize.</param>
    /// <returns>The tokenized program.</returns>
    public TokenProgram Toast(string program)
    {
        // split lines without removing empty lines
        string[] lines = program.Split(_lineSeparators, StringSplitOptions.None);

        // create target array
        TokenLine[] tokenLines = new TokenLine[lines.Length];

        // offsetIndex is used to define the index of the line in an instruction-only context
        int offsetIndex = 0;

        // parse each line
        for (int i = 0; i < lines.Length; i++)
        {
            tokenLines[i] = ParseLine(lines[i], i, offsetIndex);

            // if the newly parsed line is an instruction
            // increment offsetIndex
            if (tokenLines[i].IsInstruction)
                offsetIndex++;
        }

        // construct result and return
        return new TokenProgram(tokenLines);
    }

    private TokenLine ParseLine(string line, int lineIndex, int offsetIndex)
    {
        string searchingSection = line;

        int startPosition = 0;

        List<Token> tokens = new List<Token>();

        while (searchingSection.Length > 0)
        {
            TokenRule matchingRule = null;
            Match validMatchResult = null;

            foreach (TokenRule rule in _tokenRules)
            {
                Match matchResult = rule.TryMatch(searchingSection);

                // if the match was successful and was at the start
                if (matchResult.Success && matchResult.Index == 0)
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
                // set search area to continue after current match
                searchingSection = searchingSection.Substring(validMatchResult.Length);

                // skip if rule says to discard
                if (!matchingRule.IsDiscarded)
                {
                    // get position of token in program
                    TokenPosition position = new TokenPosition(lineIndex, startPosition, startPosition + validMatchResult.Length);

                    // create token and add to list
                    Token token = new Token(validMatchResult.Value, matchingRule!.ResultingType, position, validMatchResult);
                    tokens.Add(token);
                }
            }
            // if no match was found
            else
            {

            }
        }

        // create result line and return
        return new TokenLine(line, lineIndex, offsetIndex, tokens);
    }

    private static TokenRule[] GetTokenRules()
    {
        List<TokenRule> rules = new List<TokenRule>();

        // get all enum values
        TokenType[] orderedTokenTypes = ((TokenType[])Enum.GetValues(typeof(TokenType))).OrderByDescending(t => t).ToArray();

        // loop through all values and get information from attribute
        foreach (TokenType tokenType in orderedTokenTypes)
        {
            TokenRuleAttribute attribute = GetEnumAttribute<TokenRuleAttribute>(tokenType);

            // ignore if no attribute found
            if (attribute != null)
                rules.Add(new TokenRule(attribute.Regex, tokenType , attribute.DiscardSelf, attribute.IsComment));
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
