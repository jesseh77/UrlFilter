using System;
using System.Collections.Generic;
using System.Linq;

namespace UrlFilter
{
    internal class QueryStringProcessor
    {
        internal static List<Token> GetWhereSegments(string queryString)
        {
            var querySegments = PadBracketsWithSpace(queryString).Split(' ');
            var tokens = Enumerable.Where<string>(querySegments, x => !String.IsNullOrWhiteSpace(x))
                .Select(x => new Token
                {
                    GroupPriority = OperatorPrecedence.GetOperatorPrecedence(x),
                    TokenValue = x
                }).ToList();

            return tokens;
        }

        private static string PadBracketsWithSpace(string queryString)
        {
            return queryString.Replace("(", " ( ").Replace(")", " ) ");
        }
    }
}
