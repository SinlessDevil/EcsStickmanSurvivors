using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.ConcaveHullGenerator
{
    public static class HullFunctions
    {
        public static List<Line> GetDividedLine(Line line, List<Node> nearbyPoints, List<Line> concaveHull,
            double concavity)
        {
            List<Line> dividedLine = new List<Line>();
            List<Node> okMiddlePoints = new List<Node>();
            foreach (Node middlePoint in nearbyPoints)
            {
                double cos = GetCos(line.Nodes[0], line.Nodes[1], middlePoint);
                if (cos < concavity)
                {
                    Line newLineA = new Line(line.Nodes[0], middlePoint);
                    Line newLineB = new Line(middlePoint, line.Nodes[1]);
                    if (!LineCollidesWithHull(newLineA, concaveHull) && !LineCollidesWithHull(newLineB, concaveHull))
                    {
                        middlePoint.Cos = cos;
                        okMiddlePoints.Add(middlePoint);
                    }
                }
            }

            if (okMiddlePoints.Count > 0)
            {
                okMiddlePoints = okMiddlePoints
                    .OrderBy(p => p.Cos)
                    .ToList();
                dividedLine.Add(new Line(line.Nodes[0], okMiddlePoints[0]));
                dividedLine.Add(new Line(okMiddlePoints[0], line.Nodes[1]));
            }

            return dividedLine;
        }

        private static bool LineCollidesWithHull(Line line, List<Line> concave_hull)
        {
            foreach (Line hullLine in concave_hull)
            {
                if (line.Nodes[0].Id != hullLine.Nodes[0].Id && 
                    line.Nodes[0].Id != hullLine.Nodes[1].Id && 
                    line.Nodes[1].Id != hullLine.Nodes[0].Id &&
                    line.Nodes[1].Id != hullLine.Nodes[1].Id)
                {
                    if (LineIntersectionFunctions.DoIntersect(line.Nodes[0], line.Nodes[1], hullLine.Nodes[0],
                            hullLine.Nodes[1]))
                        return true;
                }
            }

            return false;
        }

        private static double GetCos(Node A, Node B, Node O)
        {
            double aPow2 = Math.Pow(A.X - O.X, 2) + Math.Pow(A.Y - O.Y, 2);
            double bPow2 = Math.Pow(B.X - O.X, 2) + Math.Pow(B.Y - O.Y, 2);
            double cPow2 = Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2);
            double cos = (aPow2 + bPow2 - cPow2) / (2 * Math.Sqrt(aPow2 * bPow2));
            return Math.Round(cos, 4);
        }
        
        /// <summary>
        /// The bigger the _scaleFactor the more points it will return
        /// We calculate an ellipse arround the Nodes that define the line (the focus points of said ellipse)
        /// We will add all Nodes contained within the base ellipse scaled to the _scaleFactor
        /// Be carefull: if it's too small it will return very little points (or non!),
        /// if it's too big it will add points that will not be used and will consume time
        /// </summary>
        public static List<Node> GetNearbyPoints(Line line, List<Node> nodeList, double scaleFactor)
        {
            List<Node> nearbyPoints = new List<Node>();
            double lineLength = line.GetLength();
            double baseEllipseFocusSum = 2 * lineLength / Math.Sqrt(2);
            double scaledBaseEllipseFocusSum = baseEllipseFocusSum * scaleFactor;

            foreach (Node node in nodeList)
            {
                double distanceToFocusA =
                    Math.Sqrt(Math.Pow(line.Nodes[0].X - node.X, 2) + Math.Pow(line.Nodes[0].Y - node.Y, 2));
                double distanceToFocusB =
                    Math.Sqrt(Math.Pow(line.Nodes[1].X - node.X, 2) + Math.Pow(line.Nodes[1].Y - node.Y, 2));
                double ellipseFocusSum = distanceToFocusA + distanceToFocusB;
                
                if (ellipseFocusSum <= scaledBaseEllipseFocusSum)
                {
                    nearbyPoints.Add(node);
                }
            }

            return nearbyPoints;
        }
    }
}