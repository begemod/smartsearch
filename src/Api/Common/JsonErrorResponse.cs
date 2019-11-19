using System.Collections.Generic;

namespace Api.Common
{
    public class JsonErrorResponse
    {
        public string Message { get; set; }

        public IReadOnlyCollection<ErrorEntry> Errors { get; set; }

        public object DeveloperMessage { get; set; }
    }
}
