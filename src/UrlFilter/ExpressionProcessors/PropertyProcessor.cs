using System;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class PropertyProcessor : IPropertyProcessor
    {
        private readonly IProvidePropertyInfo propertyInfo;
        private readonly IPropertyExpressionFactory propertyExpressionFactory;

        public PropertyProcessor(IProvidePropertyInfo propertyInfo, IPropertyExpressionFactory propertyExpressionFactory)
        {
            this.propertyInfo = propertyInfo;
            this.propertyExpressionFactory = propertyExpressionFactory;
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
                return propertyExpressionFactory.CreatePropertyExpression(segment, paramExpression);
            }
            throw new InvalidOperationException($"Property of name {segment} was not found on object of type {paramExpression.Type}");
        }        
    }
}
