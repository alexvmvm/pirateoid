using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListUtils
{ 

    public static T RandomElement<T>(this IEnumerable<T> elements)
    {
        var count = elements.Count();

        if(count <= 0)
        {
            Debug.LogError("Tried to find a random element of an empty list.");
            return default(T);
        }

        var index = UnityEngine.Random.Range(0, count);

        return elements.ElementAt(index);
    }

    public static T RandomElementWithFallback<T>(this IEnumerable<T> elements)
    {
        var count = elements.Count();

        if(count <= 0)
            return default;

        var index = UnityEngine.Random.Range(0, count);

        return elements.ElementAt(index);
    }

    public static T RandomElementWithWeight<T>(this IEnumerable<T> elements, Func<T, float> selector)
    {
        float totalWeight = 0f;
        for(var i = 0; i < elements.Count(); i++)
        {
            totalWeight += selector(elements.ElementAt(i));
        }

        float randomNumber = UnityEngine.Random.Range(0, totalWeight);

        for(var i = 0; i < elements.Count(); i++)
        {
            var item = elements.ElementAt(i);
            
            if (randomNumber < selector(item))
            {
                return item;
            }

            randomNumber = randomNumber - selector(item);
        }
        
        return default;
    }


    public static bool TryRandomElementByWeight<T>(this IEnumerable<T> elements, Func<T, float> selector, out T result)
    {
        float totalWeight = 0f;
        for(var i = 0; i < elements.Count(); i++)
        {
            totalWeight += selector(elements.ElementAt(i));
        }

        float randomNumber = UnityEngine.Random.Range(0, totalWeight);

        for(var i = 0; i < elements.Count(); i++)
        {
            var item = elements.ElementAt(i);
            
            if (randomNumber < selector(item))
            {
                result = item;
                return true;
            }

            randomNumber = randomNumber - selector(item);
        }
        
        result = default(T);

        return false;
    }

    public static bool TryRandomElementByWeight<T>(this IEnumerable<T> elements, int seed, Func<T, float> selector, out T result)
    {
        float totalWeight = 0f;
        for(var i = 0; i < elements.Count(); i++)
        {
            totalWeight += selector(elements.ElementAt(i));
        }

        // note: potentially slow, not checked this
        UnityEngine.Random.InitState(seed);

        float randomNumber = UnityEngine.Random.Range(0, totalWeight);

        for(var i = 0; i < elements.Count(); i++)
        {
            var item = elements.ElementAt(i);
            if (randomNumber < selector(item))
            {
                result = item;
                return true;
            }

            randomNumber -= selector(item);
        }
        
        result = default(T);

        return false;
    }

    public static bool NullOrEmpty<T>(this IEnumerable<T> list)
    {
        return list == null || list.Count() == 0;
    }

    public static bool NotNullAndContains<T>(this IEnumerable<T> list, T item)
    {
        return list != null && list.Contains(item);
    }

    public static IEnumerable<T> InRandomOrder<T>(this IEnumerable<T> source)
    {
        return source.OrderBy<T, int>((item) => UnityEngine.Random.Range(int.MinValue, int.MaxValue));
    }

    public static void SortBy<T, U>(this List<T> list, Func<T, U> selector) where U : IComparable<U>
    {
        if(list.Count <= 1)
            return;

        list.Sort((a, b) => selector(a).CompareTo(selector(b)));
    }
    public static void SortByDescending<T, U>(this List<T> list, Func<T, U> selector) where U : IComparable<U>
    {
        if(list.Count <= 1)
            return;

        list.Sort((a, b) => selector(b).CompareTo(selector(a)));
    }

}