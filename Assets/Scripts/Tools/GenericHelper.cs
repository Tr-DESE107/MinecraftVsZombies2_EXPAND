namespace Tools
{
    public static class GenericHelper
    {
        public static T ToGeneric<T>(this object value)
        {
            if (TryToGeneric<T>(value, out var result))
                return result;
            return default;
        }
        public static bool TryToGeneric<T>(this object value, out T result)
        {
            if (value is null)
            {
                result = default(T);
                return true;
            }
            if (value is T tProp)
            {
                result = tProp;
                return true;
            }
            if (TryConvertBaseType<T>(value, out var baseValue))
            {
                result = baseValue;
                return true;
            }
            result = default;
            return false;
        }
        private static bool TryConvertBaseType<T>(this object value, out T result)
        {
            var targetType = typeof(T);
            object resultValue = null;
            if (targetType == typeof(short))
            {
                switch (value)
                {
                    case byte byteValue:
                        resultValue = (short)byteValue;
                        break;
                }
            }
            else if (targetType == typeof(int))
            {
                switch (value)
                {
                    case byte byteValue:
                        resultValue = (int)byteValue;
                        break;
                    case short shortValue:
                        resultValue = (int)shortValue;
                        break;
                }
            }
            else if (targetType == typeof(long))
            {
                switch (value)
                {
                    case byte byteValue:
                        resultValue = (long)byteValue;
                        break;
                    case short shortValue:
                        resultValue = (long)shortValue;
                        break;
                    case int intValue:
                        resultValue = (long)intValue;
                        break;
                }
            }
            else if (targetType == typeof(float))
            {
                switch (value)
                {
                    case byte byteValue:
                        resultValue = (float)byteValue;
                        break;
                    case short shortValue:
                        resultValue = (float)shortValue;
                        break;
                    case int intValue:
                        resultValue = (float)intValue;
                        break;
                    case long longValue:
                        resultValue = (float)longValue;
                        break;
                }
            }
            else if (targetType == typeof(double))
            {
                switch (value)
                {
                    case byte byteValue:
                        resultValue = (double)byteValue;
                        break;
                    case short shortValue:
                        resultValue = (double)shortValue;
                        break;
                    case int intValue:
                        resultValue = (double)intValue;
                        break;
                    case long longValue:
                        resultValue = (double)longValue;
                        break;
                    case float floatValue:
                        resultValue = (double)floatValue;
                        break;
                }
            }


            if (resultValue is T res)
            {
                result = res;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}