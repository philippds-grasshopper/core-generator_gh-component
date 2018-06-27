using System;
using System.Timers;
using System.Collections.Generic;
using Grasshopper.Kernel;
using System.Globalization;
using Rhino.Geometry;
using System.Linq;
using Grasshopper;

namespace core_generator
{
    public class core_generator_comp : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;
        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public core_generator_comp(): base("core_generator", "", "", "PhilsComps", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("type", "type", "type", GH_ParamAccess.item, 0);

            pManager.AddBooleanParameter("allow_skin_variation", "allow_skin_variation", "allow_skin_variation", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("skin_width", "skin_width", "skin_width", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("skin_height", "skin_height", "skin_height", GH_ParamAccess.item, 10);

            pManager.AddBooleanParameter("allow_core_variation", "allow_core_variation", "allow_core_variation", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("core_min_width", "core_min_width", "core_min_width", GH_ParamAccess.item, 5);
            pManager.AddIntegerParameter("core_min_height", "core_min_height", "core_min_height", GH_ParamAccess.item, 5);

            pManager.AddNumberParameter("efficiency", "efficiency", "value 0.0 - 1.0", GH_ParamAccess.item, 0.25);
            pManager.AddNumberParameter("deviation", "deviation", "value 0.0 - 1.0", GH_ParamAccess.item, 0.0);

            pManager.AddIntegerParameter("max_core_count", "max_core_count", "max_core_count", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("skin", "skin", "skin", GH_ParamAccess.list);
            pManager.AddPointParameter("grid_pts", "grid_pts", "grid_pts", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("grid_val", "grid_val", "grid_val", GH_ParamAccess.tree);
            pManager.AddCurveParameter("cores", "cores", "cores", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int type_index = 0;
            bool allow_skin_variation = false;
            int max_skin_width = 0;
            int max_skin_height = 0;
            bool allow_core_variation = false;
            int core_min_width = 0;
            int core_min_height = 0;
            double efficiency = 0;
            double deviation = 0.0;
            int max_core_count = 1;
            

            DA.GetData(0, ref type_index);
            DA.GetData(1, ref allow_skin_variation);
            DA.GetData(2, ref max_skin_width);
            DA.GetData(3, ref max_skin_height);
            DA.GetData(4, ref allow_core_variation);
            DA.GetData(5, ref core_min_width);
            DA.GetData(6, ref core_min_height);
            DA.GetData(7, ref efficiency);
            DA.GetData(8, ref deviation);
            DA.GetData(9, ref max_core_count);

            generate_tower gt = new generate_tower(ref type_index, ref allow_skin_variation, ref max_skin_width, ref max_skin_height, ref allow_core_variation, ref core_min_width, ref core_min_height, ref efficiency, ref deviation, ref max_core_count);

            switch (type_index)
            {
                case 0:


                    DA.SetDataList(0, gt.variable_skin);
                    DA.SetDataTree(1, gt.grid_pts_tree);
                    DA.SetDataTree(2, gt.grid_val);
                    DA.SetDataTree(3, gt.cores_2_tree);                 

                    /*
                    DA.SetData(0, gt.skin);
                    DA.SetDataList(1, gt.grid_pts);
                    DA.SetDataTree(2, gt.grid_val);
                    DA.SetDataList(3, gt.cores);
                    */
                    break;
                case 1:
                    if (!allow_skin_variation) { DA.SetData(0, gt.skin); }
                    else { DA.SetDataList(0, gt.variable_skin); }
                    DA.SetDataList(1, gt.grid_pts);
                    DA.SetDataTree(2, gt.grid_val);
                    DA.SetDataTree(3, gt.cores_2_tree);
                    break;
                case 2:
                    DA.SetData(0, gt.skin);
                    DA.SetDataList(1, gt.grid_pts);
                    DA.SetDataTree(2, gt.grid_val);
                    DA.SetDataList(3, gt.irregular_cores);
                    break;
                case 3:
                    DA.SetDataList(0, gt.variable_skin);
                    DA.SetDataTree(1, gt.grid_pts_tree);
                    DA.SetDataTree(2, gt.grid_val);
                    DA.SetDataTree(3, gt.cores_2_tree);
                    break;
            }         
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                // return core_generator.Properties.Resources.os_reading_component;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("CC01E9DF-91D9-40F0-95A5-CAA8E51EC757"); }
        }
        
    }
}
