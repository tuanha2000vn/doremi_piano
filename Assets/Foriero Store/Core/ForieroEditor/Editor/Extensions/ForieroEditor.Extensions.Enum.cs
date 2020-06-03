using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ForieroEditor.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        public static bool IsObsolete(this Enum value, List<string> includes, List<string> excludes)
        {
            if (includes != null && includes.Contains(value.ToString()))
            {
                return true;
            }

            if (excludes != null && excludes.Contains(value.ToString()))
            {
                return false;
            }

            var enumType = value.GetType();
            //var enumName = enumType.GetEnumName(value);
            var enumName = value.ToString();
            var fieldInfo = enumType.GetField(enumName);
            return Attribute.IsDefined(fieldInfo, typeof(ObsoleteAttribute));
        }

        //		public static T SetFlag <T> (this T a, T b) where T : struct, IConvertible
        //		{
        //			if (!typeof(T).IsEnum) {
        //				Debug.LogError ("T must be an enumerated type");
        //			}
        //
        //			return a | b;
        //		}
        //
        //		public static T UnsetFlag <T> (this T a, T b) where T : struct, IConvertible
        //		{
        //			if (!typeof(T).IsEnum) {
        //				Debug.LogError ("T must be an enumerated type");
        //			}
        //
        //			return a & (~b);
        //		}

        // works well with 'none'
        public static bool HasFlag<T>(this T a, T b) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                Debug.LogError("T must be an enumerated type");
            }

            var aValue = Convert.ToInt64(a);
            var bValue = Convert.ToInt64(b);

            return (aValue & bValue) == bValue;
        }

        //		public static T ToogleFlag <T> (this T a, T b) where T : struct, IConvertible
        //		{
        //			if (!typeof(T).IsEnum) {
        //				Debug.LogError ("T must be an enumerated type");
        //			}
        //
        //			var aValue = Convert.ToUInt64 (a);
        //			var bValue = Convert.ToUInt64 (b);
        //
        //			return (aValue ^ bValue);
        //		}
    }
}
