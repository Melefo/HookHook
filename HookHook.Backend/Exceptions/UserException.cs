namespace HookHook.Backend.Exceptions
{
    public enum TypeUserException
    {
        Username,
        Email,
        Password
    }

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