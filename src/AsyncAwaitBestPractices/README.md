# AsyncAwaitBestPractices

[![NuGet](https://buildstats.info/nuget/AsyncAwaitBestPractices?includePreReleases=true)](https://www.nuget.org/packages/AsyncAwaitBestPractices/)

Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices/ 

- `SafeFireAndForget`
    - An extension method to safely fire-and-forget a `Task` or a `ValueTask`
    - Ensures the `Task` will rethrow an `Exception` if an `Exception` is caught in `IAsyncStateMachine.MoveNext()`
- `WeakEventManager`
    - Avoids memory leaks when events are not unsubscribed
    - Used by `AsyncCommand`, `AsyncCommand<T>`, `AsyncValueCommand`, `AsyncValueCommand<T>`
- [Usage instructions](#asyncawaitbestpractices-3)

## Setup

- Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices/ 
- Add to any project supporting .NET Standard 1.0
  
## Usage

### `SafeFireAndForget`
An extension method to safely fire-and-forget a `Task`.

`SafeFireAndForget` allows a Task to safely run on a different thread while the calling thread does not wait for its completion.

```csharp
public static async void SafeFireAndForget(this System.Threading.Tasks.Task task, System.Action<System.Exception>? onException = null, bool continueOnCapturedContext = false)
```

```csharp
public static async void SafeFireAndForget(this System.Threading.Tasks.ValueTask task, System.Action<System.Exception>? onException = null, bool continueOnCapturedContext = false)
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
    SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling()
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