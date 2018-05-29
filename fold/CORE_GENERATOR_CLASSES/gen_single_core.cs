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
            int max_skin_width,
            int max_skin_height,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            // call calculate method
            calculate(
                ref core_list,
                ref g_pts,
                ref g_val,
                max_skin_width,
                max_skin_height,
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
            int max_skin_width,
            int max_skin_height,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            Rectangle3d skin = new Rectangle3d(Plane.WorldXY, max_skin_width, max_skin_height);

            List<Rectangle3d> c = new List<Rectangle3d>();
            double core_area = max_skin_width * max_skin_height * efficiency;

            for (int i = 1; i <= max_skin_width; i++)
            {
                for (int j = 1; j <= max_skin_height; j++)
                {
                    g_pts.Add(new Point3d(i - 0.5, j - 0.5, 0));

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
            }
            core_list = c;
        }
    }
}
