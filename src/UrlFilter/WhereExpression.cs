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
            _validator.ValidateQueryText(queryString);
            var parameterExpression = Expression.Parameter(typeof(T));

            Expression expression = reducer.ReduceLogical(queryString, parameterExpression);
            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        public Expression<Func<T, bool>> FromString<T>(string queryString, Expression<Func<T, bool>> expression) where T : class
        {
            throw new NotImplementedException();
        }

        //public Expression<Func<T, bool>> FromString<T>(string queryString, ParameterExpression parameterExpression, IDictionary<string, Func<string, object, Expression>> customExpressions) where T : class

        private static ILogicalReducer BuildLogicalReducer()
        {
            var propertyInfo = new PropertyInfoProvider();
            return new LogicalReducer(
                new ComparisonReducer(
                    new ComparisonProcessor(),
                    new PropertyProcessor(propertyInfo, new PropertyExpressionFactory(propertyInfo)),
                    new ValueProcessor()),
                new UnaryProcessor(),
                new LogicalProcessor());
        }
    }
}
