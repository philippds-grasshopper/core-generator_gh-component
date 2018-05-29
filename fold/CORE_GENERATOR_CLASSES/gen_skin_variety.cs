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
    class gen_skin_variety
    {
        public DataTree<Rectangle3d> core_list;
        public List<Rectangle3d> skin_list;
        public DataTree<Point3d> g_pts;
        public DataTree<int> g_val;

        public gen_skin_variety(
            int max_skin_width,
            int max_skin_height,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            calculate(
                ref core_list,
                ref skin_list,
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

        public void calculate(
            ref DataTree<Rectangle3d> core_list,
            ref List<Rectangle3d> skin_list,
            ref DataTree<Point3d> g_pts,
            ref DataTree<int> g_val,
            int max_skin_width,
            int max_skin_height,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            List<Rectangle3d> s = new List<Rectangle3d>();
            DataTree<Rectangle3d> c = new DataTree<Rectangle3d>();

            double core_area = core_min_width * core_min_height;
            double gfa = ((1.0 / efficiency) * core_area) - core_area;

            for (int skin_w = 1; skin_w <= max_skin_width; skin_w++)
            {
                for (int skin_h = 1; skin_h <= max_skin_height; skin_h++)
                {
                    if (skin_w * skin_h - core_area >= gfa * (1.0 - deviation) && skin_w * skin_h - core_area <= gfa * (1.0 + deviation))
                    {
                        s.Add(new Rectangle3d(Plane.WorldXY, new Point3d(0, 0, 0), new Point3d(skin_w, skin_h, 0)));

                        double possible_x_pos = skin_w - core_min_width;
                        double possible_y_pos = skin_h - core_min_height;

                        c.EnsurePath(s.Count - 1);
                        for (int k = 0; k <= possible_x_pos; k++)
                        {
                            for (int l = 0; l <= possible_y_pos; l++)
                            {

                                c.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + core_min_width, l + core_min_height, 0)));

                                g_val.EnsurePath(c.AllData().Count - 1);

                                g_pts.EnsurePath(c.AllData().Count - 1);

                                for (int m = 0; m < skin_w; m++)
                                {
                                    for (int n = 0; n < skin_h; n++)
                                    {
                                        g_pts.Add(new Point3d(m + 0.5, n + 0.5, 0));

                                        if ((m + 0.5 > k && m + 0.5 < k + core_min_width) && (n + 0.5 > l && n + 0.5 < l + core_min_height))
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

            skin_list = s;
            core_list = c;
        }
    }
}
