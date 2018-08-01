using System;
using System.Net;

namespace SampleCPMProject
{
    public class ServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        
        public ServiceException(string message = "An unexpected error occurred", HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
