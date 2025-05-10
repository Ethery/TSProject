using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IListExtensions 
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static IList<T> Shuffle<T>(this IList<T> aList)
    {
        int count = aList.Count;
        int lastIndex = count - 1;
        for (int i = 0; i < lastIndex; ++i) {
            int r = UnityEngine.Random.Range(i, count);
            T tmp = aList[i];
            aList[i] = aList[r];
            aList[r] = tmp;
        }

        return aList;
    } 
    
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static T[] Shuffle<T>(this T[] aList)
    {
        int count = aList.Length;
        int lastIndex = count - 1;
        for (int i = 0; i < lastIndex; ++i) {
            int r = UnityEngine.Random.Range(i, count);
            T tmp = aList[i];
            aList[i] = aList[r];
            aList[r] = tmp;
        }

        return aList;
    }
}
