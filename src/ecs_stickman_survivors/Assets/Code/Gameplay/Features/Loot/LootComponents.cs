using Entitas;

namespace Code.Gameplay.Features.Loot
{
    [Game] public class LootTypeIdComponent : IComponent { public LootTypeId Id; }
    [Game] public class Pullable : IComponent { }
    [Game] public class Pulling : IComponent { }
    [Game] public class Collected : IComponent { }
    
    [Game] public class PickupRadius : IComponent { public float Value;}
    
    [Game] public class Experience : IComponent { public float Value; }
}