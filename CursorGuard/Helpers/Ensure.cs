using System;
using System.Runtime.Serialization;

namespace CursorGuard.Helpers
{
    internal static class Ensure
    {
        public static void Precondition<TException>(bool condition, string message) where TException : Exception
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

        public static T ArgumentNotNull<T>(T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argument;
        }

        public static string ArgumentNotNullOrEmptyString(string argument, string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("Argument cannot be null or empty string", argumentName);
            }

            return argument;
        }

        public static T ResultNotNull<T>(T result)
        {
            if (result == null)
            {
                throw new NullResultException();
            }

            return result;
        }

        public static string NotNullOrEmptyString<TException>(string value) where TException : Exception
        {
            return NotNullOrEmptyString<TException>(value, "Provided string should not be null or empty");
        }

        public static string NotNullOrEmptyString<TException>(string value, string message) where TException : Exception
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            TException ex;
            try
            {
                ex = (TException)Activator.CreateInstance(typeof(TException), message);
            }
            catch
            {
                ex = (TException)Activator.CreateInstance(typeof(TException));
            }

            throw ex;
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
