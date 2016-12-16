using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    public class WhereExpression : IFilterExpression
    {
        public static readonly IFilterExpression Build = new WhereExpression();
        private readonly ExpressionReducer _reducer;
        private readonly OperatorPrecedence _precedence;

        public WhereExpression()
        {
            var operators = new ExpressionOperators();
            var processors = new ExpressionProcessors(operators);
            _reducer = new ExpressionReducer(processors);
            _precedence = new OperatorPrecedence();
        }
        
        public Expression<Func<T,bool>> FromString<T>(string queryString) where T : class
        {
            var tokens = GetWhereSegments(queryString);
            var parameterExpression = Expression.Parameter(typeof(T));

            var expression = _reducer.ReduceGroupSegments<T>(tokens, parameterExpression);
            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        private List<Token> GetWhereSegments(string queryString)
        {
            var querySegments = PadBracketsWithSpace(queryString).Split(' ');
            var tokens = querySegments.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new Token
                {
                    GroupPriority = _precedence.GetOperatorPrecedence(x),
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
