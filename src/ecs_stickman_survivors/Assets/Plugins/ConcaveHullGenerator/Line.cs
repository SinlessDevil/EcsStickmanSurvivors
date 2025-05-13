using System;

namespace Plugins.ConcaveHullGenerator
{
    public class Line
    {
        public Node[] Nodes = new Node[2];
        
        public Line(Node n1, Node n2)
        {
            Nodes[0] = n1;
            Nodes[1] = n2;
        }

        public double GetLength()
        {
            double length = Math.Sqrt(Math.Pow(Nodes[0].Y - Nodes[1].Y, 2) + Math.Pow(Nodes[0].X - Nodes[1].X, 2));
            return length;
        }

        public static double GetLength(Node node1, Node node2)
        {
            double length = Math.Sqrt(Math.Pow(node1.Y - node2.Y, 2) + Math.Pow(node1.X - node2.X, 2));
            return length;
        }
    }
}