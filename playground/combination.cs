using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper;
using System;
using System.Linq;

public static class test
{ static void Main()
    {
        //List<List<int>> list = new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 0, 2 }, new List<int> { 1, 0 }, new List<int> { 3, 1 } };
        //List<List<int>> modified = new List<List<int>>();


        List<List<int>> dist(List<List<int>> input)
        {
            var returnListList = new List<List<int>>();
            var counts = new List<int>(input.Count);

            foreach (var l in input)
            {
                l.Sort();
                counts.Add(l.Count);
            }

            var alreadyMatched = new HashSet<int>();
            int count = 0;
            int lastEval = 0;

            for (int i = 0; i < input.Count - 1; i++)
            {
                if (alreadyMatched.Contains(i))
                {
                    continue;
                }

                returnListList.Add(input[i]);
                count = 1;
                lastEval = 0;

                var current = input[i];

                for (int j = i + 1; j < input.Count; j++)
                {
                    if (alreadyMatched.Contains(j))
                    {
                        continue;
                    }

                    lastEval = j;

                    if (counts[i] != counts[j])
                    {
                        continue;
                    }

                    var compareAgainst = input[j];

                    if (AreSame(current, compareAgainst))
                    {
                        count++;
                        alreadyMatched.Add(j);
                    }
                }
            }

            if (count == 1 && lastEval > 0)
            {
                returnListList.Add(input[lastEval]);
            }

            return returnListList;
        }


        bool AreSame(List<int> first, List<int> second)
        {
            // The Count properties of the lists are already checked and the same
            for (int k = 0; k < first.Count; k++)
            {
                if (first[k] != second[k])
                {
                    return false;
                }
            }
            return true;
        }


        int core_count = 1;

        List<int> single_combination = new List<int>();
        for (int i = 0; i < core_count; i++)
        {
            single_combination.Add(0);
        }
        
        List<Point3d> locations = new List<Point3d>();
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                locations.Add(new Point3d(i, j, 0));
            }
        }
        
        List<List<int>> valid_core_locations = new List<List<int>>();
        Console.Write("single combination {0}\n",single_combination[0]);
        Console.Write("location count {0}\n", locations.Count);
                
        while (single_combination[0] < locations.Count)
        {
            Console.Write("current combination {0}\n", single_combination[0]);
            List<int> temp = new List<int>(single_combination);
            valid_core_locations.Add(temp);
            Console.Write("incrementing index: {0}\n", single_combination.Count - 1);
            single_combination[single_combination.Count - 1]++;

            /*
            for (int i = 0; i < single_combination.Count; i++)
            {
                if (single_combination[i] == locations.Count - 1 && single_combination.Count > 1 && i > 0)
                {
                    single_combination[i] = 0;
                    single_combination[i - 1]++;
                }
            }
            */
        }
                
        foreach (List<int> l in valid_core_locations)
        {
            foreach (int item in l)
            {
                Console.Write(item);
            }
            Console.Write("\n");
        }

        /*

        
        modified = dist(list);

        foreach(List<int> l in modified)
        {
            foreach(int item in l)
            {
                Console.Write(item);
            }
            Console.Write("\n");
        }
        */



        void Permute(List<int> perm, int states, int count, List<List<int>> result)
        {
            if (count == 0)
                return;

            perm.Add(0);
            Permute(perm, states, count - 1, result);

            for (int i = 1; i < states; i++)
            {
                var copy = new List<int>(perm);
                copy.Add(i);

                result.Add(copy);
                Permute(copy, states, count - 1, result);
            }
        }




        void Permute2(HashSet<int> perm, int states, int count, List<HashSet<int>> result)
        {
            if (count == 0)
                return;

            if(perm.Add(0))
                Permute2(perm, states, count - 1, result);

            for (int i = 1; i < states; i++)
            {
                if (perm.Contains(i)) continue;

                var copy = new HashSet<int>(perm);
                copy.Add(i);

                result.Add(copy);
                Permute2(copy, states, count - 1, result);
            }
        }
    }
}