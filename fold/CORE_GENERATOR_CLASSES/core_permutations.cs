using System;
using System.Collections.Generic;
using System.Linq;

namespace core_generator
{
    public class MultiCombinations
    {
        private int max_core_count;
        public List<List<int>> combinations;

        public MultiCombinations(int max_core_count)
        {
            this.max_core_count = max_core_count;
            this.combinations = new List<List<int>>();
            compute();
        }

        private void compute()
        {
            var set = Enumerable.Range(0, this.max_core_count).ToList();
            var combinations = GenerateCombinations(set, this.max_core_count);
            this.combinations = combinations;
        }

        private static List<List<T>> GenerateCombinations<T>(List<T> combinationList, int k)
        {
            var combinations = new List<List<T>>();

            if (k == 0)
            {
                var emptyCombination = new List<T>();
                combinations.Add(emptyCombination);

                return combinations;
            }

            if (combinationList.Count == 0)
            {
                return combinations;
            }

            T head = combinationList[0];
            var copiedCombinationList = new List<T>(combinationList);

            List<List<T>> subcombinations = GenerateCombinations(copiedCombinationList, k - 1);

            foreach (var subcombination in subcombinations)
            {
                subcombination.Insert(0, head);
                combinations.Add(subcombination);
            }

            combinationList.RemoveAt(0);
            combinations.AddRange(GenerateCombinations(combinationList, k));

            return combinations;
        }
    }
}
