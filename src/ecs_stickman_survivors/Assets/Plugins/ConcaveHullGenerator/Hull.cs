using System.Collections.Generic;
using System.Linq;

namespace Plugins.ConcaveHullGenerator
{
    public static class Hull
    {
        public static List<Node> UnusedNodes = new();
        public static List<Line> HullEdges = new();
        public static List<Line> HullConcaveEdges = new();

        private static List<Line> GetHull(List<Node> nodes)
        {
            List<Node> convexH = new List<Node>();
            List<Line> exitLines = new List<Line>();

            convexH = new List<Node>();
            convexH.AddRange(GrahamScan.convexHull(nodes));
            for (int i = 0; i < convexH.Count - 1; i++)
            {
                exitLines.Add(new Line(convexH[i], convexH[i + 1]));
            }

            exitLines.Add(new Line(convexH[0], convexH[convexH.Count - 1]));
            return exitLines;
        }

        public static void SetConvexHull(List<Node> nodes)
        {
            UnusedNodes.AddRange(nodes);
            HullEdges.AddRange(GetHull(nodes));
            foreach (var node in HullEdges.SelectMany(line => line.Nodes))
            {
                UnusedNodes.RemoveAll(a => a.Id == node.Id);
            }
        }

        public static List<Line> SetConcaveHull(double concavity, double scaleFactor)
        {
            bool aLineWasDividedInTheIteration;
            HullConcaveEdges.AddRange(HullEdges);
            
            do
            {
                aLineWasDividedInTheIteration = false;
                for (int linePositionInHull = 0;
                     linePositionInHull < HullConcaveEdges.Count && !aLineWasDividedInTheIteration;
                     linePositionInHull++)
                {
                    Line line = HullConcaveEdges[linePositionInHull];
                    List<Node> nearbyPoints = HullFunctions.GetNearbyPoints(line, UnusedNodes, scaleFactor);
                    List<Line> dividedLine =
                        HullFunctions.GetDividedLine(line, nearbyPoints, HullConcaveEdges, concavity);
                    if (dividedLine.Count > 0)
                    {
                        aLineWasDividedInTheIteration = true;
                        UnusedNodes
                            .Remove(UnusedNodes
                                .Where(n => n.Id == dividedLine[0].Nodes[1].Id)
                                .FirstOrDefault());
                        HullConcaveEdges.AddRange(dividedLine);
                        HullConcaveEdges.RemoveAt(linePositionInHull);
                    }
                }

                HullConcaveEdges = HullConcaveEdges
                    .OrderByDescending(a => Line.GetLength(a.Nodes[0], a.Nodes[1]))
                    .ToList();
            } while (aLineWasDividedInTheIteration);

            return HullConcaveEdges;
        }

        public static void CleanUp()
        {
            UnusedNodes.Clear();
            HullEdges.Clear();
            HullConcaveEdges.Clear();
        }
    }
}