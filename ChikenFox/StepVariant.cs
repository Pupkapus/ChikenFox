using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChikenFox
{
    class StepVariant
    {
        public List<Point> steps; 

        public int x;
        public int y;

        public StepVariant(int x, int y)
        {
            this.x = x;
            this.y = y;

            steps = new List<Point>();
        }
   
        public StepVariant Clone()
        {
            StepVariant res = new StepVariant(x, y);
            foreach (Point p in steps)
                res.steps.Add(p);

            return res;
        }
    }
}
