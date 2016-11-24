using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            ProcessEqualityAndRelational(tokens, OperatorPrecedence.Precedence.Relational, paramExpression);
            ProcessEqualityAndRelational(tokens, OperatorPrecedence.Precedence.Equality, paramExpression);
            ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalAnd);
            ProcessConditional(tokens, OperatorPrecedence.Precedence.ConditionalOr);

            return tokens.First.Value.OperatorExpression;
        }

        private static LinkedList<Token> ProcessConditional(LinkedList<Token> tokens, OperatorPrecedence.Precedence operation)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (current.Next.Value.GroupPriority == operation)
                {
                    current.Next.Value.OperatorExpression =
                        ExpressionOperators.OperatorExpression(current.Next.Value.TokenValue,
                            current.Value.OperatorExpression, current.Next.Next.Value.OperatorExpression);

                    tokens.Remove(current.Next.Next);
                    var next = current.Next;
                    tokens.Remove(current);
                    current = next;
                }
                else
                {
                    current = current.Next;
                }
                
            }
            return tokens;
        }

        private static LinkedList<Token> ProcessEqualityAndRelational(LinkedList<Token> tokens, OperatorPrecedence.Precedence operation, ParameterExpression paramExpression)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (current.Next.Value.GroupPriority == operation)
                {
                    var propertyInfo = GetPropertyInfo(current.Value.TokenValue);
                    current.Next.Value.OperatorExpression = TokenToExpression(
                        propertyInfo, current.Next.Value.TokenValue, current.Next.Next.Value.TokenValue, paramExpression);

                    tokens.Remove(current.Next.Next);
                    var next = current.Next;
                    tokens.Remove(current);
                    current = next;
                }
                else
                {
                    current = current.Next;
                }
            }
            return tokens;
        }

        private static Expression TokenToExpression(PropertyInfo property, string operation, string value, ParameterExpression paramExpression)
        {
            if (property.PropertyType == typeof(string) && value[0] == '\'' && value[value.Length -1] == '\'')
            {
                value = value.Substring(1, value.Length -2);
            }
            var leftExpression = Expression.Property(paramExpression, property);

            var propValue = Convert.ChangeType(value, property.PropertyType);
            var rightExpression = Expression.Constant(propValue);

            var expression = ExpressionOperators.OperatorExpression(operation, leftExpression, rightExpression);
            return expression;
        }

        private static LinkedList<Token> GetWhereSegments(string queryString)
        {
            var query = queryString.Replace("(", " ( ").Replace(")", " ) ").Split(' ');
            var tokens = query.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new Token
            {
                GroupPriority = OperatorPrecedence.GetOperatorPrecedence(x),
                TokenValue = x
            });
            
            return new LinkedList<Token>(tokens);
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            return typeof(T).GetRuntimeProperties()
                .Single(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
