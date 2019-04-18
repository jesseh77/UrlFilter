using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    public class WhereExpression : IFilterExpression
    {
        public static readonly IFilterExpression Build = new WhereExpression();
        private readonly QueryValidator _validator;

        public WhereExpression()
        {
            _validator = new QueryValidator();
        }
        
        public Expression<Func<T,bool>> FromString<T>(string queryString) where T : class
        {
            _validator.ValidateQueryText(queryString);
            var parameterExpression = Expression.Parameter(typeof(T));

            Expression expression = null; //get the expression
            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        public Expression<Func<T, bool>> FromString<T>(string queryString, Expression<Func<T, bool>> expression) where T : class
        {
            return null;
        }

        //public Expression<Func<T, bool>> FromString<T>(string queryString, ParameterExpression parameterExpression, IDictionary<string, Func<string, object, Expression>> customExpressions) where T : class
    }
}
