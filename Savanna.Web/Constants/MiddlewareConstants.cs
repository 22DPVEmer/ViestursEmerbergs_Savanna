using System.Net;

namespace Savanna.Web.Constants
{
    public static class MiddlewareConstants
    {
        public static class Messages
        {
            public const string GameServiceError = "Game service error";
            public const string ConfigurationError = "Configuration error";
            public const string FileNotFound = "File not found";
            public const string InvalidJsonFormat = "Invalid JSON format";
            public const string UnauthorizedAccess = "Unauthorized access";
            public const string UnauthorizedAccessDetails = "You do not have permission to perform this action";
            public const string InternalServerError = "An error occurred while processing your request";
            public const string InternalServerErrorDetails = "Please try again later";
        }

        public static class StatusCodes
        {
            public const int BadRequest = (int)HttpStatusCode.BadRequest;
            public const int Unauthorized = (int)HttpStatusCode.Unauthorized;
            public const int NotFound = (int)HttpStatusCode.NotFound;
            public const int InternalServerError = (int)HttpStatusCode.InternalServerError;
        }
    }
} 