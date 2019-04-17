using System;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class PropertyProcessor : IPropertyProcessor
    {
        private readonly IProvidePropertyInfo propertyInfo;

        public PropertyProcessor(IProvidePropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        public bool CanProcess(string operand, ParameterExpression paramExpression)
        {
            var propInfo = propertyInfo.GetPropertyInfoFromPath(operand, paramExpression.Type);
            return propInfo != null;
        }

        public Expression Process(string segment, ParameterExpression paramExpression)
        {
            if (CanProcess(segment, paramExpression))
            {
                return GetPropertyExpression(segment, paramExpression);
            }
            throw new InvalidOperationException($"Property of name {segment} was not found on object of type {paramExpression.Type}");
        }

        private Expression GetPropertyExpression(string segment, ParameterExpression paramExpression)
        {
            var propInfo = propertyInfo.GetPropertyInfoFromPath(segment, paramExpression.Type);
            return Expression.Property(paramExpression, propInfo);
        }
    }
}
