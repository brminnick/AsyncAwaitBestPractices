using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices
{
    /// <summary>
    /// Extension methods for System.Threading.Tasks.Task
    /// </summary>
    public static class SafeFireAndForgetExtensions
    {
        static Action<Exception> _onException;
        static bool _shouldAlwaysThrowException;

        /// <summary>
        /// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
        /// </summary>
        /// <param name="task">Task.</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        /// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        public static void SafeFireAndForget(this Task task, bool continueOnCapturedContext = false, Action<Exception> onException = null) => HandleSafeFireAndForget(task, continueOnCapturedContext, onException);

        /// <summary>
        /// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
        /// </summary>
        /// <param name="task">Task.</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        /// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        /// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
        public static void SafeFireAndForget<TException>(this Task task, bool continueOnCapturedContext = false, Action<TException> onException = null) where TException : Exception => HandleSafeFireAndForget(task, continueOnCapturedContext, onException);

        /// <summary>
        /// Initialize SafeFireAndForget to always rethrow an exception.
        ///
        /// Warning: When <c>true</c>, there is no way to catch this exception and it will always result in a crash. Recommended only for debugging purposes.
        /// </summary>
        /// <param name="shouldAlwaysThrowException">If set to <c>true</c>, after the exception has been caught and handled, the exception will always be rethrown.</param>
		public static void Initialize(in bool shouldAlwaysThrowException = false) => _shouldAlwaysThrowException = shouldAlwaysThrowException;

        /// <summary>
        /// Set the default actionfor SafeFireAndForget to handle every exception
        /// </summary>
        /// <param name="onException">If an exception is thrown in the Task using SafeFireAndForget, <c>onException</c> will execute</param>
        public static void SetDefaultExceptionHandling(in Action<Exception> onException) => _onException = onException;

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        static async void HandleSafeFireAndForget<TException>(Task task, bool continueOnCapturedContext, Action<TException> onException) where TException : Exception
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (TException ex) when (_onException != null || onException != null)
            {
                _onException?.Invoke(ex);
                onException?.Invoke(ex);

                if (_shouldAlwaysThrowException)
                    throw;
            }
        }
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
    }
}
