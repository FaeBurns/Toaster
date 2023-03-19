namespace Toaster
{
    /// <summary>
    /// A class representing a token found in a program.
    /// </summary>
    /// <typeparam name="T">The enum that represents what token has been found.</typeparam>
    public class Token<T>
        where T : struct
    {
        /// <summary>
        /// Gets information about the position of the token.
        /// </summary>
        public TokenPosition Position { get; }

        /// <summary>
        /// Gets what type of token this is.
        /// </summary>
        public T TokenType { get; }

        /// <summary>
        /// Gets the string value that makes up this token.
        /// </summary>
        public string Value { get; }
    }
}