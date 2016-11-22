using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter
{
    public static class ExpressionOperators
    {
        private static Dictionary<string, Func<Expression, Expression, Expression>> _expressions = GetExpressions();
        public static Expression OperatorExpression(string operation, Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        private static Dictionary<string, Func<Expression, Expression, Expression>> GetExpressions()
        {
            return new Dictionary<string, Func<Expression, Expression, Expression>>
            {
                {"eq", Expression.Equal },
                {"ne", Expression.NotEqual },
                {"gt", Expression.GreaterThan },
                {"ge", Expression.GreaterThanOrEqual },
                {"lt", Expression.LessThan },
                {"le", Expression.LessThanOrEqual },
                {"or", Expression.OrElse },
                { "and", Expression.AndAlso }
            };
        }
    }
}
