namespace Contracts.Http
{
    public enum ErrorCode
    {
        BadRequest = 40000,
        Unauthorized = 40100,
        WrongPassword = 40301,
        NotFound = 40400,
        AlreadyExists = 40900,
        InternalServerError = 50000,
        DbFailureError = 50001,
    }

    public class ErrorResponse
    {
        public ErrorCode Code { get; init; }
        public string Message { get; init; }
    }
}