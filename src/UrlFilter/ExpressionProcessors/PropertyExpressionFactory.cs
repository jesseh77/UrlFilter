using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class PropertyExpressionFactory : IPropertyExpressionFactory
    {
        private readonly IProvidePropertyInfo propertyInfoProvider;

        public PropertyExpressionFactory(IProvidePropertyInfo propertyInfoProvider)
        {
            this.propertyInfoProvider = propertyInfoProvider;
        }
        public Expression CreatePropertyExpression(string segment, ParameterExpression paramExpression)
        {
            var segments = segment.Split('.');
            Expression propertyExpression = null;
            var propertyType = paramExpression.Type;
            foreach (var prop in segments)
            {
                var propInfo = propertyInfoProvider.GetPropertyInfoFromPath(prop, propertyType);
                propertyExpression = Expression.Property(propertyExpression ?? paramExpression, propInfo);
            }

            return propertyExpression;
        }
    }
}
