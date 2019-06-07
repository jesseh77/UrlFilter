using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class BaseBinaryProcessor
    {
        private readonly Dictionary<string, ExpressionType> expressionTypeMap;

        public BaseBinaryProcessor()
        {
            expressionTypeMap = AcceptedExpressionTypes();
        }

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

        public virtual Dictionary<string, ExpressionType> AcceptedExpressionTypes()
        {
            return new Dictionary<string, ExpressionType>();
        }
    }
}
