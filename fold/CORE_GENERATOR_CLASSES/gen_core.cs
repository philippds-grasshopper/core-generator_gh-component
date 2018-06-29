using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper;

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
        private List<Rectangle3d> cores;
        private List<List<int>> valid_core_combinations;
        public DataTree<Rectangle3d> valid_cores;

        public gen_core(int core_min_width, int core_min_height, Rectangle3d skin, int max_core_count, double efficiency, double deviation)
        {
            Rhino.RhinoApp.WriteLine("INITIALIZED_CORES");
            this.core_min_width = core_min_width;
            this.core_min_height = core_min_height;
            this.skin = skin;
            this.max_core_count = max_core_count;
            this.efficiency = efficiency;
            this.deviation = deviation;

            this.locations = new List<Point3d>();
            this.values = new DataTree<int>();
            this.cores = new List<Rectangle3d>();
            this.valid_cores = new DataTree<Rectangle3d>();

            compute();
        }

        private void compute()
        {
            create_locations();
            create_cores();
            evaluate_core_combinations();
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

        private void create_cores()
        {
            double core_area = this.skin.Area * this.efficiency;

            for (int core_width = this.core_min_width; core_width <= this.skin.Width; core_width++)
            {
                for (int core_height = this.core_min_height; core_height <= this.skin.Height; core_height++)
                {
                    if(this.core_min_width * this.core_min_height <= core_width * core_height && core_area * (1.0 + deviation) >= core_width * core_height)
                    {
                        this.cores.Add(new Rectangle3d(Plane.WorldXY, core_width, core_height));
                    }
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

        private void evaluate_core_combinations()
        {
            MultiCombinations all_combinations = new MultiCombinations(this.cores.Count, this.max_core_count);
            double core_default_area = this.skin.Area * this.efficiency;
            this.valid_core_combinations = new List<List<int>>();

            // validate core combination
            foreach (var combination in all_combinations.combinations)
            {
                double area_sum = 0;
                
                for(int i = 0; i < combination.Count; i++)
                {
                    area_sum += this.cores[combination[i]].Area;
                }

                if (area_sum >= core_default_area * (1.0 - deviation) && area_sum <= core_default_area * (1.0 + deviation))
                {
                    this.valid_core_combinations.Add(combination);
                }
            }
        }

        private void place_cores()
        {
            // sort cores and values into trees
            for (int i = 0; i < this.valid_core_combinations.Count; i++)
            {





                this.valid_cores.EnsurePath(i);
                this.values.EnsurePath(i);

                List<int> default_values = new List<int>();
                for (int j = 0; j < this.locations.Count; j++)
                {
                    default_values.Add(1);
                }

                foreach (int c in this.valid_core_combinations[i])
                {
                    this.valid_cores.Add(cores[c]);
                    replace_core_values(ref this.locations, ref default_values, cores[c]);
                }
                this.values.AddRange(default_values);
            }
        }
    }
}
