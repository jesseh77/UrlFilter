using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    public static class QueryString<T> where T : class
    {        
        public static Expression<Func<T,bool>> GetWhereExpression(string queryString)
        {
            var segments = GetWhereSegments(queryString);
            var paramExpression = Expression.Parameter(typeof(T));

            var expression = ReduceQuerySegments(segments, paramExpression);

            return Expression.Lambda<Func<T, bool>>(expression, paramExpression);
        }

        private static Expression ReduceQuerySegments(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            ExpressionProcessors.ProcessUnary(tokens, OperatorPrecedence.Precedence.Unary);
            ExpressionProcessors.ProcessEqualityAndRelational<T>(tokens, OperatorPrecedence.Precedence.Relational, paramExpression);
            ExpressionProcessors.ProcessEqualityAndRelational<T>(tokens, OperatorPrecedence.Precedence.Equality, paramExpression);
            ExpressionProcessors.ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalAnd);
            ExpressionProcessors.ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalOr);

            return tokens.First.Value.OperatorExpression;
        }

        private static LinkedList<Token> GetWhereSegments(string queryString)
        {
            var querySegments = PadBracketsWithSpace(queryString).Split(' ');
            var tokens = querySegments
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new Token
                {
                    GroupPriority = OperatorPrecedence.GetOperatorPrecedence(x),
                    TokenValue = x
                });
            
            return new LinkedList<Token>(tokens);
        }

        private static string PadBracketsWithSpace(string queryString)
        {
            return queryString.Replace("(", " ( ").Replace(")", " ) ");
        }
    }
}
