using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper;

namespace core_generator
{
    class gen_core
    {
        private int core_width;
        private int core_height;
        public List<Point3d> locations;
        public DataTree<int> values;
        public Rectangle3d skin;
        private int max_core_count;
        private double efficiency;
        private double deviation;
        private List<Rectangle3d> cores;
        private List<List<int>> valid_core_combinations;
        public DataTree<Rectangle3d> valid_cores;

        public gen_core(int core_width, int core_height, Rectangle3d skin, int max_core_count, double efficiency, double deviation)
        {
            Rhino.RhinoApp.WriteLine("INITIALIZED_CORES");
            this.core_width = core_width;
            this.core_height = core_height;
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
            double core_area = (this.skin.Width * this.skin.Height) * this.efficiency;
            Rhino.RhinoApp.WriteLine("core area: " + core_area.ToString());

            for (int core_width = this.core_width; core_width <= this.skin.Width; core_width++)
            {
                for (int core_height = this.core_height; core_height <= this.skin.Height; core_height++)
                {
                    if(core_area * (1.0 - deviation) <= core_width * core_height && core_area * (1.0 + deviation) >= core_width * core_height)
                    {
                        this.cores.Add(new Rectangle3d(Plane.WorldXY, core_width, core_height));
                    }
                }
            }
        }

        private void evaluate_core_combinations()
        {
            MultiCombinations all_combinations = new MultiCombinations(this.cores.Count, this.max_core_count);
            //double core_default_area = this.core_width * this.core_height;
            double core_default_area = this.skin.Area * this.efficiency;
            //Rhino.RhinoApp.WriteLine(core_default_area.ToString());
            this.valid_core_combinations = new List<List<int>>();


            Rhino.RhinoApp.WriteLine("core combination count: " + all_combinations.combinations.Count.ToString());

            foreach (var combination in all_combinations.combinations)
            {
                double area_sum = 0;
                
                for(int i = 0; i < combination.Count; i++)
                {
                    area_sum += this.cores[combination[i]].Area;
                }

                Rhino.RhinoApp.WriteLine("total core area: " + area_sum.ToString());
                Rhino.RhinoApp.WriteLine("should be around: " + core_default_area.ToString());

                if (area_sum >= core_default_area * (1.0 - deviation) && area_sum <= core_default_area * (1.0 + deviation))
                {
                    this.valid_core_combinations.Add(combination);
                }
            }

            Rhino.RhinoApp.WriteLine("combination count: " + this.valid_core_combinations.Count.ToString());
            for (int i = 0; i < this.valid_core_combinations.Count; i++)
            {
                this.valid_cores.EnsurePath(i);
                foreach (int c in this.valid_core_combinations[i])
                {
                    this.valid_cores.Add(cores[c]);
                    Rhino.RhinoApp.WriteLine("added core to list");
                }                
            }

            Rhino.RhinoApp.WriteLine("total cores: " + this.valid_cores.BranchCount.ToString());
        }

    }
}
