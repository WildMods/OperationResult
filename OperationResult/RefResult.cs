// ReSharper disable InconsistentNaming
using System;
using System.Diagnostics.CodeAnalysis;
using OperationResult.Tags;

namespace OperationResult
{
    /// <summary>
    /// Result of operation (no result type, only Error field)
    /// </summary>
    public readonly ref struct RefResult<E>
    {
        private readonly E? _error;
        private readonly bool _isOk;

        public RefResult()
        {
            _isOk = true;
        }

        internal RefResult(E error)
        {
            _isOk = false;
            _error = error;
        }

        public bool TryGetValue([NotNullWhen(false)] out E? error)
        {
            error = _error;
            return _isOk;
        }
        
        public string GetErrorMessage() => _error?.ToString() ?? string.Empty;

        public static implicit operator RefResult<E>(SuccessTag _)
        {
            return new();
        }

        public static implicit operator RefResult<E>(ErrorTag<E> tag)
        {
            return new(tag.Error);
        }

        /// <summary>
        /// <para>Returns true if the result is Err.</para>
        /// </summary>
        /// <returns></returns>
        public bool IsErr() => !_isOk;

        /// <summary>
        /// <para>Returns true if the result is Err and the value inside of it matches a predicate.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsErrAnd(Func<E, bool> f)
        {
            return !_isOk && f(_error!);
        }

        /// <summary>
        /// <para>Converts from Result&lt;T, E&gt; to E?.</para>
        ///
        /// <para>Converts self into an E?, consuming self, and discarding the success value, if any.</para>
        /// </summary>
        /// <returns></returns>
        public E? Err()
        {
            return _isOk ? default : _error;
        }

        /// <summary>
        /// <para>Returns true if the result is Ok.</para>
        /// </summary>
        /// <returns></returns>
        public bool IsOk() => _isOk;

        /// <summary>
        /// <para>Returns true if the result is Ok and a closure returns true.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsOkAnd(Func<bool> f)
        {
            return _isOk && f();
        }

        /// <summary>
        /// <para>Due to language limitations, this does nothing. It's only included to match the spec.</para>
        /// </summary>
        /// <returns></returns>
        public void Ok() { }

        /// <summary>
        /// <para>Returns res if the result is Ok, otherwise returns the Err value of self.</para>
        ///
        /// <para>Arguments passed to And are eagerly evaluated; if you are passing the result of a function call, it
        /// is recommended to use AndThen, which is lazily evaluated.</para>
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public RefResult<E> And(RefResult<E> res)
        {
            return _isOk ? res : this;
        }

        /// <summary>
        /// <para>Calls f if the result is Ok, otherwise returns the Err value of self.</para>
        ///
        /// <para>This function can be used for control flow based on Result values.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public RefResult<E> AndThen(Func<RefResult<E>> f)
        {
            return _isOk ? f() : this;
        }

        /// <summary>
        /// <para>Returns res if the result is Err, otherwise returns the Ok value of self.</para>
        ///
        /// <para>Arguments passed to Or are eagerly evaluated; if you are passing the result of a function call, it is
        /// recommended to use OrElse, which is lazily evaluated.</para>
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public RefResult<E> Or(RefResult<E> res)
        {
            return _isOk ? this : res;
        }

        /// <summary>
        /// <para>Calls f if the result is Err, otherwise returns the Ok value of self.</para>
        ///
        /// <para>This function can be used for control flow based on result values.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public RefResult<E> OrElse(Func<E, RefResult<E>> f)
        {
            return _isOk ? this : f(_error!);
        }

        /// <summary>
        /// <para>Calls a function with a reference to the contained value if Ok.</para>
        ///
        /// <para>Returns the original result.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public RefResult<E> Inspect(Action f)
        {
            if (_isOk) f();
            return this;
        }

        /// <summary>
        /// <para>Calls a function with a reference to the contained value if Err.</para>
        ///
        /// <para>Returns the original result.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public RefResult<E> InspectErr(Action<E> f)
        {
            if (!_isOk) f(_error!);
            return this;
        }

        /// <summary>
        /// <para>Maps a Result to the typed Result of a closure, leaving an Err value untouched.</para>
        /// </summary>
        public RefResult<T, E> Map<T>(Func<T> f) where T : allows ref struct
        {
            return _isOk ? Helpers.Ok(f()) : Helpers.Err(_error!);
        }

        /// <summary>
        /// <para>Returns the provided default (if Err), or the result of a closure (if Ok).</para>
        /// </summary>
        public T MapOr<T>(T def, Func<T> f) where T : allows ref struct
        {
            return _isOk ? f() : def;
        }

        /// <summary>
        /// <para>Maps a Result to T by applying fallback function def to a contained Err value, or the result of a
        /// closure.</para>
        ///
        /// <para>This function can be used to unpack a successful result while handling an error.</para>
        /// </summary>
        public T MapOrElse<T>(Func<E, T> def, Func<T> f) where T : allows ref struct
        {
            return _isOk ? f() : def(_error!);
        }

        /// <summary>
        /// <para>Maps a Result to a T by the result of a closure if the result is Ok, otherwise if Err, returns the
        /// default value for the type T.</para>
        /// </summary>
        public T MapOrDefault<T>(Func<T> f) where T : notnull, allows ref struct
        {
            return _isOk ? f() : default!;
        }

        /// <summary>
        /// <para>Maps a Result&lt;T, E&gt; to Result&lt;T, F&gt; by applying a function to a contained Err value,
        /// leaving an Ok value untouched.</para>
        ///
        /// <para>This function can be used to pass through a successful result while handling an error.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public RefResult<F> MapErr<F>(Func<E, RefResult<F>> f)
        {
            return !_isOk ? f(_error!) : Helpers.Ok();
        }

        /// <summary>
        /// <para>Throws error if this is an error. Only included to match spec.</para>
        /// </summary>
        public void Unwrap()
        {
            if (!_isOk) throw new InvalidOperationException($"Unwrap called on Err value: {_error!}");
        }

        /// <summary>
        /// Does nothing. Only included to match spec.
        /// </summary>
        public void UnwrapOr() { }

        /// <summary>
        /// <para>You probably want InspectErr. This is only included to match spec.</para>
        /// </summary>
        public void UnwrapOrElse(Action<E> def)
        {
            if (!_isOk) def(_error!);
        }

        /// <summary>
        /// <para>Throws a specified error message if the contained value is an Err</para>
        /// </summary>
        /// <param name="message">Error message</param>
        /// <exception cref="InvalidOperationException">If the contained value is an Err</exception>
        public void Expect(string message)
        {
            if (!_isOk) throw new InvalidOperationException(message);
        }

        /// <summary>
        /// <para>Returns the contained Err value.</para>
        ///
        /// <para>Throws a specified error message if the contained value is Ok</para> 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>The contained Err value</returns>
        /// <exception cref="InvalidOperationException">If the contained value is Ok</exception>
        public E ExpectErr(string message)
        {
            return !_isOk ? _error! : throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Result of operation (with Error field)
    /// </summary>
    /// <typeparam name="T">Type of Value field</typeparam>
    /// <typeparam name="E">Type of Error field</typeparam>
    public readonly ref struct RefResult<T, E> where T : allows ref struct
    {
        private readonly T? _value;
        internal readonly E? Error;
        private readonly bool _isOk;

        public T? Value => _value;

        private RefResult(T? result)
        {
            _isOk = true;
            _value = result;
        }

        private RefResult(E? error)
        {
            _isOk = false;
            Error = error;
        }

        public bool TryGetValue([NotNullWhen(true)] ref T? result, [NotNullWhen(false)] out E? error)
        {
            result = _value;
            error = Error;
            return _isOk;
        }

        public string GetErrorMessage() => Error?.ToString() ?? string.Empty;

        public static implicit operator RefResult<T, E>(T? result)
        {
            return new(result);
        }

        public static implicit operator RefResult<T, E>(SuccessTag<T> tag)
        {
            return new(tag.Value);
        }

        public static implicit operator RefResult<T, E>(ErrorTag<E> tag)
        {
            return new(tag.Error);
        }

        private RefResult<U, E> ConvertError<U>() where U : allows ref struct
        {
            return Helpers.Err(Error!);
        }

        /// <summary>
        /// <para>Returns true if the result is Err.</para>
        /// </summary>
        /// <returns></returns>
        public bool IsErr() => !_isOk;

        /// <summary>
        /// <para>Returns true if the result is Err and the value inside of it matches a predicate.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsErrAnd(Func<E, bool> f)
        {
            return !_isOk && f(Error!);
        }

        /// <summary>
        /// <para>Converts from Result&lt;T, E&gt; to E?.</para>
        ///
        /// <para>Converts self into an E?, consuming self, and discarding the success value, if any.</para>
        /// </summary>
        /// <returns></returns>
        public E? Err()
        {
            return _isOk ? default : Error;
        }

        /// <summary>
        /// <para>Returns true if the result is Ok.</para>
        /// </summary>
        /// <returns></returns>
        public bool IsOk() => _isOk;

        /// <summary>
        /// <para>Returns true if the result is Ok and the value inside of it matches a predicate.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsOkAnd(Func<T, bool> f)
        {
            return _isOk && f(_value!);
        }

        /// <summary>
        /// <para>Converts from Result&lt;T, E&gt; to T?.</para>
        ///
        /// <para>Converts self into an T?, consuming self, and converting the error to None, if any.</para>
        ///
        /// <para>Due to language limitations, this function replaces UnwrapOrDefault, as they would serve the same
        /// purpose.</para>
        /// </summary>
        /// <returns></returns>
        public T? Ok()
        {
            return _isOk ? _value : default;
        }

        /// <summary>
        /// <para>Returns res if the result is Ok, otherwise returns the Err value of self.</para>
        ///
        /// <para>Arguments passed to And are eagerly evaluated; if you are passing the result of a function call, it
        /// is recommended to use AndThen, which is lazily evaluated.</para>
        /// </summary>
        /// <param name="res"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public RefResult<U, E> And<U>(RefResult<U, E> res) where U : allows ref struct
        {
            return _isOk && res._isOk || _isOk ? res : ConvertError<U>();
        }

        /// <summary>
        /// <para>Calls f if the result is Ok, otherwise returns the Err value of self.</para>
        ///
        /// <para>This function can be used for control flow based on Result values.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public RefResult<U, E> AndThen<U>(Func<T, RefResult<U, E>> f) where U : allows ref struct
        {
            return _isOk ? f(_value!) : ConvertError<U>();
        }

        /// <summary>
        /// <para>Returns res if the result is Err, otherwise returns the Ok value of self.</para>
        ///
        /// <para>Arguments passed to Or are eagerly evaluated; if you are passing the result of a function call, it is
        /// recommended to use OrElse, which is lazily evaluated.</para>
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public RefResult<T, E> Or(RefResult<T, E> res)
        {
            return _isOk ? this : res;
        }

        /// <summary>
        /// <para>Calls f if the result is Err, otherwise returns the Ok value of self.</para>
        ///
        /// <para>This function can be used for control flow based on result values.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public RefResult<T, E> OrElse(Func<E, RefResult<T, E>> f)
        {
            return _isOk ? this : f(Error!);
        }

        /// <summary>
        /// <para>Calls a function with a reference to the contained value if Ok.</para>
        ///
        /// <para>Returns the original result.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public RefResult<T, E> Inspect(Action<T> f)
        {
            if (_isOk) f(_value!);
            return this;
        }

        /// <summary>
        /// <para>Calls a function with a reference to the contained value if Err.</para>
        ///
        /// <para>Returns the original result.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public RefResult<T, E> InspectErr(Action<E> f)
        {
            if (!_isOk) f(Error!);
            return this;
        }

        /// <summary>
        /// <para>Maps a Result&lt;T, E&gt; to Result&lt;U, E&gt; by applying a function to a contained Ok value,
        /// leaving an Err value untouched.</para>
        ///
        /// <para>This function can be used to compose the results of two functions.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public RefResult<U, E> Map<U>(Func<T, U> f) where U : allows ref struct
        {
            return _isOk ? f(_value!) : ConvertError<U>();
        }

        /// <summary>
        /// <para>Returns the provided default (if Err), or applies a function to the contained value (if Ok).</para>
        ///
        /// <para>Arguments passed to MapOr are eagerly evaluated; if you are passing the result of a function call, it
        /// is recommended to use MapOrElse, which is lazily evaluated.</para>
        /// </summary>
        /// <param name="def"></param>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public U MapOr<U>(U def, Func<T, U> f) where U : allows ref struct
        {
            return _isOk ? f(_value!) : def;
        }

        /// <summary>
        /// <para>Maps a Result&lt;T, E&gt; to U by applying fallback function def to a contained Err value, or
        /// function f to a contained Ok value.</para>
        ///
        /// <para>This function can be used to unpack a successful result while handling an error.</para>
        /// </summary>
        /// <param name="def"></param>
        /// <param name="f"></param>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public U MapOrElse<U>(Func<E, U> def, Func<T, U> f) where U : allows ref struct
        {
            return _isOk ? f(_value!) : def(Error!);
        }

        /// <summary>
        /// <para>Maps a Result&lt;T, E&gt; to a U by applying function f to the contained value if the result is Ok,
        /// otherwise if Err, returns the default value for the type U.</para>
        /// </summary>
        /// <param name="f">The provided function to apply to the contained value.</param>
        /// <typeparam name="U">The type to transform the contained value into.</typeparam>
        /// <returns>The result of applying f to the contained value, or the default value for non-nullable type
        /// U</returns>
        public U MapOrDefault<U>(Func<T, U> f) where U : notnull, allows ref struct
        {
            return _isOk ? f(_value!) : default!;
        }

        /// <summary>
        /// <para>Maps a Result&lt;T, E&gt; to Result&lt;T, F&gt; by applying a function to a contained Err value,
        /// leaving an Ok value untouched.</para>
        ///
        /// <para>This function can be used to pass through a successful result while handling an error.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public RefResult<T, F> MapErr<F>(Func<E, RefResult<T, F>> f)
        {
            return !_isOk ? f(Error!) : Helpers.Ok(_value!);
        }

        /// <summary>
        /// <para>Returns the contained Ok value</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If called on a contained Err value</exception>
        public T Unwrap()
        {
            return _isOk ? _value! : throw new InvalidOperationException($"Unwrap called on Err value: {Error!}");
        }

        /// <summary>
        /// <para>Returns the contained Ok value or a provided default.</para>
        /// </summary>
        /// <param name="def">The value to be returned if this is an Err value</param>
        /// <returns></returns>
        public T UnwrapOr(T def)
        {
            return _isOk ? _value! : def;
        }

        /// <summary>
        /// <para>Returns the contained Ok value or computes it from a closure.</para>
        /// </summary>
        /// <param name="def">The closure</param>
        /// <returns></returns>
        public T UnwrapOrElse(Func<E, T> def)
        {
            return _isOk ? _value! : def(Error!);
        }

        /// <summary>
        /// <para>Returns the contained Ok value, consuming the self value.</para>
        ///
        /// <para>Because this function may directly throw an error, its use is generally discouraged. Instead, prefer
        /// to use TryGetValue and handle the Err case explicitly, or call UnwrapOr, UnwrapOrElse, or Ok (not
        /// UnwrapOrDefault, as language limitations would have rendered that a duplicate of Ok).</para>
        /// </summary>
        /// <param name="message">Error message</param>
        /// <exception cref="InvalidOperationException">If the contained value is an Err</exception>
        public T Expect(string message)
        {
            return _isOk ? _value! : throw new InvalidOperationException(message);
        }

        /// <summary>
        /// <para>Returns the contained Err value.</para>
        ///
        /// <para>Throws a specified error message if the contained value is Ok</para> 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>The contained Err value</returns>
        /// <exception cref="InvalidOperationException">If the contained value is Ok</exception>
        public E ExpectErr(string message)
        {
            return !_isOk ? Error! : throw new InvalidOperationException(message);
        }
    }
}
