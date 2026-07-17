namespace OperationResult.Tags
{
    public struct SuccessTag { }

    public ref struct SuccessTag<TResult> where TResult : allows ref struct
    {
        internal readonly TResult Value;

        internal SuccessTag(TResult result)
        {
            Value = result;
        }
    }
}
