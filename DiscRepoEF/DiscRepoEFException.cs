using System;
using System.Runtime.Serialization;

namespace DiscRepoEF
{   
    public class DiscRepoEfException : Exception
    {
        public DiscRepoEfException()
        {
        }

        protected DiscRepoEfException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DiscRepoEfException(string message) : base(message)
        {
        }

        public DiscRepoEfException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}