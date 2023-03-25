using System.Text.RegularExpressions;

namespace Toaster.Parsing
{
    internal class TokenRule
    {
        private readonly Regex _solver;

        public TokenRule(string regexString, TokenId resultingId, bool isDiscarded, bool isComment, bool mustBeFirst)
        {
            _solver = new Regex(regexString);

            ResultingId = resultingId;
            IsDiscarded = isDiscarded;
            IsComment = isComment;
            MustBeFirst = mustBeFirst;
        }

        public TokenId ResultingId { get; }

        public bool IsDiscarded { get; }

        public bool IsComment { get; }

        public bool MustBeFirst { get; }

        public Match TryMatch(string section)
        {
            return _solver.Match(section);
        }
    }
}