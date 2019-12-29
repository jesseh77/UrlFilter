using System;
using System.Reflection;

namespace UrlFilter
{
    public class PropertyInfoProvider : IProvidePropertyInfo
    {
        public PropertyInfo GetPropertyInfoFromPath(string propertyPath, Type type)
        {
            var pathParts = propertyPath.Split('.');
            PropertyInfo propInfo = null;

            foreach (var path in pathParts)
            {
                propInfo = GetPropertyInfoFromName(path, type);
                if (propInfo == null) return null;
            }

            return propInfo;
        }

        public PropertyInfo GetPropertyInfoFromName(string propertyName, Type type)
        {
            var propsInfo = type.GetRuntimeProperties();
            foreach (var prop in propsInfo)
            {
                if (prop.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) return prop;
            }
            return null;
        }
    }
}
