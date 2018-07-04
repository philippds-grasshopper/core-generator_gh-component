using System;
using System.Timers;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using System.Web.Script.Serialization;
using Grasshopper;
using System.IdentityModel;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace core_generator
{
    public class generate_tower
    {
        public bool allow_skin_variation;
        public int max_skin_width;
        public int max_skin_height;
        public bool allow_core_variation;
        public int core_min_width;
        public int core_min_height;
        public double efficiency;
        public double deviation;
        public int max_core_count;

        public List<Rectangle3d> variable_skin;
        public DataTree<Point3d> grid_pts_tree;
        public DataTree<int> grid_val;
        public DataTree<Rectangle3d> cores;
        
        public generate_tower(ref bool allow_skin_variation,
                              ref int max_skin_width,
                              ref int max_skin_height,
                              ref bool allow_core_variation,
                              ref int core_min_width,
                              ref int core_min_height,
                              ref double efficiency,
                              ref double deviation,
                              ref int max_core_count)
        {
            // initialize values
            this.allow_skin_variation = allow_skin_variation;
            this.max_skin_width = max_skin_width;
            this.max_skin_height = max_skin_height;
            this.allow_core_variation = allow_core_variation;
            this.core_min_width = core_min_width;
            this.core_min_height = core_min_height;
            this.efficiency = efficiency;
            this.deviation = deviation;
            this.max_core_count = max_core_count;

            gen_skin gs = new gen_skin(max_skin_width, max_skin_height, deviation, allow_skin_variation);
            this.variable_skin = gs.skin;
            this.grid_pts_tree = new DataTree<Point3d>();
            this.grid_val = new DataTree<int>();
            this.cores = new DataTree<Rectangle3d>();

            for (int i = 0; i < gs.skin.Count; i++)
            {
                gen_core gc = new gen_core(core_min_width, core_min_height, gs.skin[i], max_core_count, efficiency, deviation, allow_core_variation);
                this.cores.MergeTree(gc.valid_cores);
                this.grid_val.MergeTree(gc.values);
                this.grid_pts_tree.EnsurePath(i);
                this.grid_pts_tree.AddRange(gc.locations);
            }
        }
    }
}
