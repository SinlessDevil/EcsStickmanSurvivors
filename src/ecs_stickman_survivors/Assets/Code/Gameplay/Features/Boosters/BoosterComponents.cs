using Entitas;

namespace Code.Gameplay.Features.Boosters
{
    [Game] public class Booster : IComponent { }
    [Game] public class BoosterSpawnTimer : IComponent { public float Value; }
}