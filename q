[33mcommit 5895a20530ea1d9fbf705e00d9e3edad58a3fbe3[m[33m ([m[1;36mHEAD -> [m[1;32mmaster[m[33m, [m[1;31morigin/master[m[33m)[m
Author: Philipp Siedler <Philipp.S@zaha-hadid.com>
Date:   Fri Mar 23 16:46:42 2018 +0000

    changes

[1mdiff --git a/fold/core_generator_class.cs b/fold/core_generator_class.cs[m
[1mindex 63ff4d7..6513480 100644[m
[1m--- a/fold/core_generator_class.cs[m
[1m+++ b/fold/core_generator_class.cs[m
[36m@@ -26,8 +26,14 @@[m [mnamespace core_generator[m
         [m
         public Rectangle3d skin;[m
         public List<Rectangle3d> cores;[m
[32m+[m
[32m+[m[32m        public List<Rectangle3d> cores_1;[m
[32m+[m[32m        public List<Rectangle3d> cores_2;[m
[32m+[m[32m        public DataTree<Rectangle3d> cores_2_tree = new DataTree<Rectangle3d>();[m
[32m+[m
         public List<Point3d> grid_pts = new List<Point3d>();[m
[31m-        public DataTree<bool> grid_val = new DataTree<bool>();[m
[32m+[m[32m        public DataTree<int> grid_val = new DataTree<int>();[m
[32m+[m
 [m
         public generate_tower(ref int sw, ref int sh, ref int cw, ref int ch, ref double e, ref double d)[m
         {[m
[36m@@ -40,24 +46,22 @@[m [mnamespace core_generator[m
 [m
             skin = new Rectangle3d(Plane.WorldXY, sw, sh);[m
             core_area = (sw * sh) * e;[m
[31m-            test_core(ref cores, ref grid_pts, ref grid_val);[m
[32m+[m
[32m+[m[32m            //test_core(ref cores, ref grid_pts, ref grid_val);[m
[32m+[m[32m            test_dual_core(ref cores_1, ref cores_2_tree, ref grid_pts, ref grid_val);[m
         }[m
 [m
[31m-        public void test_core(ref List<Rectangle3d> core_list, ref List<Point3d> g_pts, ref DataTree<bool> g_val)[m
[32m+[m[32m        public void test_core(ref List<Rectangle3d> core_list, ref List<Point3d> g_pts, ref DataTree<int> g_val)[m
         {[m
             List<Rectangle3d> c = new List<Rectangle3d>();[m
 [m
[31m-            double core_area_min = core_area * (1.0 - deviation);[m
[31m-            double core_area_max = core_area * (1.0 + deviation);[m
[31m-[m
             for (int i = 1; i <= skin_width; i++)[m
             {[m
                 for(int j = 1; j <= skin_height; j++)[m
                 {[m
[31m-[m
                     g_pts.Add(new Point3d(i - 0.5, j - 0.5,0));[m
 [m
[31m-                    if ((i * j == core_area || (i * j >= core_area_min && i * j <= core_area_max)) && i >= core_min_width && j >= core_min_height)[m
[32m+[m[32m                    if ((i * j == core_area || (i * j >= (core_area * (1.0 - deviation)) && i * j <= (core_area * (1.0 + deviation))))  && i >= core_min_width && j >= core_min_height)[m
                     {[m
                         double possible_x_pos = skin.Width - i;[m
                         double possible_y_pos = skin.Height - j;[m
[36m@@ -75,11 +79,11 @@[m [mnamespace core_generator[m
                                     {[m
                                         if ((m + 0.5 > k && m + 0.5 < k + i) && (n + 0.5 > l && n + 0.5 < l + j))[m
                                         {[m
[31m-                                            g_val.Add(false);[m
[32m+[m[32m                                            g_val.Add(0);[m
                                         }[m
                                         else[m
                                         {[m
[31m-                                            g_val.Add(true);[m
[32m+[m[32m                                            g_val.Add(1);[m
                                         }                                        [m
                                     }[m
                                 }[m
[36m@@ -90,5 +94,84 @@[m [mnamespace core_generator[m
             }[m
             core_list = c;            [m
         }[m
[32m+[m
[32m+[m[32m        public void test_dual_core(ref List<Rectangle3d> cores_1_list, ref DataTree<Rectangle3d> cores_2_tree, ref List<Point3d> g_pts, ref DataTree<int> g_val)[m
[32m+[m[32m        {[m
[32m+[m[32m            List<Rectangle3d> c1 = new List<Rectangle3d>();[m
[32m+[m[32m            DataTree<Rectangle3d> c2 = new DataTree<Rectangle3d>();[m
[32m+[m
[32m+[m[32m            for (int i = 1; i <= skin_width; i++)[m
[32m+[m[32m            {[m
[32m+[m[32m                for (int j = 1; j <= skin_height; j++)[m
[32m+[m[32m                {[m
[32m+[m[32m                    // generate grid points[m
[32m+[m[32m                    g_pts.Add(new Point3d(i - 0.5, j - 0.5, 0));[m
[32m+[m[41m                    [m
[32m+[m[32m                    if (i * j == core_area / 2 && i >= core_min_width && j >= core_min_height)[m
[32m+[m[32m                    {[m
[32m+[m[32m                        double possible_x_pos = skin.Width - i;[m
[32m+[m[32m                        double possible_y_pos = skin.Height - j;[m
[32m+[m
[32m+[m[32m                        for (int k = 0; k <= possible_x_pos; k++)[m
[32m+[m[32m                        {[m
[32m+[m[32m                            for (int l = 0; l <= possible_y_pos; l++)[m
[32m+[m[32m                            {[m
[32m+[m[32m                                //c1.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + i, l + j, 0)));[m
[32m+[m
[32m+[m[32m                                for (int o = 1; o <= skin_width; o++)[m
[32m+[m[32m                                {[m
[32m+[m[32m                                    for (int p = 1; p <= skin_height; p++)[m
[32m+[m[32m                                    {[m[41m                              [m
[32m+[m
[32m+[m[32m                                        if (o * p == core_area / 2 && o >= core_min_width && p >= core_min_height)[m
[32m+[m[32m                                        {[m
[32m+[m[32m                                            double possible_x_pos_c2 = skin.Width - o;[m
[32m+[m[32m                                            double possible_y_pos_c2 = skin.Height - p;[m
[32m+[m
[32m+[m[41m                                            [m
[32m+[m[32m                                            for (int q = 0; q <= possible_x_pos_c2; q++)[m
[32m+[m[32m                                            {[m
[32m+[m[32m                                                for (int r = 0; r <= possible_y_pos_c2; r++)[m
[32m+[m[32m                                                {[m
[32m+[m[32m                                                    if((q <= k - o || q >= k + i) || (r <= l - p || r >= l + j))[m
[32m+[m[32m                                                    {[m
[32m+[m[32m                                                        g_val.EnsurePath(c2.BranchCount);[m
[32m+[m[32m                                                        c2.EnsurePath(c2.BranchCount);[m
[32m+[m
[32m+[m[32m                                                        c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + i, l + j, 0)));[m
[32m+[m[32m                                                        c2.Add(new Rectangle3d(Plane.WorldXY, new Point3d(q, r, 0), new Point3d(q + o, r + p, 0)));[m
[32m+[m
[32m+[m[32m                                                        // generate values[m
[32m+[m[41m                                                        [m
[32m+[m
[32m+[m[32m                                                        for (int m = 0; m < skin_width; m++)[m
[32m+[m[32m                                                        {[m
[32m+[m[32m                                                            for (int n = 0; n < skin_height; n++)[m
[32m+[m[32m                                                            {[m
[32m+[m[32m                                                                if (((m + 0.5 > k && m + 0.5 < k + i) && (n + 0.5 > l && n + 0.5 < l + j)) || ((m + 0.5 > q && m + 0.5 < q + o) && (n + 0.5 > r && n + 0.5 < r + p)))[m
[32m+[m[32m                                                                {[m
[32m+[m[32m                                                                    g_val.Add(0);[m
[32m+[m[32m                                                                }[m
[32m+[m[32m                                                                else[m
[32m+[m[32m                                                                {[m
[32m+[m[32m                                                                    g_val.Add(1);[m
[32m+[m[32m                                                                }[m
[32m+[m[32m                                                            }[m
[32m+[m[32m                                                        }[m
[32m+[m[32m                                                    }[m[41m                                                    [m
[32m+[m[32m                                                }[m
[32m+[m[32m                                            }[m
[32m+[m[32m                                        }[m
[32m+[m[32m                                    }[m
[32m+[m[32m                                }[m
[32m+[m[32m                            }[m
[32m+[m[32m                        }[m
[32m+[m[32m                    }[m[41m                   [m
[32m+[m[32m                }[m
[32m+[m[32m            }[m
[32m+[m
[32m+[m[32m            cores_1_list = c1;[m
[32m+[m[32m            cores_2_tree = c2;[m
[32m+[m[32m        }[m
     }[m
 }[m
[1mdiff --git a/fold/core_generator_main.cs b/fold/core_generator_main.cs[m
[1mindex f56c19d..0888ccb 100644[m
[1m--- a/fold/core_generator_main.cs[m
[1m+++ b/fold/core_generator_main.cs[m
[36m@@ -47,9 +47,9 @@[m [mnamespace core_generator[m
         protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)[m
         {[m
             pManager.AddCurveParameter("skin", "skin", "skin", GH_ParamAccess.item);[m
[31m-            pManager.AddCurveParameter("cores", "cores", "cores", GH_ParamAccess.list);[m
             pManager.AddPointParameter("grid_pts", "grid_pts", "grid_pts", GH_ParamAccess.list);[m
             pManager.AddIntegerParameter("grid_val", "grid_val", "grid_val", GH_ParamAccess.tree);[m
[32m+[m[32m            pManager.AddCurveParameter("cores", "cores", "cores", GH_ParamAccess.tree);[m
         }[m
 [m
         /// <summary>[m
[36m@@ -76,9 +76,9 @@[m [mnamespace core_generator[m
             generate_tower gt = new generate_tower(ref skin_width, ref skin_height, ref core_min_width, ref core_min_height, ref efficiency, ref deviation);[m
 [m
             DA.SetData(0, gt.skin);[m
[31m-            DA.SetDataList(1, gt.cores);[m
[31m-            DA.SetDataList(2, gt.grid_pts);[m
[31m-            DA.SetDataTree(3, gt.grid_val);[m
[32m+[m[32m            DA.SetDataList(1, gt.grid_pts);[m
[32m+[m[32m            DA.SetDataTree(2, gt.grid_val);[m
[32m+[m[32m            DA.SetDataTree(3, gt.cores_2_tree);[m
         }[m
 [m
         /// <summary>[m
