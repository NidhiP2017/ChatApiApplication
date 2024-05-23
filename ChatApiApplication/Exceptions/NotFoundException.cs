namespace ChatApiApplication.Exceptions
{
    public class NotFoundException : AuthException
    {
        #region Constructors

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, System.Exception inner) : base(message, inner)
        {
        }

        #endregion

        #region Properties

        public override int StatusCode { get; protected set; } = 404;

        #endregion

        #region Methods

        public override BaseErrorResponse GetResponse()
        {
            var response = base.GetDefaultResponse();
            response.Title = "Record Not Found!";
            response.Message = Message ?? "Requested Record Not Found.";

            return response;
        }

        #endregion
    }
}
