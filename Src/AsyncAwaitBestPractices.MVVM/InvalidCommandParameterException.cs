using System;

namespace AsyncAwaitBestPractices.MVVM
{
    /// <summary>
    /// Represents errors that occur during IAsyncCommand execution.
    /// </summary>
    public class InvalidCommandParameterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="excpectedType">Excpected parameter type for AsyncCommand.Execute.</param>
        /// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
        /// <param name="innerException">Inner Exception</param>
        public InvalidCommandParameterException(Type excpectedType, Type actualType, Exception innerException) : base(CreateErrorMessage(excpectedType, actualType), innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="excpectedType">Excpected parameter type for AsyncCommand.Execute.</param>
        /// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
        public InvalidCommandParameterException(Type excpectedType, Type actualType) : base(CreateErrorMessage(excpectedType, actualType))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="excpectedType">Excpected parameter type for AsyncCommand.Execute.</param>
        /// <param name="innerException">Inner Exception</param>
        public InvalidCommandParameterException(Type excpectedType, Exception innerException) : base(CreateErrorMessage(excpectedType), innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="excpectedType">Excpected parameter type for AsyncCommand.Execute.</param>
        public InvalidCommandParameterException(Type excpectedType) : base(CreateErrorMessage(excpectedType))
        {

        }

        static string CreateErrorMessage(Type excpectedType) => $"Invalid type for parameter. Expected Type {excpectedType}";

        static string CreateErrorMessage(Type excpectedType, Type actualType) => $"Invalid type for parameter. Expected Type {excpectedType}, but received Type {actualType}";
    }
}
