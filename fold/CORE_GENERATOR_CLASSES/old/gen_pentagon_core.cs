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
    class gen_pentagon_core
    {
        public List<Polyline> core_list;
        public List<Point3d> g_pts;
        public DataTree<int> g_val;

        public gen_pentagon_core(
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
        
        // check if point is in polygon
        private bool IsPointInPolygon(Point3d[] polygon, Point3d point)
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
        
        // calculate method
        public void calculate(
            ref List<Polyline> core_list,
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

            List<Polyline> c = new List<Polyline>();

            double core_area = max_skin_width * max_skin_height * efficiency;

            for (int i = 1; i <= max_skin_width; i++)
            {
                for (int j = 1; j <= max_skin_height; j++)
                {
                    g_pts.Add(new Point3d(i - 0.5, j - 0.5, 0));

                    double possible_x_pos = skin.Width - i;
                    double possible_y_pos = skin.Height - j;

                    int it = 3;

                    for (int k = -it; k < it; k++)
                    {
                        if (possible_x_pos + k <= skin.Width && possible_x_pos + k >= 0)
                        {
                            for (int l = -it; l < it - 1; l++)
                            {
                                if (possible_y_pos + l <= skin.Height && possible_y_pos + l >= 0)
                                {
                                    for (int m = -it; m < it; m++)
                                    {
                                        if (possible_x_pos + m <= skin.Width && possible_x_pos + m >= 0)
                                        {
                                            for (int n = -it; n < it; n++)
                                            {
                                                if (possible_y_pos + n <= skin.Height && possible_y_pos + n >= 0)
                                                {
                                                    for (int o = -it; o < it; o++)
                                                    {
                                                        if (possible_y_pos + o <= skin.Height && possible_y_pos + o >= 0)
                                                        {
                                                            for (int p = -it; p < it; p++)
                                                            {
                                                                if (possible_x_pos + p <= skin.Width && possible_x_pos + p >= 0)
                                                                {
                                                                    List<Point3d> temp = new List<Point3d>();

                                                                    temp.Add(new Point3d(possible_x_pos, possible_y_pos, 0));

                                                                    temp.Add(new Point3d(possible_x_pos + k, possible_y_pos + l, 0));
                                                                    temp.Add(new Point3d(possible_x_pos + m, possible_y_pos + n, 0));
                                                                    temp.Add(new Point3d(possible_x_pos + p, possible_y_pos + o, 0));

                                                                    temp.Add(new Point3d(possible_x_pos, possible_y_pos, 0));

                                                                    //PolylineCurve myLine = new PolylineCurve();

                                                                    var points = temp;
                                                                    var area = Math.Abs(points.Take(points.Count - 1)
                                                                        .Select((pt, ind) => (points[ind + 1].X - pt.X) * (points[ind + 1].Y + pt.Y))
                                                                        .Sum() / 2);

                                                                    if (area == core_area || (area >= core_area * (1.0 - deviation) && area <= core_area * (1.0 + deviation)))
                                                                    {
                                                                        g_val.EnsurePath(c.Count);
                                                                        c.Add(new Polyline(temp));

                                                                        for (int q = 0; q < max_skin_width; q++)
                                                                        {
                                                                            for (int r = 0; r < max_skin_height; r++)
                                                                            {
                                                                                if (IsPointInPolygon(temp.ToArray(), new Point3d(q + 0.5, r + 0.5, 0)))
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
                            }
                        }
                    }
                }
            }

            core_list = c;
        }
    }
}
