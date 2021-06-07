# AsyncAwaitBestPractices.MVVM  

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

- Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/  
- Add to any project supporting .NET Standard 1.0

## `AsyncCommand`

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

## `AsyncValueCommand`

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
