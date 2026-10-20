using System.Net;

namespace HaberApp.Core.Utils
{
    public class ResponseResult<T> where T : class
    {
        public T? Entity { get; set; } = null;
        public List<T>? Entities { get; set; } = null;
        public bool Success { get; set; } = true;
        public Dictionary<string, string>? Errors { get; set; } = null;
        public int? RowCount { get; set; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    }
}
