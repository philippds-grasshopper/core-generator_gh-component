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
    class gen_single_core
    {
        public List<Rectangle3d> core_list;
        public List<Point3d> g_pts;
        public DataTree<int> g_val;

        public gen_single_core(
            bool allow_skin_variation,
            int max_skin_width,
            int max_skin_height,
            bool allow_core_variation,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            // call calculate method
            core_list = new List<Rectangle3d>();
            g_pts = new List<Point3d>();
            g_val = new DataTree<int>();

            calculate(
                ref core_list,
                ref g_pts,
                ref g_val,
                allow_skin_variation,
                max_skin_width,
                max_skin_height,
                allow_core_variation,
                core_min_width,
                core_min_height,
                efficiency,
                deviation
                );
        }
        
        // calculate method
        public void calculate(
            ref List<Rectangle3d> core_list,
            ref List<Point3d> g_pts,
            ref DataTree<int> g_val,
            bool allow_skin_variation,
            int max_skin_width,
            int max_skin_height,
            bool allow_core_variation,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            Rectangle3d skin = new Rectangle3d(Plane.WorldXY, max_skin_width, max_skin_height);

            List<Rectangle3d> c = new List<Rectangle3d>();
            double core_area = max_skin_width * max_skin_height * efficiency;

            int default_core_width = (int)Math.Sqrt(core_area);
            int default_core_height = default_core_width;

            int x_position = max_skin_width;
            int y_position = max_skin_height;
            
            for (int i = 0; i < x_position; i++)
            {
                for (int j = 0; j < y_position; j++)
                {
                    g_pts.Add(new Point3d(i + 0.5, j + 0.5, 0));
                    if (allow_core_variation)
                    {
                        if ((i * j == core_area || (i * j >= (core_area * (1.0 - deviation)) && i * j <= (core_area * (1.0 + deviation)))) && i >= core_min_width && j >= core_min_height)
                        {
                            double possible_x_pos = skin.Width - i;
                            double possible_y_pos = skin.Height - j;

                            for (int k = 0; k <= possible_x_pos; k++)
                            {
                                for (int l = 0; l <= possible_y_pos; l++)
                                {
                                    c.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + i, l + j, 0)));
                                    g_val.EnsurePath(c.Count() - 1);

                                    for (int m = 0; m < max_skin_width; m++)
                                    {
                                        for (int n = 0; n < max_skin_height; n++)
                                        {
                                            if ((m + 0.5 > k && m + 0.5 < k + i) && (n + 0.5 > l && n + 0.5 < l + j))
                                            {
                                                g_val.Add(0);
                                            }
                                            else
                                            {
                                                g_val.Add(1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {                                                
                        if(i + default_core_width <= max_skin_width && j + default_core_height <= max_skin_height)
                        {
                            c.Add(new Rectangle3d(Plane.WorldXY, new Point3d(i, j, 0), new Point3d(i + default_core_width, j + default_core_height, 0)));
                            g_val.EnsurePath(c.Count() - 1);

                            for (int m = 0; m < max_skin_width; m++)
                            {
                                for (int n = 0; n < max_skin_height; n++)
                                {
                                    if ((m + 0.5 > i && m + 0.5 < i + default_core_width) && (n + 0.5 > j && n + 0.5 < j + default_core_height))
                                    {
                                        g_val.Add(0);
                                    }
                                    else
                                    {
                                        g_val.Add(1);
                                    }
                                }
                            }
                        }                        
                    }
                }
            }
            core_list = c;
        }
    }
}
