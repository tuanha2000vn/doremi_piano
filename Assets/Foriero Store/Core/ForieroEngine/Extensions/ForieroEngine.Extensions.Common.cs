using UnityEngine;
using System.Collections;
using System;

#if NETFX_CORE
using System.Reflection;
#endif

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
        public static bool IsEnum(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        public static bool IsAssignableFrom(this Type first, Type second)
        {
#if NETFX_CORE
            return first.GetTypeInfo().IsAssignableFrom(second.GetTypeInfo());
#else
            return first.IsAssignableFrom(second);
#endif
        }
    }
}
