using Code.Gameplay.Features.Enemies.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Enemies
{
    [Game] public class Enemy : IComponent { }
    [Game] public class EnemyShaman : IComponent { }
    
    [Game] public class EnemyAnimatorComponent : IComponent { public EnemyAnimator Value; }
    [Game] public class EnemyTypeIDComponent : IComponent { public EnemyTypeId Value; }
    
    [Game] public class RadiusToFindEnemyComponent : IComponent { public float Value; }
    [Game] public class CreateEffectIntervalComponent : IComponent { public float Value; }
    
    [Game] public class ReloadingTimer : IComponent { } 
    [Game] public class BuffTimer : IComponent { public float Value; }
    [Game] public class SpawnTimer : IComponent { public float Value; }
}