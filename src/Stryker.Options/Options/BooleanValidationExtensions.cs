namespace Stryker.Abstractions.Options
{
    public static class BooleanValidationExtensions
    {
        /// <summary>
        /// Check if a nullable boolean is true or not null
        /// </summary>
        /// <param name="input">the boolean</param>
        public static bool IsNotNullAndTrue(this bool? input)
        {
            return input ?? false;
        }
    }
}
