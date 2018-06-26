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
    class gen_core
    {
        private int core_min_width;
        private int core_min_height;
        public DataTree<Rectangle3d> core;
        public List<Point3d> locations;
        public DataTree<int> values;
        private Rectangle3d skin;
        private int max_core_count;
        private double efficiency;
        private double deviation;

        public gen_core(int core_min_width, int core_min_height, ref Rectangle3d skin, int max_core_count, double efficiency, double deviation)
        {
            this.core_min_width = core_min_width;
            this.core_min_height = core_min_height;
            this.skin = skin;
            this.max_core_count = max_core_count;
            this.efficiency = efficiency;
            this.deviation = deviation;
        }

        public void compute()
        {
            create_locations();

        }

        public void create_locations()
        {
            for (int skin_width = 0; skin_width < this.skin.Width; skin_width++)
            {
                for (int skin_height = 0; skin_height < this.skin.Height; skin_height++)
                {
                    this.locations.Add(new Point3d(skin_width, skin_height, 0));
                }
            }
        }

        public void create_cores()
        {
            double min_core_area = (this.skin.Width * this.skin.Height) * this.efficiency;
            double remaining_core_area = min_core_area;
            if(this.max_core_count > 0)
            {
                double area = this.core_min_width * this.core_min_height;
                remaining_core_area = min_core_area - (area * (this.max_core_count - 1));
            }

            List<Rectangle3d> cores = new List<Rectangle3d>();
            for (int core_width = this.core_min_width; core_width <= this.skin.Width; core_width++)
            {
                for (int core_height = this.core_min_height; core_height <= this.skin.Height; core_height++)
                {
                    if(min_core_area <= core_width * core_height && remaining_core_area >= core_width * core_height)
                    {
                        cores.Add(new Rectangle3d(Plane.WorldXY, core_width, core_height));
                    }
                }
            }
        }






    }
}
