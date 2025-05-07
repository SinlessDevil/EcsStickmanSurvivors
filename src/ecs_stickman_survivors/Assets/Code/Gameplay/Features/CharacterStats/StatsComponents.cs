using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.CharacterStats
{
    [Game] public class BaseStats : IComponent { public Dictionary<Stats, float> Value; }
    [Game] public class StatModifiers : IComponent { public Dictionary<Stats, float> Value; }
    [Game] public class StatChange : IComponent { public Stats Value; }
}