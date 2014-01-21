using System;
namespace Exceptions
{
    public class ApiNotAvailableException : System.IO.IOException
    {

        public ApiNotAvailableException()
            : base("Failed accessing API")
        {
        }

        public ApiNotAvailableException(Exception innerException)
            : base("Failed accessing API", innerException)
        {
        }

    }
}
