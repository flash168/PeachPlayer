using System.Net;

namespace Peach.Foundation.Models
{

    public class HttpResult<T>
    {
        public HttpResult(string errormage)
        {
            Message = errormage;
            IsSuccessful = false;
        }
        public HttpResult() { }
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public HttpStatusCode Code { get; set; }
        public T Content { get; set; }

    }

}
