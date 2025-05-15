using System.Collections.Generic;
using Plugins.ConcaveHull.Code;
using UnityEngine;

namespace Code.Editors.СolliderMeshCreator
{
    public static class EdgeOutlineBuilder
    {
        private const float Epsilon = 0.001f;

        public static List<Vector3> BuildOutline(List<Line> unorderedEdges)
        {
            List<Vector3> outline = new List<Vector3>();
            if (unorderedEdges == null || unorderedEdges.Count == 0) return outline;

            List<Line> edges = new List<Line>(unorderedEdges);
            Line current = edges[0];
            edges.RemoveAt(0);

            Vector2 start = new Vector2((float)current.Nodes[0].X, (float)current.Nodes[0].Y);
            Vector2 end = new Vector2((float)current.Nodes[1].X, (float)current.Nodes[1].Y);

            outline.Add(new Vector3(start.x, 0, start.y));
            outline.Add(new Vector3(end.x, 0, end.y));

            while (edges.Count > 0)
            {
                Vector2 last = new Vector2(outline[^1].x, outline[^1].z);

                int index = edges.FindIndex(e =>
                    Vector2.Distance(new Vector2((float)e.Nodes[0].X, (float)e.Nodes[0].Y), last) < Epsilon ||
                    Vector2.Distance(new Vector2((float)e.Nodes[1].X, (float)e.Nodes[1].Y), last) < Epsilon);

                if (index == -1) 
                    break;

                Line edge = edges[index];
                edges.RemoveAt(index);

                Node nextNode = Vector2.Distance(new Vector2((float)edge.Nodes[0].X, (float)edge.Nodes[0].Y), last) < Epsilon
                    ? edge.Nodes[1]
                    : edge.Nodes[0];

                outline.Add(new Vector3((float)nextNode.X, 0, (float)nextNode.Y));
            }

            return outline;
        }
    }
}