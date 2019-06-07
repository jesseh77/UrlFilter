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
            var nullExpression = Expression.Constant(null);
            var member = leftExpression as MemberExpression ?? rightExpression as MemberExpression;
            Expression expression = null;
            while (member.Member != null)
            {
                if(expression is null)
                {
                    expression = Expression.MakeBinary(ExpressionType.NotEqual, member, nullExpression);
                }
                else
                {
                    var notNullExpression = Expression.MakeBinary(ExpressionType.NotEqual, member, nullExpression);
                    expression = Expression.MakeBinary(ExpressionType.AndAlso, expression, notNullExpression);
                }
                //member = member.Member;
            }
           
            return Expression.AndAlso(expression, Expression.MakeBinary(expressionType, leftExpression, rightExpression));
        }

        public virtual Dictionary<string, ExpressionType> AcceptedExpressionTypes()
        {
            return new Dictionary<string, ExpressionType>();
        }
    }
}
