using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.ConcaveHullGenerator
{
    public static class GrahamScan
    {
        private const int TurnLeft = 1;
        private const int TurnRight = -1;
        private const int TurnNone = 0;

        private static int Turn(Node p, Node q, Node r) => ((q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y))
            .CompareTo(0);

        private static void KeepLeft(List<Node> hull, Node r)
        {
            while (hull.Count > 1 && Turn(hull[hull.Count - 2], hull[hull.Count - 1], r) != TurnLeft)
            {
                hull.RemoveAt(hull.Count - 1);
            }

            if (hull.Count == 0 || hull[hull.Count - 1] != r)
            {
                hull.Add(r);
            }
        }

        private static double GetAngle(Node p1, Node p2)
        {
            double xDiff = p2.X - p1.X;
            double yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        private static List<Node> MergeSort(Node p0, List<Node> arrPoint)
        {
            if (arrPoint.Count == 1)
                return arrPoint;

            List<Node> arrSortedInt = new List<Node>();
            int middle = (int)arrPoint.Count / 2;
            List<Node> leftArray = arrPoint.GetRange(0, middle);
            List<Node> rightArray = arrPoint.GetRange(middle, arrPoint.Count - middle);
            leftArray = MergeSort(p0, leftArray);
            rightArray = MergeSort(p0, rightArray);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Count + rightArray.Count; i++)
            {
                if (leftptr == leftArray.Count)
                {
                    arrSortedInt.Add(rightArray[rightptr]);
                    rightptr++;
                }
                else if (rightptr == rightArray.Count)
                {
                    arrSortedInt.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else if (GetAngle(p0, leftArray[leftptr]) < GetAngle(p0, rightArray[rightptr]))
                {
                    arrSortedInt.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else
                {
                    arrSortedInt.Add(rightArray[rightptr]);
                    rightptr++;
                }
            }

            return arrSortedInt;
        }

        public static List<Node> convexHull(List<Node> points)
        {
            Node p0 = null;
            foreach (Node value in points)
            {
                if (p0 == null)
                {
                    p0 = value;
                }
                else
                {
                    if (p0.Y > value.Y)
                    {
                        p0 = value;
                    }
                }
            }

            List<Node> order = points.Where(value => p0 != value).ToList();
            order = MergeSort(p0, order);
            
            List<Node> result = new List<Node>();
            result.Add(p0);
            result.Add(order[0]);
            result.Add(order[1]);
            
            order.RemoveAt(0);
            order.RemoveAt(0);
            
            foreach (Node value in order)
            {
                KeepLeft(result, value);
            }

            return result;
        }
    }
}