using System.Linq;

namespace Stryker.Core.Options
{
    public static class StringValidationExtensions
    {
        /// <summary>
        /// Check that input contains at least one digit or number and is not null
        /// Useful to check that an input value does not contain only whitespace or special characters (newlines)
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true if input is null or input does not contain at least one letter or number</returns>
        public static bool IsNullOrEmptyInput(this string input)
        {
            var isNull = input is null;
            var hasLetterOrDigit = input?.Any(char.IsLetterOrDigit) ?? false;
            return isNull | !hasLetterOrDigit;
        }
    }
}
