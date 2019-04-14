using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UrlFilter.ExpressionProcessors
{
    public class ValueProcessor
    {
        public ConstantExpression Process(string value, PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            if (!isValueType(type)) throw new NotImplementedException($"Type {type.Name} is not valid for a value expression");

            var trimValue = StripOuterSingleQuotes(value);
            var convertedValue = ConvertValue(trimValue, type);
            return Expression.Constant(convertedValue, type);
        }

        private object ConvertValue(string expressionValue, Type propertyType)
        {
            if (expressionValue.Equals("null", StringComparison.CurrentCultureIgnoreCase)) { return null; }

            Type t = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            var convertedValue = Convert.ChangeType(expressionValue, t);
            return convertedValue;
        }

        private static string StripOuterSingleQuotes(string value)
        {
            if (value[0] == '\'' && value[value.Length - 1] == '\'')
            {
                return value.Substring(1, value.Length - 2);
            }
            return value;
        }
        
        private bool isValueType(Type type)
        {
            return type.GetTypeInfo().IsValueType || type.Equals(typeof(string)) || type.Equals(typeof(DateTime));
        }
    }
}
