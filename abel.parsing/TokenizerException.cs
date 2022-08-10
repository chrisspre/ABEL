using System.Runtime.Serialization;

namespace abel.parsing
{
    [Serializable]
    internal class TokenizerException : Exception
    {
        public TokenizerException()
        {
        }

        public TokenizerException(string? message) : base(message)
        {
        }

        public TokenizerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TokenizerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}