namespace HookHook.Backend.Exceptions
{
    /// <summary>
    /// Type of user exception
    /// </summary>
    public enum TypeUserException
    {
        Username,
        Email,
        Password
    }

    /// <summary>
    /// Exception thrown when there is a problem with an User
    /// </summary>
    public class UserException : Exception
    {
        public TypeUserException Type;

        public UserException(TypeUserException type)
            => Type = type;

        public UserException(TypeUserException type, string message)
            : base(message) =>
            Type = type;

        public UserException(TypeUserException type, string message, Exception inner)
            : base(message, inner) =>
            Type = type;
    }
}