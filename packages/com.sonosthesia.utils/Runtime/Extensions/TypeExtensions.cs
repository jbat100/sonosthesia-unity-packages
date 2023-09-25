using System;
using System.Reflection;

namespace Sonosthesia.Utils
{
    public static class TypeExtensions
    {
        public static FieldInfo GetFieldInHierarchy(this Type type, string name, BindingFlags attributes)
        {
            FieldInfo fieldInfo = null;
            
            while (type != null && fieldInfo == null)
            {
                fieldInfo = type.GetField(name, attributes);
                type = type.BaseType;
            }

            return fieldInfo;
        }
    }
}