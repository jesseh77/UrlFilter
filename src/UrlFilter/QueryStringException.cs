using System;

namespace UrlFilter
{
    public class QueryStringException : Exception
    {
        public QueryStringException(string message) : base(message)
        {
        }
    }
}
