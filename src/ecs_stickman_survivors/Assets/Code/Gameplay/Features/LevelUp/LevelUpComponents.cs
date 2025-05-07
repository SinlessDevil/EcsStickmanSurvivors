using Code.Gameplay.Features.LevelUp.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.LevelUp
{
    [Game] public class ExperienceMeterComponent : IComponent { public ExperienceMeter Value; }
    [Game] public class LevelUp : IComponent { }
}
