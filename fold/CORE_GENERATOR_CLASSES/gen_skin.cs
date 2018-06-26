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
    class gen_skin
    {
        public List<Rectangle3d> skin;
        private int max_skin_width;
        private int max_skin_height;
        private bool variety;
        private double deviation;
        private int area;

        public gen_skin(int max_skin_width, int max_skin_height, bool variety, double deviation)
        {
            this.max_skin_width = max_skin_width;
            this.max_skin_height = max_skin_height;
            this.variety = variety;
            this.deviation = deviation;
            this.area = max_skin_width * max_skin_height;

        }

        public void compute()
        {
            if(variety) { multiple_skin(); }
            else { single_skin(); }
        }

        public void single_skin()
        {
            this.skin.Add(new Rectangle3d(Plane.WorldXY, this.max_skin_width, this.max_skin_height));
        }
        public void multiple_skin()
        {
            for(int skin_width = 0; skin_width <= this.max_skin_width; skin_width++)
            {
                for(int skin_height = 0; skin_height <= this.max_skin_height; skin_height++)
                {
                    if(skin_width * skin_height >= this.area * (1.0 - this.deviation))
                    {
                        this.skin.Add(new Rectangle3d(Plane.WorldXY, skin_width, skin_height));
                    }
                }
            }
        }
    }
}
