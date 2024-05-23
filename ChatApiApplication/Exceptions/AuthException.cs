using System.Runtime.Serialization;

namespace ChatApiApplication.Exceptions
{
    public abstract class AuthException : ApplicationException
    {
        public AuthException()
        {
        }

        public AuthException(string message) : base(message)
        {
        }

        public AuthException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected AuthException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
        public virtual int StatusCode { get; protected set; } = 500;
        public abstract BaseErrorResponse GetResponse();
        protected BaseErrorResponse GetDefaultResponse()
        {
            var response = new BaseErrorResponse();
            response.Type = this.GetType().Name;
            response.Status = this.StatusCode;
            response.Title = "Unhandle Exception";
            response.Message = Message;

            return response;
        }
    }
}
