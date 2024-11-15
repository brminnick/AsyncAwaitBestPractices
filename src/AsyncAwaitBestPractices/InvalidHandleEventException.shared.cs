using System;
using System.Reflection;

namespace AsyncAwaitBestPractices;

/// <summary>
/// Represents errors that occur during WeakEventManager.HandleEvent execution.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="T:AsyncAwaitBestPractices.InvalidHandleEventException"/> class.
/// </remarks>
/// <param name="message">Message.</param>
/// <param name="targetParameterCountException">Target parameter count exception.</param>
public class InvalidHandleEventException(string message, TargetParameterCountException targetParameterCountException) : Exception(message, targetParameterCountException)
{
}