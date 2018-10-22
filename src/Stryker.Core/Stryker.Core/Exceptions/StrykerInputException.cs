using System;

namespace Stryker.Core.Exceptions
{
    /// <summary>
    /// Represents errors which are related to user input.
    /// Everything that the user can fix themselves should be shown to them
    ///  using this kind of exception.
    /// </summary>
    public class StrykerInputException : Exception
    {
        public StrykerInputException(string message)
            : base(message)
        {
        }
        
    }
}