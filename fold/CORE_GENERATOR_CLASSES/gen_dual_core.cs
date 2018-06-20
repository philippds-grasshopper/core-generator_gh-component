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
    class gen_dual_core
    {
        public DataTree<Rectangle3d> cores_2_tree;
        public List<Point3d> g_pts;
        public DataTree<int> g_val;
        public List<Rectangle3d> skin_list;

        public gen_dual_core(
            int max_skin_width,
            int max_skin_height,
            bool allow_core_variation,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            cores_2_tree = new DataTree<Rectangle3d>();
            g_pts = new List<Point3d>();
            g_val = new DataTree<int>();
            skin_list = new List<Rectangle3d>();

            // call calculate method
            calculate(
                ref cores_2_tree,
                ref g_pts,
                ref g_val,
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
            ref DataTree<Rectangle3d> cores_2_tree,
            ref List<Point3d> g_pts,
            ref DataTree<int> g_val,
            int max_skin_width,
            int max_skin_height,
            bool allow_core_variation,
            int core_min_width,
            int core_min_height,
            double efficiency,
            double deviation
            )
        {
            List<Rectangle3d> skins = new List<Rectangle3d>();

            Rectangle3d skin = new Rectangle3d(Plane.WorldXY, max_skin_width, max_skin_height);
            DataTree<Rectangle3d> c2 = new DataTree<Rectangle3d>();

            double core_area = max_skin_width * max_skin_height * efficiency;
            double gfa = ((1.0 / efficiency) * core_area) - core_area;
            double single_core_area = core_area / 2;

            int default_core_width = (int)Math.Sqrt(single_core_area);
            int default_core_height = default_core_width;

            int x_position_c1 = max_skin_width;
            int y_position_c1 = max_skin_height;

            int x_position_c2 = x_position_c1;
            int y_position_c2 = y_position_c1;




            for(int s = core_min_width; s < max_skin_width; s++)
            {
                for(int t = core_min_height; t < max_skin_height; t++)
                {
                    if (s * t - core_area >= gfa * (1.0 - deviation) && s * t - core_area <= gfa * (1.0 + deviation))
                    {
                        skins.Add(new Rectangle3d(Plane.WorldXY, new Point3d(0, 0, 0), new Point3d(s, t, 0)));

                        double possible_x_pos = s - core_min_width;
                        double possible_y_pos = t - core_min_height;

                        //c.EnsurePath(skins.Count - 1);

                        for (int i = 0; i < x_position_c1; i++)
                        {
                            for (int j = 0; j < y_position_c1; j++)
                            {
                                // generate grid points
                                g_pts.Add(new Point3d(i + 0.5, j + 0.5, 0));


                                if (allow_core_variation)
                                {
                                    if (i >= core_min_width && j >= core_min_height)
                                    {
                                        //double possible_x_pos = skin.Width - i;
                                        //double possible_y_pos = skin.Height - j;

                                        for (int k = 0; k < possible_x_pos; k++)
                                        {
                                            for (int l = 0; l < possible_y_pos; l++)
                                            {
                                                for (int o = 1; o < max_skin_width; o++)
                                                {
                                                    for (int p = 1; p < max_skin_height; p++)
                                                    {

                                                        if ((o * p == (core_area - i * j) || ((o * p >= (core_area - i * j) * (1.0 - deviation)) && (o * p <= (core_area - i * j) * (1.0 + deviation)))) && o >= core_min_width && p >= core_min_height)
                                                        {
                                                            double possible_x_pos_c2 = skin.Width - o;
                                                            double possible_y_pos_c2 = skin.Height - p;

                                                            for (int q = 0; q < possible_x_pos_c2; q++)
                                                            {
                                                                for (int r = 0; r < possible_y_pos_c2; r++)
                                                                {
                                                                    if ((q <= k - o || q >= k + i) || (r <= l - p || r >= l + j))
                                                                    {
                                                                        g_val.EnsurePath(c2.BranchCount);
                                                                        c2.EnsurePath(c2.BranchCount);

                                                                        c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + i, l + j, 0)));
                                                                        c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(q, r, 0), new Point3d(q + o, r + p, 0)));

                                                                        // generate values
                                                                        for (int m = 0; m < max_skin_width; m++)
                                                                        {
                                                                            for (int n = 0; n < max_skin_height; n++)
                                                                            {
                                                                                if (((m + 0.5 > k && m + 0.5 < k + i) && (n + 0.5 > l && n + 0.5 < l + j)) || ((m + 0.5 > q && m + 0.5 < q + o) && (n + 0.5 > r && n + 0.5 < r + p)))
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
                                                }
                                            }
                                        }
                                    }
                                }
                                /*
                                if (allow_core_variation)
                                {
                                    if (i >= core_min_width && j >= core_min_height)
                                    {
                                        double possible_x_pos = skin.Width - i;
                                        double possible_y_pos = skin.Height - j;

                                        for (int k = 0; k < possible_x_pos; k++)
                                        {
                                            for (int l = 0; l < possible_y_pos; l++)
                                            {
                                                for (int o = 1; o < max_skin_width; o++)
                                                {
                                                    for (int p = 1; p < max_skin_height; p++)
                                                    {

                                                        if ((o * p == (core_area - i * j) || ((o * p >= (core_area - i * j) * (1.0 - deviation)) && (o * p <= (core_area - i * j) * (1.0 + deviation)))) && o >= core_min_width && p >= core_min_height)
                                                        {
                                                            double possible_x_pos_c2 = skin.Width - o;
                                                            double possible_y_pos_c2 = skin.Height - p;

                                                            for (int q = 0; q < possible_x_pos_c2; q++)
                                                            {
                                                                for (int r = 0; r < possible_y_pos_c2; r++)
                                                                {
                                                                    if ((q <= k - o || q >= k + i) || (r <= l - p || r >= l + j))
                                                                    {
                                                                        g_val.EnsurePath(c2.BranchCount);
                                                                        c2.EnsurePath(c2.BranchCount);

                                                                        c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + i, l + j, 0)));
                                                                        c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(q, r, 0), new Point3d(q + o, r + p, 0)));

                                                                        // generate values
                                                                        for (int m = 0; m < max_skin_width; m++)
                                                                        {
                                                                            for (int n = 0; n < max_skin_height; n++)
                                                                            {
                                                                                if (((m + 0.5 > k && m + 0.5 < k + i) && (n + 0.5 > l && n + 0.5 < l + j)) || ((m + 0.5 > q && m + 0.5 < q + o) && (n + 0.5 > r && n + 0.5 < r + p)))
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
                                                }
                                            }
                                        }
                                    }
                                }
                                */
                                else if (i <= max_skin_width - default_core_width && j <= max_skin_height - default_core_height)
                                {
                                    for (int k = 0; k < x_position_c2; k++)
                                    {
                                        for (int l = 0; l < y_position_c2; l++)
                                        {
                                            if ((k >= i + default_core_width ||
                                                k <= i - default_core_width ||
                                                l >= j + default_core_height ||
                                                l <= j - default_core_height) &&
                                                (k <= max_skin_width - default_core_width &&
                                                l <= max_skin_height - default_core_height))
                                            {
                                                g_val.EnsurePath(c2.BranchCount);
                                                c2.EnsurePath(c2.BranchCount);

                                                c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(i, j, 0), new Point3d(i + default_core_width, j + default_core_height, 0)));
                                                c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + default_core_width, l + default_core_height, 0)));

                                                // generate values
                                                for (int m = 0; m < max_skin_width; m++)
                                                {
                                                    for (int n = 0; n < max_skin_height; n++)
                                                    {
                                                        if (((m + 0.5 > k && m + 0.5 < k + i) && (n + 0.5 > l && n + 0.5 < l + j)) || ((m + 0.5 > i && m + 0.5 < i + default_core_width) && (n + 0.5 > j && n + 0.5 < j + default_core_height)))
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
                        }
                    }
                }
            }
            cores_2_tree = c2;
            skin_list = skins;
        }
    }
}
