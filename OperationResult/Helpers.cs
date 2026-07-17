using OperationResult.Tags;

namespace OperationResult
{
    public static class Helpers
    {
        private static SuccessTag SuccessTag = new();

        /// <summary>
        /// Create "Success" Status or Result
        /// </summary>
        public static SuccessTag Ok()
        {
            return SuccessTag;
        }

        /// <summary>
        /// Create "Success" Status or Result
        /// </summary>
        public static SuccessTag<T> Ok<T>(T result) where T : allows ref struct
        {
            return new SuccessTag<T>(result);
        }

        private static ErrorTag ErrorTag = new();

        /// <summary>
        /// Create "Error" Status or Result
        /// </summary>
        public static ErrorTag Err()
        {
            return ErrorTag;
        }

        /// <summary>
        /// Create "Error" Status or Result
        /// </summary>
        public static ErrorTag<E> Err<E>(E error)
        {
            return new ErrorTag<E>(error);
        }
    }
}
