using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    public class WhereExpression : IFilterExpression
    {
        public static readonly IFilterExpression Build = new WhereExpression();
        private readonly ExpressionReducer _reducer;
        private readonly QueryValidator _validator;

        public WhereExpression()
        {
            var operators = new ExpressionOperator();
            _reducer = new ExpressionReducer(operators);
            _validator = new QueryValidator();
        }
        
        public Expression<Func<T,bool>> FromString<T>(string queryString) where T : class
        {
            _validator.ValidateQueryText(queryString);
            var parameterExpression = Expression.Parameter(typeof(T));

            var expression = _reducer.ProcessQueryText<T>(queryString, parameterExpression);
            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        //public Expression<Func<T, bool>> FromString<T>(string queryString, Expression<Func<T, bool>> expression) where T : class
        //{
        //    _validator.ValidateQueryText(queryString);
        //    var parameterExpression = expression.Parameters.First();

        //    var left = expression.Body;

        //    var tokens = _reducer.GetWhereSegments(queryString);
        //    var right = _reducer.ReduceGroupSegments(tokens, parameterExpression);

        //    var netExpression = Expression.And(left, right);
        //    return Expression.Lambda<Func<T, bool>>(netExpression, parameterExpression);
        //}

        //public Expression<Func<T, bool>> FromString<T>(string queryString, ParameterExpression parameterExpression, IDictionary<string, Func<string, object, Expression>> customExpressions) where T : class
        //{
        //    _validator.ValidateQueryText(queryString);
        //    var expression = _reducer.ProcessQueryText<T>(queryString, parameterExpression, customExpressions);
        //    return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        //}
    }
}
