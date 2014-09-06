using System;
using System.Runtime.Serialization;

namespace LangForRealMen.ParserLogic
{
    public class ParserBaseException : ApplicationException
    {
        public ParserBaseException()
        {
        }

        public ParserBaseException(string message)
            : base(message)
        {
        }

        public ParserBaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ParserBaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
