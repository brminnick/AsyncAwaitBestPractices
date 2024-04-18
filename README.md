# AsyncAwaitBestPractices

[![Build Status](https://brminnick.visualstudio.com/AsyncAwaitBestPractices/_apis/build/status/brminnick.AsyncAwaitBestPractices?branchName=main)](https://brminnick.visualstudio.com/AsyncAwaitBestPractices/_build/latest?definitionId=8&branchName=main)

Extensions for `System.Threading.Tasks.Task`.

Inspired by [John Thiriet](https://github.com/johnthiriet)'s blog posts: 
- [Removing Async Void](https://johnthiriet.com/removing-async-void/)
- [MVVM - Going Async With AsyncCommand](https://johnthiriet.com/mvvm-going-async-with-async-command/).

###  AsyncAwaitBestPractices

[![NuGet](https://buildstats.info/nuget/AsyncAwaitBestPractices?includePreReleases=true)](https://www.nuget.org/packages/AsyncAwaitBestPractices/)

Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices/ 

- `SafeFireAndForget`
    - An extension method to safely fire-and-forget a `Task` or a `ValueTask`
    - Ensures the `Task` will rethrow an `Exception` if an `Exception` is caught in `IAsyncStateMachine.MoveNext()`
- `WeakEventManager`
    - Avoids memory leaks when events are not unsubscribed
    - Used by `AsyncCommand`, `AsyncCommand<T>`, `AsyncValueCommand`, `AsyncValueCommand<T>`
- [Usage instructions](#asyncawaitbestpractices-3)
  
### AsyncAwaitBestPractices.MVVM

[![NuGet](https://buildstats.info/nuget/AsyncAwaitBestPractices.MVVM?includePreReleases=true)](https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/)

- Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/  

- Allows for `Task` to safely be used asynchronously with `ICommand`:
  - `IAsyncCommand : ICommand`
  - `AsyncCommand : IAsyncCommand`
  - `IAsyncCommand<T> : ICommand`    
  - `AsyncCommand<T> : IAsyncCommand<T>`
  - `IAsyncCommand<TExecute, TCanExecute> : IAsyncCommand<TExecute>`    
  - `AsyncCommand<TExecute, TCanExecute> : IAsyncCommand<TExecute, TCanExecute>`    
  
- Allows for `ValueTask` to safely be used asynchronously with `ICommand`:
  - `IAsyncValueCommand : ICommand`
  - `AsyncValueCommand : IAsyncValueCommand`
  - `IAsyncValueCommand<T> : ICommand`    
  - `AsyncValueCommand<T> : IAsyncValueCommand<T>`
  - `IAsyncValueCommand<TExecute, TCanExecute> : IAsyncValueCommand<TExecute>`    
  - `AsyncValueCommand<TExecute, TCanExecute> : IAsyncValueCommand<TExecute, TCanExecute>`   
- [Usage instructions](#asyncawaitbestpracticesmvvm-2)

## Setup

###  AsyncAwaitBestPractices

- Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices/ 
- Add to any project supporting .NET Standard 1.0

### AsyncAwaitBestPractices.MVVM

- Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/  
- Add to any project supporting .NET Standard 1.0

## Why Do I Need This?

### Podcasts

[No Dogma Podcast](https://nodogmapodcast.bryanhogan.net), Hosted by [Bryan Hogan](https://twitter.com/bryanjhogan)
- [Episode #133 Brandon Minnick, Async Await – Common Mistakes, Part 1](https://nodogmapodcast.bryanhogan.net/133-brandon-minnick-async-await-common-mistakes-part-1/)
- [Episode #134 Brandon Minnick, Async Await – Common Mistakes, Part 2](https://nodogmapodcast.bryanhogan.net/134-brandon-minnick-async-await-common-mistakes-part-2/)

### Video

**NDC London 2024**

[Correcting Common Async Await Mistakes in .NET 8](https://www.youtube.com/watch?v=GQYd6MWKiLI&embeds_referring_euri=https%3A%2F%2Ftwitter.com%2F&source_ve_path=Mjg2NjY&feature=emb_logo)

[![](https://github.com/brminnick/AsyncAwaitBestPractices/assets/13558917/d59803c2-cf28-41b9-ac4f-9dce4a0fadc5)](https://www.youtube.com/watch?v=GQYd6MWKiLI)


### Explaination

Async/await is great *but* there are two subtle problems that can easily creep into code:
1. Creating race conditions/concurrent execution (where you code things in the right order but the code executes in a different order than you expect) 
2. Creating methods where the compiler recognizes exceptions but you the coder never see them (making it head-scratchingly annoying to debug *especially* if you accidentally introduced a race condition that you can’t see).

This library solves both of these problems.

To better understand why this library was created and the problem it solves, it’s important to first understand how the compiler generates code for an async method.  

**tl;dr** A non-awaited `Task` doesn't rethrow exceptions and `AsyncAwaitBestPractices.SafeFireAndForget` ensures it will

## Compiler-Generated Code for Async Method

![Compiler-Generated Code for Async Method](https://i.stack.imgur.com/c9im1.png)

(Source: [Xamarin University: _Using Async and Await_](https://github.com/XamarinUniversity/CSC350))

The compiler transforms an `async` method into an `IAsyncStateMachine` class which allows the .NET Runtime to "remember" what the method has accomplished.

![Move Next](https://i.stack.imgur.com/JsmG1.png)

(Source: [Xamarin University: _Using Async and Await_](https://github.com/XamarinUniversity/CSC350))

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

2. `.Result` or `.Wait()` rethrow your exception as a `System.AggregateException`, which makes it difficult to find the actual exception.
  
# Usage

## AsyncAwaitBestPractices

### `SafeFireAndForget`
An extension method to safely fire-and-forget a `Task`.

`SafeFireAndForget` allows a Task to safely run on a different thread while the calling thread does not wait for its completion.

```csharp
public static async void SafeFireAndForget(this System.Threading.Tasks.Task task, System.Action<System.Exception>? onException = null, bool continueOnCapturedContext = false)
```

```csharp
public static async void SafeFireAndForget(this System.Threading.Tasks.ValueTask task, System.Action<System.Exception>? onException = null, bool continueOnCapturedContext = false)
```

#### On .NET 8.0 (and higher)

.NET 8.0 Introduces [`ConfigureAwaitOptions`](https://learn.microsoft.com/dotnet/api/system.threading.tasks.configureawaitoptions) that allow users to customize the behavior when awaiting:
- `ConfigureAwaitOptions.None`
    - No options specified
- `ConfigureAwaitOptions.SuppressThrowing`
    - Avoids throwing an exception at the completion of awaiting a Task that ends in the Faulted or Canceled state
- `ConfigureAwaitOptions.ContinueOnCapturedContext`
    - Attempts to marshal the continuation back to the original SynchronizationContext or TaskScheduler present on the originating thread at the time of the await
- `ConfigureAwaitOptions.ForceYielding`
    - Forces an await on an already completed Task to behave as if the Task wasn't yet completed, such that the current asynchronous method will be forced to yield its execution

For more information, check out Stephen Cleary's blog post, ["ConfigureAwait in .NET 8"](https://blog.stephencleary.com/2023/11/configureawait-in-net-8.html).

```csharp
public static void SafeFireAndForget(this System.Threading.Tasks.Task task, ConfigureAwaitOptions configureAwaitOptions, Action<Exception>? onException = null)
```

#### Basic Usage - Task

```csharp
void HandleButtonTapped(object sender, EventArgs e)
{
    // Allows the async Task method to safely run on a different thread while the calling thread continues, not awaiting its completion
    // onException: If an Exception is thrown, print it to the Console
    ExampleAsyncMethod().SafeFireAndForget(onException: ex => Console.WriteLine(ex));

    // HandleButtonTapped continues execution here while `ExampleAsyncMethod()` is running on a different thread
    // ...
}

async Task ExampleAsyncMethod()
{
    await Task.Delay(1000);
}
```

> **Note:** `ConfigureAwaitOptions.SuppressThrowing` will always supress exceptions from being rethrown. This means that `onException` will never execute when `ConfigureAwaitOptions.SuppressThrowing` is set.

#### Basic Usage - ValueTask

If you're new to ValueTask, check out this great write-up, [Understanding the Whys, Whats, and Whens of ValueTask
](https://blogs.msdn.microsoft.com/dotnet/2018/11/07/understanding-the-whys-whats-and-whens-of-valuetask?WT.mc_id=mobile-0000-bramin).

```csharp
void HandleButtonTapped(object sender, EventArgs e)
{
    // Allows the async ValueTask method to safely run on a different thread while the calling thread continues, not awaiting its completion
    // onException: If an Exception is thrown, print it to the Console
    ExampleValueTaskMethod().SafeFireAndForget(onException: ex => Console.WriteLine(ex));

    // HandleButtonTapped continues execution here while `ExampleAsyncMethod()` is running on a different thread
    // ...
}

async ValueTask ExampleValueTaskMethod()
{
    var random = new Random();
    if (random.Next(10) > 9)
        await Task.Delay(1000);
}
```

#### Advanced Usage

```csharp
void InitializeSafeFireAndForget()
{
    // Initialize SafeFireAndForget
    // Only use `shouldAlwaysRethrowException: true` when you want `.SafeFireAndForget()` to always rethrow every exception. This is not recommended, because there is no way to catch an Exception rethrown by `SafeFireAndForget()`; `shouldAlwaysRethrowException: true` should **not** be used in Production/Release builds.
    SafeFireAndForgetExtensions.Initialize(shouldAlwaysRethrowException: false);

    // SafeFireAndForget will print every exception to the Console
    SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => Console.WriteLine(ex));
}

void UninitializeSafeFireAndForget()
{
    // Remove default exception handling
    SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
}

void HandleButtonTapped(object sender, EventArgs e)
{
    // Allows the async Task method to safely run on a different thread while not awaiting its completion
    // onException: If a WebException is thrown, print its StatusCode to the Console. **Note**: If a non-WebException is thrown, it will not be handled by `onException`
    // Because we set `SetDefaultExceptionHandling` in `void InitializeSafeFireAndForget()`, the entire exception will also be printed to the Console
    ExampleAsyncMethod().SafeFireAndForget<WebException>(onException: ex =>
    {
        if(ex.Response is HttpWebResponse webResponse)
            Console.WriteLine($"Task Exception\n Status Code: {webResponse.StatusCode}");
    });
    
    ExampleValueTaskMethod().SafeFireAndForget<WebException>(onException: ex =>
    {
        if(ex.Response is HttpWebResponse webResponse)
            Console.WriteLine($"ValueTask Error\n Status Code: {webResponse.StatusCode}");
    });

    // HandleButtonTapped continues execution here while `ExampleAsyncMethod()` and `ExampleValueTaskMethod()` run in the background
}

async Task ExampleAsyncMethod()
{
    await Task.Delay(1000);
    throw new WebException();
}

async ValueTask ExampleValueTaskMethod()
{
    var random = new Random();
    if (random.Next(10) > 9)
        await Task.Delay(1000);
        
    throw new WebException();
}
```

> **Note:** `ConfigureAwaitOptions.SuppressThrowing` will always supress exceptions from being rethrown. This means that `onException` will never execute when `ConfigureAwaitOptions.SuppressThrowing` is set.

### `WeakEventManager`

An event implementation that enables the [garbage collector to collect an object without needing to unsubscribe event handlers](http://paulstovell.com/blog/weakevents).

Inspired by [Xamarin.Forms.WeakEventManager](https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/WeakEventManager.cs).

#### Using `EventHandler`

```csharp
readonly WeakEventManager _canExecuteChangedEventManager = new WeakEventManager();

public event EventHandler CanExecuteChanged
{
    add => _canExecuteChangedEventManager.AddEventHandler(value);
    remove => _canExecuteChangedEventManager.RemoveEventHandler(value);
}

void OnCanExecuteChanged() => _canExecuteChangedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
```

#### Using `Delegate`

```csharp
readonly WeakEventManager _propertyChangedEventManager = new WeakEventManager();

public event PropertyChangedEventHandler PropertyChanged
{
    add => _propertyChangedEventManager.AddEventHandler(value);
    remove => _propertyChangedEventManager.RemoveEventHandler(value);
}

void OnPropertyChanged([CallerMemberName]string propertyName = "") => _propertyChangedEventManager.RaiseEvent(this, new PropertyChangedEventArgs(propertyName), nameof(PropertyChanged));
```

#### Using `Action`

```csharp
readonly WeakEventManager _weakActionEventManager = new WeakEventManager();

public event Action ActionEvent
{
    add => _weakActionEventManager.AddEventHandler(value);
    remove => _weakActionEventManager.RemoveEventHandler(value);
}

void OnActionEvent(string message) => _weakActionEventManager.RaiseEvent(message, nameof(ActionEvent));
```

### `WeakEventManager<T>`
An event implementation that enables the [garbage collector to collect an object without needing to unsubscribe event handlers](http://paulstovell.com/blog/weakevents).

Inspired by [Xamarin.Forms.WeakEventManager](https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/WeakEventManager.cs).

#### Using `EventHandler<T>`

```csharp
readonly WeakEventManager<string> _errorOcurredEventManager = new WeakEventManager<string>();

public event EventHandler<string> ErrorOcurred
{
    add => _errorOcurredEventManager.AddEventHandler(value);
    remove => _errorOcurredEventManager.RemoveEventHandler(value);
}

void OnErrorOcurred(string message) => _errorOcurredEventManager.RaiseEvent(this, message, nameof(ErrorOcurred));
```

#### Using `Action<T>`

```csharp
readonly WeakEventManager<string> _weakActionEventManager = new WeakEventManager<string>();

public event Action<string> ActionEvent
{
    add => _weakActionEventManager.AddEventHandler(value);
    remove => _weakActionEventManager.RemoveEventHandler(value);
}

void OnActionEvent(string message) => _weakActionEventManager.RaiseEvent(message, nameof(ActionEvent));
```

## AsyncAwaitBestPractices.MVVM

### `AsyncCommand`

Allows for `Task` to safely be used asynchronously with `ICommand`:

- `AsyncCommand<TExecute, TCanExecute> : IAsyncCommand<TExecute, TCanExecute>`
- `IAsyncCommand<TExecute, TCanExecute> : IAsyncCommand<TExecute>`
- `AsyncCommand<T> : IAsyncCommand<T>`
- `IAsyncCommand<T> : ICommand`
- `AsyncCommand : IAsyncCommand`
- `IAsyncCommand : ICommand`

```csharp
public AsyncCommand(Func<TExecute, Task> execute,
                     Func<TCanExecute, bool>? canExecute = null,
                     Action<Exception>? onException = null,
                     bool continueOnCapturedContext = false)
```

```csharp
public AsyncCommand(Func<T, Task> execute,
                     Func<object?, bool>? canExecute = null,
                     Action<Exception>? onException = null,
                     bool continueOnCapturedContext = false)
```

```csharp
public AsyncCommand(Func<Task> execute,
                     Func<object?, bool>? canExecute = null,
                     Action<Exception>? onException = null,
                     bool continueOnCapturedContext = false)
```

```csharp
public class ExampleClass
{
    bool _isBusy;

    public ExampleClass()
    {
        ExampleAsyncCommand = new AsyncCommand(ExampleAsyncMethod);
        ExampleAsyncIntCommand = new AsyncCommand<int>(ExampleAsyncMethodWithIntParameter);
        ExampleAsyncIntCommandWithCanExecute = new AsyncCommand<int, int>(ExampleAsyncMethodWithIntParameter, CanExecuteInt);
        ExampleAsyncExceptionCommand = new AsyncCommand(ExampleAsyncMethodWithException, onException: ex => Console.WriteLine(ex.ToString()));
        ExampleAsyncCommandWithCanExecuteChanged = new AsyncCommand(ExampleAsyncMethod, _ => !IsBusy);
        ExampleAsyncCommandReturningToTheCallingThread = new AsyncCommand(ExampleAsyncMethod, continueOnCapturedContext: true);
    }

    public IAsyncCommand ExampleAsyncCommand { get; }
    public IAsyncCommand<int> ExampleAsyncIntCommand { get; }
    public IAsyncCommand<int, int> ExampleAsyncIntCommandWithCanExecute { get; }
    public IAsyncCommand ExampleAsyncExceptionCommand { get; }
    public IAsyncCommand ExampleAsyncCommandWithCanExecuteChanged { get; }
    public IAsyncCommand ExampleAsyncCommandReturningToTheCallingThread { get; }
    
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                ExampleAsyncCommandWithCanExecuteChanged.RaiseCanExecuteChanged();
            }
        }
    }

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

    bool CanExecuteInt(int count)
    {
        if(count > 2)
            return true;
        
        return false;
    }

    void ExecuteCommands()
    {
        _isBusy = true;
    
        try
        {
            ExampleAsyncCommand.Execute(null);
            ExampleAsyncIntCommand.Execute(1000);
            ExampleAsyncExceptionCommand.Execute(null);
            ExampleAsyncCommandReturningToTheCallingThread.Execute(null);
            
            if(ExampleAsyncCommandWithCanExecuteChanged.CanExecute(null))
                ExampleAsyncCommandWithCanExecuteChanged.Execute(null);
            
            if(ExampleAsyncIntCommandWithCanExecute.CanExecute(1))
                ExampleAsyncIntCommandWithCanExecute.Execute(1);
        }
        finally
        {
            _isBusy = false;
        }
    }
}
```

### `AsyncValueCommand`

Allows for `ValueTask` to safely be used asynchronously with `ICommand`.

If you're new to ValueTask, check out this great write-up, [Understanding the Whys, Whats, and Whens of ValueTask
](https://blogs.msdn.microsoft.com/dotnet/2018/11/07/understanding-the-whys-whats-and-whens-of-valuetask?WT.mc_id=mobile-0000-bramin).

- `AsyncValueCommand<TExecute, TCanExecute> : IAsyncValueCommand<TExecute, TCanExecute>`
- `IAsyncValueCommand<TExecute, TCanExecute> : IAsyncValueCommand<TExecute>`
- `AsyncValueCommand<T> : IAsyncValueCommand<T>`
- `IAsyncValueCommand<T> : ICommand`
- `AsyncValueCommand : IAsyncValueCommand`
- `IAsyncValueCommand : ICommand`

```csharp
public AsyncValueCommand(Func<TExecute, ValueTask> execute,
                            Func<TCanExecute, bool>? canExecute = null,
                            Action<Exception>? onException = null,
                            bool continueOnCapturedContext = false)
```

```csharp
public AsyncValueCommand(Func<T, ValueTask> execute,
                            Func<object?, bool>? canExecute = null,
                            Action<Exception>? onException = null,
                            bool continueOnCapturedContext = false)
```

```csharp
public AsyncValueCommand(Func<ValueTask> execute,
                            Func<object?, bool>? canExecute = null,
                            Action<Exception>? onException = null,
                            bool continueOnCapturedContext = false)
```

```csharp
public class ExampleClass
{
    bool _isBusy;

    public ExampleClass()
    {
        ExampleValueTaskCommand = new AsyncValueCommand(ExampleValueTaskMethod);
        ExampleValueTaskIntCommand = new AsyncValueCommand<int>(ExampleValueTaskMethodWithIntParameter);
        ExampleValueTaskIntCommandWithCanExecute = new AsyncValueCommand<int, int>(ExampleValueTaskMethodWithIntParameter, CanExecuteInt);
        ExampleValueTaskExceptionCommand = new AsyncValueCommand(ExampleValueTaskMethodWithException, onException: ex => Debug.WriteLine(ex.ToString()));
        ExampleValueTaskCommandWithCanExecuteChanged = new AsyncValueCommand(ExampleValueTaskMethod, _ => !IsBusy);
        ExampleValueTaskCommandReturningToTheCallingThread = new AsyncValueCommand(ExampleValueTaskMethod, continueOnCapturedContext: true);
    }

    public IAsyncValueCommand ExampleValueTaskCommand { get; }
    public IAsyncValueCommand<int> ExampleValueTaskIntCommand { get; }
    public IAsyncCommand<int, int> ExampleValueTaskIntCommandWithCanExecute { get; }
    public IAsyncValueCommand ExampleValueTaskExceptionCommand { get; }
    public IAsyncValueCommand ExampleValueTaskCommandWithCanExecuteChanged { get; }
    public IAsyncValueCommand ExampleValueTaskCommandReturningToTheCallingThread { get; }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                ExampleValueTaskCommandWithCanExecuteChanged.RaiseCanExecuteChanged();
            }
        }
    }

    async ValueTask ExampleValueTaskMethod()
    {
        var random = new Random();
        if (random.Next(10) > 9)
            await Task.Delay(1000);
    }

    async ValueTask ExampleValueTaskMethodWithIntParameter(int parameter)
    {
        var random = new Random();
        if (random.Next(10) > 9)
            await Task.Delay(parameter);
    }

    async ValueTask ExampleValueTaskMethodWithException()
    {
        var random = new Random();
        if (random.Next(10) > 9)
            await Task.Delay(1000);

        throw new Exception();
    }

    bool CanExecuteInt(int count)
    {
        if(count > 2)
            return true;
        
        return false;
    }

    void ExecuteCommands()
    {
        _isBusy = true;

        try
        {
            ExampleValueTaskCommand.Execute(null);
            ExampleValueTaskIntCommand.Execute(1000);
            ExampleValueTaskExceptionCommand.Execute(null);
            ExampleValueTaskCommandReturningToTheCallingThread.Execute(null);

            if (ExampleValueTaskCommandWithCanExecuteChanged.CanExecute(null))
                ExampleValueTaskCommandWithCanExecuteChanged.Execute(null);

            if(ExampleValueTaskIntCommandWithCanExecute.CanExecute(2))
                ExampleValueTaskIntCommandWithCanExecute.Execute(2);
        }
        finally
        {
            _isBusy = false;
        }
    }
}
```

## Learn More
- [Removing Async Void](https://johnthiriet.com/removing-async-void/)
- [MVVM Going Async with Async Command](https://johnthiriet.com/mvvm-going-async-with-async-command/)
- [Asynchronous Programming in .NET](https://docs.microsoft.com/dotnet/csharp/async?WT.mc_id=mobile-0000-bramin)
- [The Managed Thread Pool](https://docs.microsoft.com/dotnet/standard/threading/the-managed-thread-pool?WT.mc_id=mobile-0000-bramin)
- [Understanding the Whys, Whats, and Whens of ValueTask](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/?WT.mc_id=mobile-0000-bramin)
- [Async/Await Best Practices Video](https://www.youtube.com/watch?v=yyT6dSjq-nE&feature=youtu.be)
- [What is Synchronization Context?](http://hamidmosalla.com/2018/06/24/what-is-synchronizationcontext/)
