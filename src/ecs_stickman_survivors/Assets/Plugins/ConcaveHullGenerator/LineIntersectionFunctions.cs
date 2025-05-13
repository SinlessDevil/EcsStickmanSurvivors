using System;

namespace Plugins.ConcaveHullGenerator
{
    public static class LineIntersectionFunctions
    {
        /// <summary>
        /// The main function that returns true if line segment 'p1q1' 
        /// and 'p2q2' intersect. 
        /// </summary>
        public static Boolean DoIntersect(Node p1, Node q1, Node p2, Node q2)
        {
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            if (o1 != o2 && o3 != o4)
                return true;

            if (o1 == 0 && OnSegment(p1, p2, q1)) 
                return true;

            if (o2 == 0 && OnSegment(p1, q2, q1)) 
                return true;

            if (o3 == 0 && OnSegment(p2, p1, q2)) 
                return true;

            if (o4 == 0 && OnSegment(p2, q1, q2)) 
                return true;

            return false;
        }

        /// <summary>
        /// Given three colinear points p, q, r, the function checks if 
        /// point q lies on line segment 'pr'
        /// </summary>
        private static bool OnSegment(Node p, Node q, Node r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        /// <summary>
        /// To find Orientation of ordered triplet (p, q, r). 
        /// The function returns following values 
        /// 0 --> p, q and r are colinear 
        /// 1 --> Clockwise 
        /// 2 --> Counterclockwise 
        /// </summary>
        private static int Orientation(Node p, Node q, Node r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if (val == 0) 
                return 0;

            return (val > 0) ? 1 : 2;
        }
    }
}