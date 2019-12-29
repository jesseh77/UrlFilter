using System;
using System.Reflection;

namespace UrlFilter
{
    public interface IProvidePropertyInfo
    {
        PropertyInfo GetPropertyInfoFromName(string propertyName, Type type);
        PropertyInfo GetPropertyInfoFromPath(string propertyPath, Type type);
    }
}
