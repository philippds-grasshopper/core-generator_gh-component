using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper;
using System;
using System.Linq;

namespace core_generator
{
    class gen_core
    {
        private int core_min_width;
        private int core_min_height;
        public List<Point3d> locations;
        public DataTree<int> values;
        public Rectangle3d skin;
        private int max_core_count;
        private double efficiency;
        private double deviation;
        private bool allow_core_variation;
        private List<Rectangle3d> cores;
        private List<List<int>> valid_core_combinations;
        private List<List<int>> valid_core_locations;
        public DataTree<Rectangle3d> valid_cores;
        private double core_area;

        public gen_core(int core_min_width, int core_min_height, Rectangle3d skin, int max_core_count, double efficiency, double deviation, bool allow_core_variation)
        {
            Rhino.RhinoApp.WriteLine("INITIALIZED_CORES");
            this.core_min_width = core_min_width;
            this.core_min_height = core_min_height;
            this.skin = skin;
            this.max_core_count = max_core_count;
            this.efficiency = efficiency;
            this.deviation = deviation;
            this.allow_core_variation = allow_core_variation;

            this.locations = new List<Point3d>();
            this.values = new DataTree<int>();
            this.cores = new List<Rectangle3d>();
            this.valid_cores = new DataTree<Rectangle3d>();
            this.valid_core_combinations = new List<List<int>>();
            this.valid_core_locations = new List<List<int>>();
            this.core_area = this.skin.Area * this.efficiency;

            compute();
        }

        private void compute()
        {
            create_locations();
            if(this.deviation != 0.0 && this.allow_core_variation)
            {
                create_variant_cores();
                Rhino.RhinoApp.WriteLine("computed {0} variant cores", this.cores.Count);
            }
            else
            {                
                create_invariant_cores();
                Rhino.RhinoApp.WriteLine("computed {0} in-variant cores", this.cores.Count);
            }
            evaluate_core_combinations();
            //create_core_location_combinations();
            //create_core_location_combinations_1(this.max_core_count, this.locations.Count);

            place_cores();
            Rhino.RhinoApp.WriteLine("computed the cores");
        }

        private void create_locations()
        {
            for (int skin_width = 0; skin_width < this.skin.Width; skin_width++)
            {
                for (int skin_height = 0; skin_height < this.skin.Height; skin_height++)
                {
                    this.locations.Add(new Point3d(skin_width, skin_height, 0));
                }
            }
        }

        private void create_variant_cores()
        {
            for (int core_width = this.core_min_width; core_width <= this.skin.Width; core_width++)
            {
                for (int core_height = this.core_min_height; core_height <= this.skin.Height; core_height++)
                {
                    if(this.core_min_width * this.core_min_height <= core_width * core_height && this.core_area * (1.0 + deviation) >= core_width * core_height)
                    {
                        this.cores.Add(new Rectangle3d(Plane.WorldXY, core_width, core_height));
                    }
                }
            }
        }

        private void create_invariant_cores()
        {            
            if (this.core_area % this.max_core_count == 0)
            {
                double core_side = this.core_area / this.max_core_count;
                Rhino.RhinoApp.WriteLine("{0}", core_side);
                for (int i = 0; i < this.max_core_count; i++)
                {
                    this.cores.Add(new Rectangle3d(Plane.WorldXY, core_side, core_side));
                }
            }
        }

        private bool point_in_polygon(Point3d[] polygon, Point3d point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        private void replace_core_values(ref List<Point3d> locations, ref List<int> default_values, Rectangle3d core)
        {
            // points of rectangle
            Point3d[] poly = { core.Corner(0), core.Corner(1), core.Corner(2), core.Corner(3) };

            for(int i = 0; i < locations.Count; i++)
            {
                if(point_in_polygon(poly, locations[i]))
                {
                    default_values[i] = 0;
                }
            }
        }

        private List<List<int>> cull_duplicate_combinations(ref List<List<int>> TestData)
        {
            // Sort every list in the list 
            for (int i = 0; i < TestData.Count; i++)
                TestData[i].Sort();

            var sorted = TestData;

            // Iterating through the ordered list of list to spot the duplicates
            List<int> t = null;
            List<List<int>> culled_list = new List<List<int>>();
            int cpt = 1;
            foreach (var l in sorted)
            {
                if (t != null)  // do nothing for the very first list
                {
                    // in all other cases, compare list with the previous one
                    var a = t.SequenceEqual(l);
                    if (a) { cpt++; }
                    else   // if not, show the duplicates and restart counting
                    {
                        culled_list.Add(t);
                        cpt = 1;
                    }
                }
                t = l;
            }
            if (t != null)  // process the last element outside the loop
            {
                culled_list.Add(t);
            }

            return culled_list;
        }

        private void evaluate_core_combinations()
        {
            MultiCombinations all_combinations = new MultiCombinations(this.max_core_count);

            
            foreach (List<int> combination in all_combinations.combinations)
            {
                foreach (int c in combination)
                {
                    Rhino.RhinoApp.Write("core combination: {0}, ", c);
                }
                Rhino.RhinoApp.Write("\n");
            }
            

            // validate core combination
            foreach (List<int> combination in all_combinations.combinations)
            {                
                double area_sum = 0;
                Rhino.RhinoApp.Write("checking core area\n");
                for (int i = 0; i < combination.Count; i++)
                {
                    Rhino.RhinoApp.Write("adding area: {0}\n", this.cores[combination[i]].Area);
                    area_sum += this.cores[combination[i]].Area;
                }
                Rhino.RhinoApp.Write("core area: {0}\n", area_sum);
                if (area_sum >= this.core_area * (1.0 - deviation) && area_sum <= this.core_area * (1.0 + deviation))
                {
                    this.valid_core_combinations.Add(combination);
                    Rhino.RhinoApp.Write("adding core to valid list\n");
                }
            }
            //this.valid_core_combinations = cull_duplicate_combinations(ref this.valid_core_combinations);
        }

        /*
        private void create_core_location_combinations()
        {
            int core_count = this.max_core_count;

            List<int> single_combination = new List<int>();
            for(int i = 0; i < core_count; i++)
            {
                single_combination.Add(0);
            }

            this.valid_core_locations = new List<List<int>>();
            while(single_combination[0] < this.locations.Count)
            {
                List<int> temp = new List<int>(single_combination);
                this.valid_core_locations.Add(temp);
                single_combination[single_combination.Count - 1]++;

                for(int i = 0; i < single_combination.Count; i++)
                {
                    if(single_combination[i] == this.locations.Count - 1 && single_combination.Count > 1 && i > 0)
                    {
                        single_combination[i] = 0;
                        single_combination[i - 1]++;
                    }
                }
            }
            
            
            foreach(List<int> core_locations in this.valid_core_locations)
            {
                foreach(int index in core_locations)
                {
                    Rhino.RhinoApp.Write("{0} ", index);
                }
                Rhino.RhinoApp.Write("\n");
            }
            
        }


        
        void create_core_location_combinations_1(int core_count, int location_count)
        {
            var curr = new int[core_count]; // value array
            var n = Pow(location_count, core_count); // total number of permutations

            if(core_count == 1)
            {
                this.valid_core_locations.Add(new List<int>() { 0 });
            }

            while (--n > 0L)            // loop through total number of permutation
            {
                int i = 0;

                // increment
                curr[i]++;

                // carry
                while (curr[i] == location_count)
                {
                    curr[i++] = 0;
                    curr[i]++;
                }

                this.valid_core_locations.Add(curr.ToList());

                foreach(int g in curr)
                {
                    //Rhino.RhinoApp.WriteLine("{0}", g);
                }
                

            }
        }

        static long Pow(long x, long y)
        {
            long result = 1;

            for (long i = 0; i < y; i++)
                result *= x;

            return result;
        }

        private void place_cores()
        {            
            // sort cores and values into trees
            for (int i = 0; i < this.valid_core_combinations.Count; i++)
            {
                int index = 0;
                for (int j = 0; j < this.valid_core_locations.Count; j++)
                {

                    int test = 0;
                    for (int k = 0; k < this.max_core_count; k++)
                    {
                        
                        if (// if cores do not overlap the skin
                            (this.locations[valid_core_locations[j][k]].X + this.cores[this.valid_core_combinations[i][k]].Width) <= this.skin.Width &&
                            (this.locations[valid_core_locations[j][k]].Y + this.cores[this.valid_core_combinations[i][k]].Height) <= this.skin.Height)
                        {
                            //Rhino.RhinoApp.WriteLine("core not overlapping skin: {0}", j);
                            if (this.max_core_count > 1)
                            {
                                for (int l = 0; l < this.max_core_count; l++)
                                {
                                    if (// if k && l are not the same
                                        (k != l) &&
                                        // if cores do not overlap each other
                                        ((this.locations[valid_core_locations[j][k]].X >= this.locations[valid_core_locations[j][l]].X + this.cores[this.valid_core_combinations[i][l]].Width ||
                                        this.locations[valid_core_locations[j][k]].X + this.cores[this.valid_core_combinations[i][k]].Width <= this.locations[valid_core_locations[j][l]].X) ||
                                        (this.locations[valid_core_locations[j][k]].Y >= this.locations[valid_core_locations[j][l]].Y + this.cores[this.valid_core_combinations[i][l]].Height ||
                                        this.locations[valid_core_locations[j][k]].Y + this.cores[this.valid_core_combinations[i][k]].Height <= this.locations[valid_core_locations[j][l]].Y))
                                        // if distance to corners is not smaller then escape distance
                                        )
                                    {
                                        test++;
                                    }
                                }
                            }
                            else
                            {
                                test++;
                            }
                        }
                    }

                    if ((test == Math.Pow(this.max_core_count, 2) - this.max_core_count) && (this.max_core_count != 1) || ((test == this.max_core_count) && (this.max_core_count == 1)))
                    {
                        //Rhino.RhinoApp.WriteLine("valid core {0}", j);
                        this.valid_cores.EnsurePath(new int[] { i, index });
                        this.values.EnsurePath(new int[] { i, index });
                        index++;

                        List<int> default_values = new List<int>();
                        for (int m = 0; m < this.locations.Count; m++)
                        {
                            default_values.Add(1);
                        }

                        int p = 0;
                        foreach (int c in this.valid_core_combinations[i])
                        {
                            int vcl_index = this.valid_core_locations[j][p];
                            //Rhino.RhinoApp.WriteLine("valid core location index: {0}", vcl_index);
                            Rectangle3d core = new Rectangle3d(Plane.WorldXY, new Point3d(this.locations[vcl_index]), new Point3d(this.locations[vcl_index].X + this.cores[c].Width, this.locations[vcl_index].Y + this.cores[c].Height, 0));
                            this.valid_cores.Add(core);
                            replace_core_values(ref this.locations, ref default_values, core);
                            p++;
                        }
                        this.values.AddRange(default_values);
                    }
                }
            }
        }
        */

        /// <summary>
        /// 
        /// </summary>
        static IEnumerable<IEnumerable<int>> Count(int digits, int radix)
        {
            var curr = new int[digits];
            var n = Pow(radix, digits);

            while (--n > 0L)
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

        private void place_cores()
        {
            Rhino.RhinoApp.WriteLine("doing something_0");
            Rhino.RhinoApp.WriteLine("combination count: {0}", this.valid_core_combinations.Count);

            // sort cores and values into trees
            for (int i = 0; i < this.valid_core_combinations.Count; i++)
            {
                Rhino.RhinoApp.WriteLine("doing something_1");
                int index = 0;                    

                foreach (var perm in Count(this.locations.Count, this.max_core_count))
                {
                    Rhino.RhinoApp.WriteLine("doing something_2");
                    int test = 0;


                    for (int k = 0; k < this.max_core_count; k++)
                    {
                        if (// if cores do not overlap the skin
                        (this.locations[perm.ToArray()[k]].X + this.cores[this.valid_core_combinations[i][k]].Width) <= this.skin.Width &&
                        (this.locations[perm.ToArray()[k]].Y + this.cores[this.valid_core_combinations[i][k]].Height) <= this.skin.Height)
                        {
                            //Rhino.RhinoApp.WriteLine("core not overlapping skin: {0}", j);
                            if (this.max_core_count > 1)
                            {
                                for (int l = 0; l < this.max_core_count; l++)
                                {
                                    if (// if k && l are not the same
                                        (k != l) &&
                                        // if cores do not overlap each other
                                        ((this.locations[perm.ToArray()[k]].X >= this.locations[perm.ToArray()[l]].X + this.cores[this.valid_core_combinations[i][l]].Width ||
                                        this.locations[perm.ToArray()[k]].X + this.cores[this.valid_core_combinations[i][k]].Width <= this.locations[perm.ToArray()[l]].X) ||
                                        (this.locations[perm.ToArray()[k]].Y >= this.locations[perm.ToArray()[l]].Y + this.cores[this.valid_core_combinations[i][l]].Height ||
                                        this.locations[perm.ToArray()[k]].Y + this.cores[this.valid_core_combinations[i][k]].Height <= this.locations[perm.ToArray()[l]].Y))
                                        // if distance to corners is not smaller then escape distance
                                        )
                                    {
                                        test++;
                                    }
                                }
                            }
                            else
                            {
                                test++;
                            }
                        }
                    }
                    
                    for (int k = 0; k < this.max_core_count; k++)
                    {


                    }

                    Rhino.RhinoApp.WriteLine("doing something");

                    if ((test == Math.Pow(this.max_core_count, 2) - this.max_core_count) && (this.max_core_count != 1) || ((test == this.max_core_count) && (this.max_core_count == 1)))
                    {
                        //Rhino.RhinoApp.WriteLine("valid core {0}", j);
                        this.valid_cores.EnsurePath(new int[] { i, index });
                        this.values.EnsurePath(new int[] { i, index });
                        index++;

                        List<int> default_values = new List<int>();
                        for (int m = 0; m < this.locations.Count; m++)
                        {
                            default_values.Add(1);
                        }

                        int p = 0;
                        foreach (int c in this.valid_core_combinations[i])
                        {
                            int vcl_index = perm.ToArray()[p];
                            //Rhino.RhinoApp.WriteLine("valid core location index: {0}", vcl_index);
                            Rectangle3d core = new Rectangle3d(Plane.WorldXY, new Point3d(this.locations[vcl_index]), new Point3d(this.locations[vcl_index].X + this.cores[c].Width, this.locations[vcl_index].Y + this.cores[c].Height, 0));
                            this.valid_cores.Add(core);
                            replace_core_values(ref this.locations, ref default_values, core);
                            p++;
                        }
                        this.values.AddRange(default_values);
                    }
                }
            }
        }
    }
}
