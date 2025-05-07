using System.Collections.Generic;
using Code.Gameplay.Features.Armaments.Behaviours;
using Code.Gameplay.Features.Effects;
using Code.Gameplay.Features.Statuses;
using Entitas;

namespace Code.Gameplay.Features.Armaments
{
    [Game] public class Armament : IComponent { }
    [Game] public class TargetLimit : IComponent { public int Value; }
    [Game] public class Processed : IComponent { }
    
    [Game] public class EffectSetups : IComponent { public List<EffectSetup> Value; }
    [Game] public class StatusSetups : IComponent { public List<StatusSetup> Value; }
    [Game] public class Target : IComponent { public int Value; }
    
    [Game] public class ArmamentBombVisualComponent : IComponent { public IArmamentBombVisuals Value; }
    
    [Game] public class BounceRate : IComponent { public float Value; }
    [Game] public class Separable : IComponent { }
    [Game] public class Bomb : IComponent { }
}