using System;

namespace Toaster
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal sealed class TokenRuleAttribute : Attribute
    {
        /// <summary>
        /// Gets or Sets if this rule should be discarded and not returned from tokenization.
        /// </summary>
        public bool DiscardSelf { get; set; }

        /// <summary>
        /// Gets or Sets if this token defines the start of a comment. If so, any proceeding tokens will be ignored.
        /// </summary>
        public bool IsComment { get; set; }

        public string Regex { get; }

        public TokenRuleAttribute(string regex)
        {
            Regex = regex;
        }
    }
}