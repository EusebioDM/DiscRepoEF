using System;
using System.Runtime.Serialization;

namespace DiscRepoEF
{
    public class DiscRepoConnectionException : InvalidOperationException
    {
        public DiscRepoConnectionException()
        {
        }

        protected DiscRepoConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DiscRepoConnectionException(string message) : base(message)
        {
        }

        public DiscRepoConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}