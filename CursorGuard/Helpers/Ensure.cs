using System;

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
    }
}
