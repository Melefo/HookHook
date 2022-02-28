namespace HookHook.Backend.Exceptions
{
    /// <summary>
    /// Exception thrown when there is a problem with Mongo
    /// </summary>
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