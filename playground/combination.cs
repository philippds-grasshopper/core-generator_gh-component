using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper;
using System;
using System.Linq;

public static class test
{
    static void Main()
    {
        PermuteTest();
    }

    static void PermuteTest()
    {
        const int itemCount = 1;
        const int stateCount = 100;

        Console.WriteLine($"Permute {stateCount} states over {itemCount} items\n");

        foreach (var perm in Count(itemCount, stateCount))
        {
            Console.WriteLine();

            foreach (var i in perm)
                Console.Write($"{i}, ");
        }

        Console.ReadLine();
    }
    
    /// <summary>
    /// 
    /// </summary>
    static IEnumerable<IEnumerable<int>> Count(int digits, int radix)
    {
        
        var curr = new int[digits]; // value array
        var n = Pow(radix, digits); // total number of permutations

        yield return curr;

        while (--n > 0L)            // loop through total number of permutation
        {
            int i = 0;

            // increment
            curr[i]++;

            // carry
            while (curr[i] == radix)
            {
                curr[i++] = 0;
                curr[i]++;
            }

            yield return curr;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    static long Pow(long x, long y)
    {
        long result = 1;

        for (long i = 0; i < y; i++)
            result *= x;

        return result;
    }
}