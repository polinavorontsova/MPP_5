using System;

namespace Core.Exceptions
{
    public class DependencyProviderException : Exception
    {
        public DependencyProviderException(string message) : base(message)
        {
        }

        public DependencyProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}