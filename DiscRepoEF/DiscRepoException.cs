using System;
using System.Runtime.Serialization;

namespace DiscRepoEF
{   
    public class DiscRepoException : ArgumentException
    {
        public DiscRepoException()
        {
        }

        protected DiscRepoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DiscRepoException(string message) : base(message)
        {
        }

        public DiscRepoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}