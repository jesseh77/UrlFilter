using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class ComparisonProcessor
    {
        private readonly Dictionary<string, ExpressionType> expressionTypeMap = getTypeMap();
        public bool CanProcess(string comparisonType)
        {
            if (string.IsNullOrWhiteSpace(comparisonType)) return false;
            return expressionTypeMap.ContainsKey(comparisonType.ToLower());
        }

        public Expression Process(string comparisonType, Expression leftExpression, Expression rightExpression)
        {
            if (!CanProcess(comparisonType)) throw new InvalidOperationException($"Comparison type of {comparisonType} is not supported");
            var expressionType = expressionTypeMap[comparisonType.ToLower()];
            return Expression.MakeBinary(expressionType, leftExpression, rightExpression);
        }

        private static Dictionary<string, ExpressionType> getTypeMap()
        {
            return new Dictionary<string, ExpressionType>
            {
                {"eq", ExpressionType.Equal},
                {"gt", ExpressionType.GreaterThan},
                {"ge", ExpressionType.GreaterThanOrEqual},
                {"lt", ExpressionType.LessThan},
                {"le", ExpressionType.LessThanOrEqual},
                {"ne", ExpressionType.NotEqual}
            };
        }
    }
}
