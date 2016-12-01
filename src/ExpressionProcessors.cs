using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UrlFilter
{
    internal static class ExpressionProcessors
    {
        internal static LinkedList<Token> ProcessConditional(LinkedList<Token> tokens, OperatorPrecedence.Precedence operation)
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

        internal static LinkedList<Token> ProcessUnary(LinkedList<Token> tokens, OperatorPrecedence.Precedence operation)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (current.Value.GroupPriority == operation)
                {
                    current.Next.Next.Value.IsNot = true;

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

        internal static LinkedList<Token> ProcessEqualityAndRelational<T>(LinkedList<Token> tokens, OperatorPrecedence.Precedence operation, ParameterExpression paramExpression)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (current.Next.Value.GroupPriority == operation)
                {
                    var propertyName = current.Value.TokenValue;

                    var segments = propertyName.Split('.');
                    Expression parameterExpression = paramExpression;
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
                    if (current.Next.Value.IsNot)
                    {
                        expression = Expression.Not(expression);
                    }

                    current.Next.Value.OperatorExpression = expression;

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

        private static PropertyInfo GetPropertyInfo(Type type, string name)
        {
            return type.GetRuntimeProperties()
                .Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static string StripOuterSingleQuotes(string value)
        {
            if (value[0] == '\'' && value[value.Length - 1] == '\'')
            {
                return value.Substring(1, value.Length - 2);
            }
            return value;
        }
    }
}
