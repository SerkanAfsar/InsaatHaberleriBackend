using HaberApp.Core.Extensions;
using System.Net;

namespace HaberApp.Core.Exceptions
{
    public class CustomException : Exception
    {
        public Dictionary<string, string> Errors { get; set; } = new();
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
        public CustomException(string title, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message)
        {
            this.Errors.Add(title.ToCamelCase(), message);
            this.StatusCode = statusCode;
        }
    }
}
