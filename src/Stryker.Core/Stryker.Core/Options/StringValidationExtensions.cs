using System.Linq;

namespace Stryker.Core.Options
{
    public static class StringValidationExtensions
    {
        /// <summary>
        /// Check that input contains at least one digit or number
        /// Useful to check that an input value does not contain only whitespace or special characters (newlines)
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true if input does not contain at least one letter or number</returns>
        public static bool IsEmptyInput(this string input)
        {
            return input.Any(char.IsLetterOrDigit);
        }

        /// <summary>
        /// Check that input contains at least one digit or number and is not null
        /// Useful to check that an input value does not contain only whitespace or special characters (newlines)
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true if input is null or input does not contain at least one letter or number</returns>
        public static bool IsNullOrEmptyInput(this string input)
        {
            return input is null || input.IsEmptyInput();
        }
    }
}
