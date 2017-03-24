using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UrlFilter.ExpressionProcessors
{
    class ValueProcessor : IExpressionProcessor
    {
        private readonly ExpressionOperator _operators;
        private readonly List<string> _operands;

        public ValueProcessor(List<string> operands, ExpressionOperator operators)
        {
            _operands = operands;
            _operators = operators;
        }

        public bool canProcess(string operand)
        {
            if (string.IsNullOrWhiteSpace(operand)) return false;
            return _operands.Contains(operand.ToLower());
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (canProcess(current.Next.Value.TokenValue))
                {
                    var propertyName = current.Value.TokenValue;

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

                    var expressionValue = StripOuterSingleQuotes(current.Next.Next.Value.TokenValue);

                    var propValue = Convert.ChangeType(expressionValue, propertyInfo.PropertyType);
                    var rightExpression = Expression.Constant(propValue);

                    var operationType = current.Next.Value.TokenValue;
                    var tailExpression = _operators.OperatorExpression(operationType, parameterExpression, rightExpression);

                    if (current.Next.Value.IsNot)
                    {
                        tailExpression = Expression.Not(tailExpression);
                    }

                    returnExpression = AndAlso(returnExpression, tailExpression);
                    current.Next.Value.OperatorExpression = returnExpression;

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
