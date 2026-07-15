# OperationResult
__Rust-style error handling for C#__

Basic usage:
```csharp
using OperationResult;
using static OperationResult.Helpers;

public Result<double, string> SqrtOperation(double argument)
{
    if (argument < 0)
    {
        return Error("Argument must be greater than zero");
    }
    double result = Math.Sqrt(argument);
    return Ok(result);
}

public void Method()
{
    var result = SqrtOperation(123);

    if (result.IsOk())
    {
        Console.WriteLine("Value is: {0}", result.Ok());
    }
    else
    {
        Console.WriteLine("Error is: {0}", result.Err());
    }
}
```

## Result<E>

Result of some method when there is no return type defined.
i.e. replacement for functions that return void

Example:
```csharp
public Result<string> PrintNonNull(string? message)
{
    if (message == null)
    {
        return Error("Message was null!");
    }
    Console.WriteLine(message);
    return Ok();
}
```

## Result<T, E>

Either Result of some method or Error from this method.
i.e. replacement for functions that return T

Example:
```csharp
public Result<bool, string> ContainsValueAt(int[] container, int index, int value)
{
    if (index < 0 || index > container.Length)
    {
        return Error("Index out of bounds!");
    }
    return Ok(container[index] == value);
}
```

## Rust-like uses of Result<E> and Result<T, E>

Most applicable member functions of Result have been reimplemented, allowing for complex
chaining, similar to original Rust behavior.

Example:
```csharp
Ok<double, string>(36.0)
  .Map(d => SqrtOperation(d))
  .Inspect(d => Console.WriteLine($"Success! Square root is {d}"))
  .InspectErr(e => Console.Debug(e));

if (Ok<double, string>(36.0).Map(d => SqrtOperation(d)).IsOkAnd(d => d == 6.0))
{
  Console.WriteLine("Yay! We know math!");
}
```

## ErrorStack

Convenience type for emulating Rust `anyhow` crate context behavior.

Example:
```csharp
Result<ErrorStack> res = Err(
  new(
    "This is the user-facing error message.",
    new Exception("This generates a stack trace.")
  )
);
res.Context("This is some friendly context!");

Console.WriteLine(res.Err().ToString()); // Prints the following:
/*
This is the user-facing error message.

Caused by:
  0. This is some friendly context!
  1. Exception: This generates a stack trace.
*/

Console.WriteLine(res.Err().Stack); // Prints the standard C# stack trace
```