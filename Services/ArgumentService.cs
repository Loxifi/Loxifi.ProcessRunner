using Loxifi.Attributes;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Loxifi.Services
{
    public static class ArgumentService
    {
        public static string Parse(object argument)
        {
            if (argument is null)
            {
                return string.Empty;
            }

            if (argument is string s)
            {
                return s;
            }

            if (argument is IEnumerable e)
            {
                return string.Join(" ", e.Cast<object>().Select(Parse));
            }

            Type aType = argument.GetType();

            if (aType.IsPrimitive)
            {
                return argument.ToString();
            }

            StringBuilder stringBuilder = new();

            PropertyInfo[] argumentProperties = aType.GetProperties();

            for (int i = 0; i < argumentProperties.Length; i++)
            {
                PropertyInfo propertyInfo = argumentProperties[i];

                if (propertyInfo.GetGetMethod() is null)
                {
                    continue;
                }

                if (propertyInfo.GetCustomAttribute<ParameterAttribute>() is ParameterAttribute argumentAttribute)
                {
                    object propertyValue = propertyInfo.GetValue(argument);

                    string serializedValue = argumentAttribute.Serialize(propertyValue);

                    if (!string.IsNullOrWhiteSpace(serializedValue))
                    {
                        if (i > 0)
                        {
                            stringBuilder.Append(' ');
                        }

                        stringBuilder.Append(serializedValue);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}