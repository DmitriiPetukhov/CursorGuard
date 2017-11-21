using System;
using System.Runtime.Serialization;

namespace CursorGuard.Helpers
{
    internal static class Ensure
    {
        public static void Precondition<TException>(bool condition, string message) where TException : Exception, new()
        {
            if (condition)
            {
                return;
            }

            TException ex;
            try
            {
                ex = (TException) Activator.CreateInstance(typeof(TException), message);
            }
            catch
            {
                ex = (TException) Activator.CreateInstance(typeof(TException));
            }
            throw ex;
        }

        public static void ArgumentNotNull<T>(T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ArgumentNotNullOrEmptyString(string argument, string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("Argument cannot be null or empty string", argumentName);
            }
        }

        public static void ResultNotNull<T>(T result)
        {
            if (result == null)
            {
                throw new NullResultException();
            }
        }
    }

    public class NullResultException : Exception
    {
        public NullResultException() : base("Null in result where value is expected")
        {
        }

        public NullResultException(string message) : base(message)
        {
        }

        public NullResultException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullResultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
