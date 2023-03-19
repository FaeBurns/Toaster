using System;

namespace Toaster.Parsing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal sealed class TokenRuleAttribute : Attribute
    {
        /// <summary>
        /// Gets or Sets a value indicating whether this rule should be discarded and not returned from tokenization.
        /// </summary>
        public bool DiscardSelf { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating whether this this token is a comment.
        /// </summary>
        public bool IsComment { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating whether this token must be the first non-discarded token in the line.
        /// </summary>
        public bool MustBeFirst { get; set; }

        public string Regex { get; }

        public TokenRuleAttribute(string regex)
        {
            Regex = regex;
        }
    }
}