using UnityEngine;
using System.Collections;
using System;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        public static T[] Populate<T>(this T[] array, T defaultVaue)
        {
            if (array == null)
                return array;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = defaultVaue;
            }
            return array;
        }

        public static T[] PopulateNullValues<T>(this T[] array, T defaultVaue)
        {
            if (array == null)
                return array;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                    array[i] = defaultVaue;
            }
            return array;
        }

        public static T[] Populate<T>(this T[] array, int count, T defaultVaue)
        {
            T[] result = (array == null || count == 0) ? new T[count] : array;
            if (count > 0)
                Array.Resize(ref result, count);
            for (int i = 0; i < count; i++)
            {
                result[i] = defaultVaue;
            }
            return result;
        }

        public static T[] PopulateNullValues<T>(this T[] array, int count, T defaultVaue)
        {
            T[] result = (array == null || count == 0) ? new T[count] : array;
            if (count > 0)
                Array.Resize(ref result, count);
            for (int i = 0; i < count; i++)
            {
                if (result[i] == null)
                    result[i] = defaultVaue;
            }
            return result;
        }

        public static T[] AddLast<T>(this T[] array, T aNewItem)
        {
            T[] result = (array == null) ? new T[0] : array;
            Array.Resize(ref result, result.Length + 1);
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }
            result[result.Length - 1] = aNewItem;
            return result;
        }

        public static T[] AddFirst<T>(this T[] array, T aNewItem)
        {
            T[] result = (array == null) ? new T[0] : array;
            Array.Resize(ref result, result.Length + 1);
            for (int i = 0; i < array.Length; i++)
            {
                result[i + 1] = array[i];
            }
            result[0] = aNewItem;
            return result;
        }

        public static void Log<T>(this T[] array)
        {
            if (array == null)
                return;
            for (int i = 0; i < array.Length; i++)
            {
                Debug.Log(array[i]);
            }
        }

        public static T[] Slice<T>(this T[] source, int index, int count)
        {
            if (source == null)
                return null;
            if (index < 0 || count < 0 || source.Length - index < count)
                throw new ArgumentException();
            T[] result = new T[count];
            Array.Copy(source, index, result, 0, count);
            return result;
        }

        public static T Random<T>(this T[] source)
        {
            if (source == null)
                return default(T);
            int i = UnityEngine.Random.Range(0, source.Length);
            return source[i];
        }
    }
}
