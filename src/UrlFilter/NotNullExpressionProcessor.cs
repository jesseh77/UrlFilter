using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter
{
    public class NotNullExpressionProcessor
    {
        private readonly IPropertyExpressionFactory propertyExpressionFactory;

        public NotNullExpressionProcessor(IPropertyExpressionFactory propertyExpressionFactory)
        {
            this.propertyExpressionFactory = propertyExpressionFactory;
        }
        public Expression NotNullPropertyExpression(string segment, ParameterExpression paramExpression)
        {
            Expression netExpression = null;
            var nullValueExpression = Expression.Constant(null);

            foreach (var index in GetIndexesOf(segment, '.'))
            {
                var propExpression = propertyExpressionFactory.CreatePropertyExpression(segment.Substring(0, index), paramExpression);
                var notEqualExpression = Expression.NotEqual(propExpression, nullValueExpression);
                
                netExpression = netExpression is null ? notEqualExpression : Expression.AndAlso(netExpression, notEqualExpression);
            }
            return netExpression;
        }

        private IEnumerable<int> GetIndexesOf(string text, char delimiter)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Equals(delimiter))
                {
                    yield return i;
                }
            }
        }
    }
}
