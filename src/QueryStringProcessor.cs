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
                    var propertyName = current.Value.TokenValue;

                    var segments = propertyName.Split('.');
                    Expression parameterExpression = Expression.Parameter(typeof(T));
                    PropertyInfo propertyInfo = null;
                    Type propertyType = typeof(T);

                    foreach (var segment in segments)
                    {
                        propertyInfo = GetPropertyInfo(propertyType, segment);
                        parameterExpression = Expression.Property(parameterExpression, propertyInfo);
                        propertyType = propertyInfo.PropertyType;
                    }

                    var value = StripOuterSingleQuotes(current.Next.Next.Value.TokenValue);
                    var operationType = current.Next.Value.TokenValue;

                    var propValue = Convert.ChangeType(value, propertyInfo.PropertyType);
                    var rightExpression = Expression.Constant(propValue);

                    var expression = ExpressionOperators.OperatorExpression(operationType, parameterExpression, rightExpression);

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

        private static string StripOuterSingleQuotes(string value)
        {
            if (value[0] == '\'' && value[value.Length - 1] == '\'')
            {
                return value.Substring(1, value.Length - 2);
            }
            return value;
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

        private static PropertyInfo GetPropertyInfo(Type type, string name)
        {
            return type.GetRuntimeProperties()
                .Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
