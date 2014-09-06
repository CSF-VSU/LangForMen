using System;
using System.Runtime.Serialization;

namespace LangForRealMen.AST
{
    public class ASTException : ApplicationException
    {
        public ASTException()
        {
        }

        public ASTException(string message)
            : base(message)
        {
        }

        public ASTException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ASTException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
