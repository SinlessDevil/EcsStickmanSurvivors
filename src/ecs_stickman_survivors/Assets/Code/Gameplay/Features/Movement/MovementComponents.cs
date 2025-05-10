using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Movement
{
  [Game] public class Speed : IComponent { public float Value; }
  
  [Game] public class Moving : IComponent { }
  [Game] public class MovementAvailable : IComponent { }
  [Game] public class Direction : IComponent { public Vector3 Value; }
  [Game] public class Destination : IComponent { public Vector3 Value; }
  [Game] public class RotationAlignedByDirection : IComponent { }
  [Game] public class TurnedAlongDirection : IComponent { }
  
  [Game] public class OrbitRadius : IComponent { public float Value; }
  [Game] public class OrbitPhase : IComponent { public float Value; }
  [Game] public class OrbitCenterPosition : IComponent { public Vector3 Value; }
  [Game] public class OrbitCenterFollowTarget : IComponent { public int Value; }
  
  [Game] public class TargetPosition : IComponent { public Vector3 Value; }
  [Game] public class MovingProcess : IComponent { }
}