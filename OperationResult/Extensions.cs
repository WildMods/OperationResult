using System;

namespace OperationResult;

public static class Extensions
{
    public static ErrorStack Context(this string message, string context) => new ErrorStack(message).Context(context);

    public static ErrorStack Context(this Exception e, string context) => new ErrorStack(e.Message, e).Context(context);

    /// <summary>
    /// Attaches a context string directly onto the contained Err value, if any.
    /// </summary>
    /// <param name="result">The called Result value, whose Error type must be ErrorStack</param>
    /// <param name="context">The context description to be attached to the inner ErrorStack</param>
    /// <typeparam name="T">The Ok value type</typeparam>
    /// <returns>The called value, for chaining.</returns>
    public static Result<T, ErrorStack> Context<T>(this Result<T, ErrorStack> result, string context)
    {
        if (result.IsErr()) result.Error!.Context(context);
        return result;
    }

    /// <summary>
    /// Attaches a context string directly onto the contained Err value, if any.
    /// </summary>
    /// <param name="result">The called Result value, whose Error type must be ErrorStack</param>
    /// <param name="context">The context description to be attached to the inner ErrorStack</param>
    /// <typeparam name="T">The Ok value type</typeparam>
    /// <returns>The called value, for chaining.</returns>
    public static RefResult<T, ErrorStack> Context<T>(this RefResult<T, ErrorStack> result, string context) where T : allows ref struct
    {
        if (result.IsErr()) result.Error!.Context(context);
        return result;
    }

    /// <summary>
    /// Transposes a Result of a Nullable into a Nullable of a Result.
    ///
    /// Ok(null) will be mapped to null. Ok(notnull) and Err will be mapped to Ok(notnull) and Err.
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <returns></returns>
    // public static Result<T, E>? Transpose<T, E>(this Result<T?, E> result)
    // {
    //     return result.IsErr() ? Helpers.Err(result.Error!) : result.Value;
    // }
}