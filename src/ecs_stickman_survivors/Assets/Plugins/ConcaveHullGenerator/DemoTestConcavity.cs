using System.Collections.Generic;
using UnityEngine;

namespace Plugins.ConcaveHullGenerator
{
    public class DemoTestConcavity : MonoBehaviour
    {
        [SerializeField] private string _seed;
        [SerializeField] private double _scaleFactor;
        [SerializeField] private int _numberOfPoints;
        [SerializeField] private double _concavity;
        
        private List<Node> _dotList = new();
        
        private void Start()
        {
            SetDots(_numberOfPoints);
            GenerateHull();
        }

        private void GenerateHull()
        {
            Hull.SetConvexHull(_dotList);
            Hull.SetConcaveHull(_concavity, _scaleFactor);
        }

        private void SetDots(int number_of_dots)
        {
            System.Random pseudorandom = new System.Random(_seed.GetHashCode());
            for (int x = 0; x < number_of_dots; x++)
            {
                _dotList.Add(new Node(pseudorandom.Next(0, 100), pseudorandom.Next(0, 100), x));
            }

            for (int pivot_position = 0; pivot_position < _dotList.Count; pivot_position++)
            {
                for (int position = 0; position < _dotList.Count; position++)
                {
                    if (_dotList[pivot_position].X == _dotList[position].X && 
                        _dotList[pivot_position].Y == _dotList[position].Y && 
                        pivot_position != position)
                    {
                        _dotList.RemoveAt(position);
                        position--;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < Hull.HullEdges.Count; i++)
            {
                Vector2 left = new Vector2((float)Hull.HullEdges[i].Nodes[0].X, (float)Hull.HullEdges[i].Nodes[0].Y);
                Vector2 right = new Vector2((float)Hull.HullEdges[i].Nodes[1].X, (float)Hull.HullEdges[i].Nodes[1].Y);
                Gizmos.DrawLine(left, right);
            }

            Gizmos.color = Color.blue;
            foreach (var t in Hull.HullConcaveEdges)
            {
                Vector2 left = new Vector2((float)t.Nodes[0].X, (float)t.Nodes[0].Y);
                Vector2 right = new Vector2((float)t.Nodes[1].X, (float)t.Nodes[1].Y);
                Gizmos.DrawLine(left, right);
            }

            Gizmos.color = Color.red;
            foreach (var dotList in _dotList)
            {
                Gizmos.DrawSphere(new Vector3((float)dotList.X, (float)dotList.Y, 0), 0.5f);
            }
        }
    }
}