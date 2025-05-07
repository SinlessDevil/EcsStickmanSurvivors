using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Features.Abilities
{
    [Game] public class AbilityIdComponent : IComponent { public AbilityId Value; }
    [Game] public class ParentAbility : IComponent { [EntityIndex] public AbilityId Value; }
    [Game] public class FollowingProducer : IComponent { }
    [Game] public class UpgradeRequest : IComponent { }
    [Game] public class RecreatedOnUpgrade : IComponent { }
    
    [Game] public class VegetableBoltAbility : IComponent { }
    [Game] public class RadiatingCogBoltAbility : IComponent { }
    [Game] public class BouncingCoinAbility : IComponent { }
    [Game] public class ScatteringRuneStoneAbility : IComponent { }
    [Game] public class OrbitingMushroomBoltAbility : IComponent { }
    [Game] public class BombBoltAbility : IComponent { }
    
    [Game] public class GarlicAuraAbility : IComponent { }
}