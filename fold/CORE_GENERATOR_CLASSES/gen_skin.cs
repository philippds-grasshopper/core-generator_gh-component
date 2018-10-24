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
        private int skin_width;
        private int skin_height;
        private double deviation;
        private bool allow_skin_variation;
        private int area;

        public gen_skin(int skin_width, int skin_height, double deviation, bool allow_skin_variation)
        {
            this.skin = new List<Rectangle3d>();
            this.skin_width = skin_width;
            this.skin_height = skin_height;
            this.deviation = deviation;
            this.allow_skin_variation = allow_skin_variation;
            this.area = skin_width * skin_height;
            compute();
        }

        public void compute()
        {
            if(this.deviation != 0.0 && this.allow_skin_variation) { multiple_skin(); }
            else { single_skin(); }
        }

        public void single_skin()
        {
            this.skin.Add(new Rectangle3d(Plane.WorldXY, this.skin_width, this.skin_height));
        }

        public void multiple_skin()
        {
            for(int skin_width = 0; skin_width <= this.skin_width * (1.0 + this.deviation); skin_width++)
            {
                for(int skin_height = 0; skin_height <= this.skin_height * (1.0 + this.deviation); skin_height++)
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
