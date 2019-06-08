using System;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;
using UrlFilter.ExpressionReducers;

namespace UrlFilter
{
    public class WhereExpression : IFilterExpression
    {
        public static readonly IFilterExpression Build = new WhereExpression();
        public static readonly ILogicalReducer reducer = BuildLogicalReducer();

        

        private readonly QueryValidator _validator;

        public WhereExpression()
        {
            _validator = new QueryValidator();
        }
        
        public Expression<Func<T,bool>> FromString<T>(string queryString) where T : class
        {            
            var parameterExpression = Expression.Parameter(typeof(T));
            var expression = createExpression(queryString, parameterExpression);
            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        private Expression createExpression(string queryString, ParameterExpression parameterExpression)
        {
            _validator.ValidateQueryText(queryString);
            return reducer.ReduceLogical(queryString, parameterExpression);
        }

        public Expression<Func<T, bool>> FromString<T>(string queryString, Expression<Func<T, bool>> expression) where T : class
        {
            var parameterExpression = expression.Parameters[0];
            var queryExpression = createExpression(queryString, parameterExpression);
            var netExpression = Expression.AndAlso(expression.Body, queryExpression);
            return Expression.Lambda<Func<T,bool>>(netExpression, parameterExpression);
        }

        private static ILogicalReducer BuildLogicalReducer()
        {
            var propertyInfo = new PropertyInfoProvider();
            var propertyExpFac = new PropertyExpressionFactory(propertyInfo);
            return new LogicalReducer(
                new ComparisonReducer(
                    new ComparisonProcessor(),
                    new PropertyProcessor(propertyInfo, propertyExpFac),
                    new ValueProcessor(),
                    new NotNullExpressionProcessor(propertyExpFac)),
                new UnaryProcessor(),
                new LogicalProcessor());
        }
    }
}
