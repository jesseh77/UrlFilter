using System;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public interface IValueProcessor
    {
        ConstantExpression Process(string value, Type propertyType);
    }
}