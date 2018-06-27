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
        //public List<Item> pjsonEnv = new List<Item>();
        public int type_index;
        public bool allow_skin_variation;
        public int max_skin_width;
        public int max_skin_height;
        public bool allow_core_variation;
        public int core_min_width;
        public int core_min_height;
        public double efficiency;
        public double deviation;
        public int max_core_count;

        public Rectangle3d skin;

        // single core
        public List<Rectangle3d> cores;

        // dual core
        public List<Rectangle3d> cores_1;
        public List<Rectangle3d> cores_2;
        public DataTree<Rectangle3d> cores_2_tree;

        // single irregular core
        public List<Polyline> irregular_cores;

        // variable skin
        public List<Rectangle3d> variable_skin;


        public List<Point3d> grid_pts;

        public DataTree<Point3d> grid_pts_tree;
        public DataTree<int> grid_val;
        
        public generate_tower(ref int ti, ref bool asv, ref int sw, ref int sh, ref bool acv, ref int cw, ref int ch, ref double e, ref double d, ref int mc)
        {
            // initialize values
            type_index = ti;
            allow_skin_variation = asv;
            max_skin_width = sw;
            max_skin_height = sh;
            allow_core_variation = acv;
            core_min_width = cw;
            core_min_height = ch;
            max_core_count = mc;

            efficiency = e;
            deviation = d;
            skin = new Rectangle3d(Plane.WorldXY, max_skin_width, max_skin_height);

            grid_pts_tree = new DataTree<Point3d>();

            grid_pts = new List<Point3d>();
            grid_val = new DataTree<int>();
            
            switch (type_index)
            {
                case 0:
                    Rhino.RhinoApp.WriteLine("Integration");

                    gen_skin gs = new gen_skin(max_skin_width, max_skin_height, deviation);

                    DataTree<Rectangle3d> cores = new DataTree<Rectangle3d>();
                    DataTree<int> values = new DataTree<int>();
                    DataTree<Point3d> loc = new DataTree<Point3d>();

                    for (int i = 0; i < gs.skin.Count; i++)
                    {
                        gen_core gc = new gen_core(core_min_width, core_min_height, gs.skin[i], max_core_count, efficiency, deviation);
                        cores.MergeTree(gc.valid_cores);
                        values.MergeTree(gc.values);
                        loc.EnsurePath(i);
                        loc.AddRange(gc.locations);
                    }

                    variable_skin = gs.skin;
                    cores_2_tree = cores;
                    grid_pts_tree = loc;
                    grid_val = values;
                    
                    /*
                    gen_single_core gsc = new gen_single_core(allow_skin_variation, max_skin_width, max_skin_height, allow_core_variation, core_min_width, core_min_height, efficiency, deviation);
                    cores = gsc.core_list;
                    grid_pts = gsc.g_pts;
                    grid_val = gsc.g_val;
                    */
                    break;
                case 1:
                    gen_dual_core gdc = new gen_dual_core(max_skin_width, max_skin_height, allow_core_variation, core_min_width, core_min_height, efficiency, deviation);
                    variable_skin = gdc.skin_list;
                    cores_2_tree = gdc.cores_2_tree;
                    grid_pts = gdc.g_pts;
                    grid_val = gdc.g_val;
                    break;
                case 2:
                    gen_pentagon_core gpc = new gen_pentagon_core(max_skin_width, max_skin_height, core_min_width, core_min_height, efficiency, deviation);
                    irregular_cores = gpc.core_list;
                    grid_pts = gpc.g_pts;
                    grid_val = gpc.g_val;
                    break;
                case 3:
                    gen_skin_variety gsv = new gen_skin_variety(max_skin_width, max_skin_height, core_min_width, core_min_height, efficiency, deviation);
                    cores_2_tree = gsv.core_list;
                    variable_skin = gsv.skin_list;
                    grid_pts_tree = gsv.g_pts;
                    grid_val = gsv.g_val;
                    break;
            }
        }
    }
}
