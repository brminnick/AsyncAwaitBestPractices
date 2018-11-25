# AsyncAwaitBestPractices

[![Build Status](https://brminnick.visualstudio.com/AsyncAwaitBestPractices/_apis/build/status/AsyncAwaitBestPractices-.NET%20Desktop-CI)](https://brminnick.visualstudio.com/AsyncAwaitBestPractices/_build/latest?definitionId=5)

Extensions for `System.Threading.Tasks.Task`, inspired by [John Thiriet](https://github.com/johnthiriet)'s blog posts: [Removing Async Void](https://johnthiriet.com/removing-async-void/) and [MVVM - Going Async With AsyncCommand](https://johnthiriet.com/mvvm-going-async-with-async-command/).


- AsyncAwaitBestPractices
  - An extension method to safely fire-and-forget a `Task`:
    - `SafeFireAndForget`
  - [Usage instructions below](#asyncawaitbestpractices)
- AsyncAwaitBestPractices.MVVM
  - Allows for `Task` to safely be used asynchronously with `ICommand`:
    - `AsyncCommand<T> : IAsyncCommand`
    - `AsyncCommand : IAsyncCommand`
    - `IAsyncCommand : ICommand``Task` to safely be used asynchronously with `ICommand`
  - [Usage instructions below](#asyncawaitbestpracticesmvvm)

## Setup

###  AsyncAwaitBestPractices

[![NuGet](https://img.shields.io/nuget/v/AsyncAwaitBestPractices.svg?label=NuGet)](https://www.nuget.org/packages/AsyncAwaitBestPractices/) [![NuGet](https://img.shields.io/nuget/dt/AsyncAwaitBestPractices.svg?label=Downloads)](https://www.nuget.org/packages/AsyncAwaitBestPractices/)

  - Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices/ 
  - Add to any project supporting .NET Standard 1.0

### AsyncAwaitBestPractices.MVVM

[![NuGet](https://img.shields.io/nuget/v/AsyncAwaitBestPractices.MVVM.svg?label=NuGet)](https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/) [![NuGet](https://img.shields.io/nuget/dt/AsyncAwaitBestPractices.MVVM.svg?label=Downloads)](https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/)

  - Available on NuGet: https://www.nuget.org/packages/AsyncAwaitBestPractices.MVVM/  
  - Add to any project supporting .NET Standard 2.0
  
## Usage

### AsyncAwaitBestPractices

An extension method to safely fire-and-forget a `Task`:
- `SafeFireAndForget`

```csharp
void HandleButtonTapped(object sender, EventArgs e)
{
    // Allows the async Task method to safely run on a different thread while not awaiting its completion
    ExampleAsyncMethod().SafeFireAndForget();
    
    // HandleButtonTapped continues execution here while `ExampleAsyncMethod()` is running on a different thread
    // ...
}

async Task ExampleAsyncMethod()
{
    await Task.Delay(1000);
}
```

### AsyncAwaitBestPractices.MVVM

Allows for `Task` to safely be used asynchronously with `ICommand`:
- `AsyncCommand<T> : IAsyncCommand`
- `AsyncCommand : IAsyncCommand`
- `IAsyncCommand : ICommand`

```csharp
public class ExampleClass
{
    public ExampleClass()
    {
        ExampleAsyncCommand = new AsyncCommand(ExampleAsyncMethod);
        ExampleAsyncIntCommand = new AsyncCommand<int>(ExampleAsyncMethodWithIntParameter);
        ExampleAsyncExceptionCommand = new AsyncCommand(ExampleAsyncMethodWithException, onException: ex => Console.WriteLine(ex.Message));
        ExampleAsyncCommandNotReturningToTheCallingThread = new AsyncCommand(ExampleAsyncMethod, continueOnCapturedContext: false);
    }
    
    public IAsyncCommand ExampleAsyncCommand { get; }
    public IAsyncCommand ExampleAsyncIntCommand { get; }
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
