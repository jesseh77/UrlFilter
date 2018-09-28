using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UrlFilter.ExpressionProcessors
{
    public class ValueProcessor : IExpressionProcessor
    {
        private readonly ExpressionOperator _operators;
        private readonly List<string> _operands;
        
        public ExpressionCategory ExpressionCategory => ExpressionCategory.Value;

        public ValueProcessor(List<string> operands, ExpressionOperator operators)
        {
            _operands = operands;
            _operators = operators;
        }

        public bool CanProcess(string operand)
        {
            if (string.IsNullOrWhiteSpace(operand)) return false;
            return _operands.Contains(operand.ToLower());
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var currentToken = tokens.First;
            while (currentToken.Next != null)
            {
                if (CanProcess(currentToken.Next.Value.TokenValue))
                {
                    var propertyName = currentToken.Value.TokenValue;

                    Expression parameterExpression = paramExpression;
                    Expression returnExpression = null;
                    PropertyInfo propertyInfo = null;
                    var propertyType = parameterExpression.Type;

                    var propertySegments = propertyName.Split('.');
                    var segmentCount = propertySegments.Count();

                    for (int i = 0; i < segmentCount; i++)
                    {
                        var segment = propertySegments[i];

                        propertyInfo = GetPropertyInfo(propertyType, segment);
                        parameterExpression = Expression.Property(parameterExpression, propertyInfo);
                        propertyType = propertyInfo.PropertyType;

                        if (i != segmentCount - 1)
                        {
                            var notNullExpression = Expression.NotEqual(parameterExpression, Expression.Constant(null, typeof(object)));
                            returnExpression = AndAlso(returnExpression, notNullExpression);
                        }
                    }

                    var expressionValue = StripOuterSingleQuotes(currentToken.Next.Next.Value.TokenValue);

                    var propValue = ConvertValue(expressionValue, propertyInfo.PropertyType);
                    var rightExpression = Expression.Constant(propValue, propertyInfo.PropertyType);

                    var operationType = currentToken.Next.Value.TokenValue;
                    var valueExpression = _operators.OperatorExpression(operationType, parameterExpression, rightExpression);

                    if (currentToken.Next.Value.IsNot)
                    {
                        valueExpression = Expression.Not(valueExpression);
                    }

                    returnExpression = AndAlso(returnExpression, valueExpression);
                    currentToken.Next.Value.OperatorExpression = returnExpression;

                    tokens.Remove(currentToken.Next.Next);
                    var next = currentToken.Next;
                    tokens.Remove(currentToken);
                    currentToken = next;
                }
                else
                {
                    currentToken = currentToken.Next;
                }
            }
        }

        private object ConvertValue(string expressionValue, Type propertyType)
        {
            if (expressionValue.Equals("null", StringComparison.CurrentCultureIgnoreCase)) { return null; }

            Type t = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            var convertedValue = Convert.ChangeType(expressionValue, t);
            return convertedValue;
        }

        private static Expression AndAlso(Expression left, Expression right)
        {
            if (left == null)
            {
                return right;
            }
            return Expression.AndAlso(left, right);
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
