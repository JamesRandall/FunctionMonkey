using System;
using System.Linq;
using System.Reflection;

namespace FunctionMonkey.Compiler.Core.Extensions
{
    public static class PropertyInfoExtension
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this PropertyInfo propertyInfo,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            return propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att ? valueSelector(att) : default(TValue);
        }
    }
}
