using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input
{
    [Input] public class Input : IComponent { }
    [Input] public class AxisInput : IComponent { public Vector3 Value; }
}