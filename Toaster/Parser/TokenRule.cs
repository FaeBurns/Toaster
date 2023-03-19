using System.Text.RegularExpressions;

namespace Toaster
{
    internal class TokenRule
    {
        private readonly Regex _solver;

        public TokenRule(string regexString, TokenType resultingType, bool isDiscarded, bool isComment)
        {
            _solver = new Regex(regexString);

            ResultingType = resultingType;
            IsDiscarded = isDiscarded;
            IsComment = isComment;
        }

        public TokenType ResultingType { get; }

        public bool IsDiscarded { get; }

        public bool IsComment { get; }

        public Match TryMatch(string section)
        {
            return _solver.Match(section);
        }
    }
}