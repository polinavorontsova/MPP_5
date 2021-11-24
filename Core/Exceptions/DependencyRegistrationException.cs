using System;

namespace Core.Exceptions
{
    public class DependencyRegistrationException : Exception
    {
        public DependencyRegistrationException(string message) : base(message)
        {
        }

        public DependencyRegistrationException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}