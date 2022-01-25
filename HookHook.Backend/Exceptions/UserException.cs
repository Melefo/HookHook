namespace HookHook.Backend.Exceptions
{
    public class UserException : Exception
    {
        public UserException() : base()
        {
        }

        public UserException(string message)
            : base(message)
        {
        }

        public UserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}