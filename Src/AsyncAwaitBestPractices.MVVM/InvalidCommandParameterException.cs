using System;

namespace AsyncAwaitBestPractices.MVVM
{
    /// <summary>
    /// Represents errors that occur during IAsyncCommand execution.
    /// </summary>
    class InvalidCommandParameterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="excpectedType">Excpected parameter type for AsyncCommand.Execute.</param>
        /// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
        /// <param name="innerException">Inner Exception</param>
        public InvalidCommandParameterException(Type excpectedType, Type actualType, Exception innerException) : this(CreateErrorMessage(excpectedType, actualType), innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="excpectedType">Excpected parameter type for AsyncCommand.Execute.</param>
        /// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
        public InvalidCommandParameterException(Type excpectedType, Type actualType) : this(CreateErrorMessage(excpectedType, actualType))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="message">Exception Message</param>
        /// <param name="innerException">Inner Exception</param>
        public InvalidCommandParameterException(string message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        /// <param name="message">Exception Message</param>
        public InvalidCommandParameterException(string message) : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
        /// </summary>
        public InvalidCommandParameterException()
        {

        }

        static string CreateErrorMessage(Type excpectedType, Type actualType) =>
            $"Invalid type for parameter. Expected Type {excpectedType}, but received Type {actualType}";
    }
}
