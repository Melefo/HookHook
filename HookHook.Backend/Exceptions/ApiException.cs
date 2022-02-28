namespace HookHook.Backend.Exceptions
{
    /// <summary>
    /// Exception thrown when there is a problem with an API
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException() : base()
        {
        }

        public ApiException(string message)
            : base(message)
        {
        }

        public ApiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}