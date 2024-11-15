using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices;

/// <summary>
/// Extension methods for System.Threading.Tasks.Task and System.Threading.Tasks.ValueTask
/// </summary> 
public static partial class SafeFireAndForgetExtensions
{
	/// <summary>
	/// Safely execute the ValueTask without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">ValueTask.</param>
	/// <param name="onException">If an exception is thrown in the ValueTask, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	public static void SafeFireAndForget(this ValueTask task, Action<Exception>? onException, bool continueOnCapturedContext = false) => task.SafeFireAndForget(in onException, in continueOnCapturedContext);

	/// <summary>
	/// Safely execute the ValueTask without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">ValueTask.</param>
	/// <param name="onException">If an exception is thrown in the ValueTask, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	/// <typeparam name="T">The return value of the ValueTask.</typeparam>
	public static void SafeFireAndForget<T>(this ValueTask<T> task, Action<Exception>? onException, bool continueOnCapturedContext = false) => task.SafeFireAndForget(in onException, in continueOnCapturedContext);

	/// <summary>
	/// Safely execute the ValueTask without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">ValueTask.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	/// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
	public static void SafeFireAndForget<TException>(this ValueTask task, Action<TException>? onException, bool continueOnCapturedContext = false) where TException : Exception => task.SafeFireAndForget(in onException, in continueOnCapturedContext);

	/// <summary>
	/// Safely execute the ValueTask without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">ValueTask.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	/// <typeparam name="T">The return value of the ValueTask.</typeparam>
	/// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
	public static void SafeFireAndForget<T, TException>(this ValueTask<T> task, Action<TException>? onException, bool continueOnCapturedContext = false) where TException : Exception => task.SafeFireAndForget(in onException, in continueOnCapturedContext);

	/// <summary>
	/// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">Task.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	public static void SafeFireAndForget(this Task task, Action<Exception>? onException, bool continueOnCapturedContext = false) => task.SafeFireAndForget(in onException, in continueOnCapturedContext);

	/// <summary>
	/// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">Task.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	/// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
	public static void SafeFireAndForget<TException>(this Task task, Action<TException>? onException, bool continueOnCapturedContext = false) where TException : Exception => task.SafeFireAndForget(in onException, in continueOnCapturedContext);

#if NET8_0_OR_GREATER
	
	/// <summary>
	/// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">Task.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="configureAwaitOptions">Options to control behavior when awaiting</param>
	public static void SafeFireAndForget(this Task task, ConfigureAwaitOptions configureAwaitOptions, Action<Exception>? onException = null) => task.SafeFireAndForget(in configureAwaitOptions, in onException);

	/// <summary>
	/// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
	/// </summary>
	/// <param name="task">Task.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="configureAwaitOptions">Options to control behavior when awaiting</param>
	/// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
	public static void SafeFireAndForget<TException>(this Task task, ConfigureAwaitOptions configureAwaitOptions, Action<TException>? onException = null) where TException : Exception => task.SafeFireAndForget(in configureAwaitOptions, in onException);
	
#endif

	/// <summary>
	/// Initialize SafeFireAndForget
	///
	/// Warning: When <c>true</c>, there is no way to catch this exception, and it will always result in a crash. Recommended only for debugging purposes.
	/// </summary>
	/// <param name="shouldAlwaysRethrowException">If set to <c>true</c>, after the exception has been caught and handled, the exception will always be rethrown.</param>
	public static void Initialize(bool shouldAlwaysRethrowException) => Initialize(in shouldAlwaysRethrowException);

	/// <summary>
	/// Set the default action for SafeFireAndForget to handle every exception
	/// </summary>
	/// <param name="onException">If an exception is thrown in the Task using SafeFireAndForget, <c>onException</c> will execute</param>
	public static void SetDefaultExceptionHandling(Action<Exception> onException) => SetDefaultExceptionHandling(in onException);
}