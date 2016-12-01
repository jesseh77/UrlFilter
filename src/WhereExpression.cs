using System;
using System.Linq.Expressions;

namespace UrlFilter
{
    public class WhereExpression
    {
        public static WhereExpression Build = new WhereExpression();
        
        public Expression<Func<T,bool>> FromString<T>(string queryString) where T : class
        {
            var tokens = QueryStringProcessor.GetWhereSegments(queryString);
            var parameterExpression = Expression.Parameter(typeof(T));

            var expression = ExpressionReducer.ReduceGroupSegments<T>(tokens, parameterExpression);
            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }
    }
}
