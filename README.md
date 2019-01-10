# AsyncAwaitBestPractices

[![Build Status](https://brminnick.visualstudio.com/AsyncAwaitBestPractices/_apis/build/status/AsyncAwaitBestPractices-.NET%20Desktop-CI)](https://brminnick.visualstudio.com/AsyncAwaitBestPractices/_build/latest?definitionId=5)

Extensions for `System.Threading.Tasks.Task`.

Inspired by [John Thiriet](https://github.com/johnthiriet)'s blog posts: [Removing Async Void](https://johnthiriet.com/removing-async-void/) and [MVVM - Going Async With AsyncCommand](https://johnthiriet.com/mvvm-going-async-with-async-command/).

- AsyncAwaitBestPractices
  - `SafeFireAndForget`
    - An extension method to safely fire-and-forget a `Task`:
  - `WeakEventManager`
    - Avoids memory leaks when events are not unsubscribed
    - Used by `AsyncCommand` and `AsyncCommand<T>`
  - [Usage instructions](#asyncawaitbestpractices-2)
- AsyncAwaitBestPractices.MVVM
  - Allows for `Task` to safely be used asynchronously with `ICommand`:
    - `IAsyncCommand : ICommand`
    - `AsyncCommand : IAsyncCommand`
    - `IAsyncCommand<T> : ICommand`    
    - `AsyncCommand<T> : IAsyncCommand<T>`
  - [Usage instructions](#asyncawaitbestpracticesmvvm-1)

## Setup

###  AsyncAwaitBestPractices

[![NuGet](https://buildstats.info/nuget/AsyncAwaitBestPractices)](https://www.nuget.org/packages/AsyncAwaitBestPractices/)

- Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices/ 
- Add to any project supporting .NET Standard 1.0

### AsyncAwaitBestPractices.MVVM

[![NuGet](https://buildstats.info/nuget/AsyncAwaitBestPractices.MVVM)](https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/)

- Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/  
- Add to any project supporting .NET Standard 2.0

## Why Do I Need This?

Async/await is great *but* there are two problems that are subtle that can easily creep into code:
1) Creating race conditions/concurrent execution (where you code things in the right order but the code executes in a different order than you expect) 
2) Creating methods where the compiler recognizes exceptions but you the coder never see them (making it head-scratchingly annoying to debug *especially* if you accidentally introduced a race condition that you can’t see)  
This library solves both of these problems.

To better understand why this library was created and the problem it solves, it’s important to first understand how the compiler generates code for an async method.  

And by the way, **tl;dr** A non-awaited `Task` doesn't rethrow exceptions so use this library!

## Compiler-Generated Code for Async Method

![Compiler-Generated Code for Async Method](https://i.stack.imgur.com/c9im1.png)

(Source: [Xamarin University: _Using Async and Await_](https://university.xamarin.com/classes/track/csharp#csc350-async))

The compiler transforms an `async` method into an `IAsyncStateMachine` class which allows the .NET Runtime to "remember" what the method has accomplished.

![Move Next](https://i.stack.imgur.com/JsmG1.png)

(Source: [Xamarin University: _Using Async and Await_](https://university.xamarin.com/classes/track/csharp#csc350-async))

The `IAsyncStateMachine` interface implements `MoveNext()`, a method the executes every time the `await` operator is used inside of the `async` method.

`MoveNext()` essentially runs your code until it reaches an `await` statement, then it `return`s while the `await`'d method executes. This is the mechanism that allows the current method to "pause", yielding its thread execution to another thread/Task.

### Try/Catch in `MoveNext()`

Look closely at `MoveNext()`; notice that it is wrapped in a `try/catch` block.

Because the compiler creates `IAsyncStateMachine` for every `async` method and `MoveNext()` is _always_ wrapped in a `try/catch`, every exception thrown inside of an `async` method is caught!

## How to Rethrow an Exception Caught By `MoveNext`

Now we see that the `async` method catches every exception thrown - that is to say, the exception is caught internally by the state machine, *but* you the coder will not see it.  In order for you to see it, you'll need to rethrow the exception to surface it in your debugging.  So the questions is - how do I rethrow the exception? 

There are a few ways to rethrow exceptions that are thrown in an `async` method:

 1. Use the `await` keyword _(Prefered)_
    - e.g. `await DoSomethingAsync()`
 2. Use `.GetAwaiter().GetResult()`
    - e.g. `DoSomethingAsync().GetAwaiter().GetResult()`

The `await` keyword is preferred because `await` allows the `Task` to run asynchronously on a different thread, and it will not lock-up the current thread.

### What About `.Result` or `.Wait()`?

Never, never, never, never, never use `.Result` or `.Wait()`:

1. Both `.Result` and `.Wait()` will lock-up the current thread. If the current thread is the Main Thread (also known as the UI Thread), your UI will freeze until the `Task` has completed.

2.`.Result` or `.Wait()` rethrow your exception as a `System.AggregateException`, which makes it difficult to find the actual exception.
  
# Usage

## AsyncAwaitBestPractices

### `SafeFireAndForget`
An extension method to safely fire-and-forget a `Task`:

```csharp
public static async void SafeFireAndForget(this System.Threading.Tasks.Task task, bool continueOnCapturedContext = true, System.Action<System.Exception> onException = null)
```

```csharp
void HandleButtonTapped(object sender, EventArgs e)
{
    // Allows the async Task method to safely run on a different thread while not awaiting its completion
    // If an exception is thrown, Console.WriteLine
    ExampleAsyncMethod().SafeFireAndForget(onException: ex => Console.WriteLine(ex.ToString()));

    // HandleButtonTapped continues execution here while `ExampleAsyncMethod()` is running on a different thread
    // ...
}

async Task ExampleAsyncMethod()
{
    await Task.Delay(1000);
}
```

### `WeakEventManager`

An event implementation that enables the [garbage collector to collect an object without needing to unsubscribe event handlers](http://paulstovell.com/blog/weakevents), inspired by [Xamarin.Forms.WeakEventManager](https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/WeakEventManager.cs):

```csharp
readonly WeakEventManager _weakEventManager = new WeakEventManager();

public event EventHandler CanExecuteChanged
{
    add => _weakEventManager.AddEventHandler(value);
    remove => _weakEventManager.RemoveEventHandler(value);
}

public void RaiseCanExecuteChanged() => _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
```

### `WeakEventManager<T>`

```csharp
readonly WeakEventManager<string> _errorOcurredEventManager = new WeakEventManager<string>();

public event EventHandler<string> ErrorOcurred
{
    add => _errorOcurredEventManager.AddEventHandler(value);
    remove => _errorOcurredEventManager.RemoveEventHandler(value);
}

public void RaiseErrorOcurred(string message) => _weakEventManager.HandleEvent(this, message, nameof(ErrorOcurred));
```

## AsyncAwaitBestPractices.MVVM

### `AsyncCommand`

Allows for `Task` to safely be used asynchronously with `ICommand`:

- `AsyncCommand<T> : IAsyncCommand<T>`
- `IAsyncCommand<T> : ICommand`
- `AsyncCommand : IAsyncCommand`
- `IAsyncCommand : ICommand`

```csharp
public AsyncCommand(Func<T, Task> execute,
                     Func<object, bool> canExecute = null,
                     Action<Exception> onException = null,
                     bool continueOnCapturedContext = true)   
```

```csharp
public AsyncCommand(Func<Task> execute,
                     Func<object, bool> canExecute = null,
                     Action<Exception> onException = null,
                     bool continueOnCapturedContext = true)
```

```csharp
public class ExampleClass
{
    public ExampleClass()
    {
        ExampleAsyncCommand = new AsyncCommand(ExampleAsyncMethod);
        ExampleAsyncIntCommand = new AsyncCommand<int>(ExampleAsyncMethodWithIntParameter);
        ExampleAsyncExceptionCommand = new AsyncCommand(ExampleAsyncMethodWithException, onException: ex => Console.WriteLine(ex.ToString()));
        ExampleAsyncCommandNotReturningToTheCallingThread = new AsyncCommand(ExampleAsyncMethod, continueOnCapturedContext: false);
    }

    public IAsyncCommand ExampleAsyncCommand { get; }
    public IAsyncCommand<int> ExampleAsyncIntCommand { get; }
    public IAsyncCommand ExampleAsyncExceptionCommand { get; }
    public IAsyncCommand ExampleAsyncCommandNotReturningToTheCallingThread { get; }

    async Task ExampleAsyncMethod()
    {
        await Task.Delay(1000);
    }
  
    async Task ExampleAsyncMethodWithIntParameter(int parameter)
    {
        await Task.Delay(parameter);
    }

    async Task ExampleAsyncMethodWithException()
    {
        await Task.Delay(1000);
        throw new Exception();
    }

    void ExecuteCommands()
    {
        ExampleAsyncCommand.Execute(null);
        ExampleAsyncIntCommand.Execute(1000);
        ExampleAsyncExceptionCommand.Execute(null);
        ExampleAsyncCommandNotReturningToTheCallingThread.Execute(null);
    }
}
```
