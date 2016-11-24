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
            var expression = ReduceQuerySegments(segments);
            var paramExpression = Expression.Parameter(typeof(T));

            return Expression.Lambda<Func<T, bool>>(expression, paramExpression);
        }

        private static Expression ReduceQuerySegments(LinkedList<Token> tokens)
        {
            ProcessEqualityAndRelational(tokens, OperatorPrecedence.Precedence.Relational);
            ProcessEqualityAndRelational(tokens, OperatorPrecedence.Precedence.Equality);
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

        private static LinkedList<Token> ProcessEqualityAndRelational(LinkedList<Token> tokens, OperatorPrecedence.Precedence operation)
        {
            var current = tokens.First;
            while (current.Next.Next != null)
            {
                if (current.Next.Value.GroupPriority == operation)
                {
                    var propertyInfo = GetPropertyInfo(current.Value.TokenValue);
                    current.Next.Value.OperatorExpression = TokenToExpression(
                        propertyInfo, current.Next.Value.TokenValue, current.Next.Next.Value.TokenValue);

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

        private static Expression TokenToExpression(PropertyInfo property, string operation, string value)
        {
            var paramExpression = Expression.Parameter(typeof(T));
            var leftExpression = Expression.Property(paramExpression, property);

            var propValue = Convert.ChangeType(value, property.PropertyType);
            var rightExpression = Expression.Constant(propValue);

            var expression = ExpressionOperators.OperatorExpression(operation, leftExpression, rightExpression);
            return expression;
        }

        private static LinkedList<Token> GetWhereSegments(string queryString)
        {
            var query = queryString.Replace("(", " ( ").Replace(")", " ) ").Split(' ');
            var tokens = query.Select(x => new Token
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

        private static int FromString(string value)
        {
            int val;
            if (int.TryParse(value, out val))
            {
                return val;
            }
            throw new InvalidCastException($"Pameter {value} is not a valid integer");
        }
    }
}
