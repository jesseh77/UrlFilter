using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace UrlFilter.ExpressionProcessors
{
    public class PropertyProcessor
    {
        public ExpressionCategory ExpressionCategory => ExpressionCategory.Property;

        public PropertyProcessor()
        {

        }

        public bool CanProcess(string operand, ParameterExpression paramExpression)
        {
            var propInfo = getPropertyInfoFromPath(operand, paramExpression.Type);
            return propInfo != null;
        }

        private PropertyInfo getPropertyInfoFromPath(string propertyPath, Type type)
        {
            var pathParts = propertyPath.Split('.');
            PropertyInfo propInfo = null;

            foreach (var path in pathParts)
            {
                propInfo = getPropertyInfoFromName(path, type);
                if (propInfo == null) return null;
            }

            return propInfo;
        }

        private PropertyInfo getPropertyInfoFromName(string propertyName, Type type)
        {
            var propsInfo = type.GetRuntimeProperties();
            return propsInfo.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }

        public void Process(string segment, ParameterExpression paramExpression)
        {
            if (CanProcess(segment, paramExpression))
            {
                return;
            }
            throw new InvalidOperationException($"Property of name {segment} was not found on object of type {paramExpression.Type}");
        }
    }
}
