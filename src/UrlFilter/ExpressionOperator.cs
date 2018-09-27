using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter
{
    internal class ExpressionOperator
    {
        private static readonly Dictionary<string, Func<Expression, Expression, Expression>> Expressions = GetExpressions();
        public Expression OperatorExpression(string operation, Expression left, Expression right)
        {
            if (CanProcessOperand(operation))
            {
                var key = operation.Trim().ToLowerInvariant();
                return Expressions[key](left, right);
            }
            
            throw new QueryStringException($"Filter of type '{operation}' is not a valid query string operator");
        }

        public bool CanProcessOperand(string operand)
        {
            if (string.IsNullOrWhiteSpace(operand)) { return false; }

            var key = operand.Trim().ToLowerInvariant();
            return Expressions.ContainsKey(key);
        }

        private static Dictionary<string, Func<Expression, Expression, Expression>> GetExpressions()
        {
            return new Dictionary<string, Func<Expression, Expression, Expression>>
            {
                {"eq", Expression.Equal},
                {"ne", Expression.NotEqual},
                {"gt", Expression.GreaterThan},
                {"ge", Expression.GreaterThanOrEqual},
                {"lt", Expression.LessThan},
                {"le", Expression.LessThanOrEqual},
                {"or", Expression.OrElse},
                {"and", Expression.AndAlso}
            };
        }
    }
}