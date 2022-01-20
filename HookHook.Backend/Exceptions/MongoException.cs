namespace HookHook.Backend.Exceptions
{
    public class MongoException : Exception
    {
        public MongoException() : base()
        {
        }

        public MongoException(string message)
            : base(message)
        {
        }

        public MongoException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}