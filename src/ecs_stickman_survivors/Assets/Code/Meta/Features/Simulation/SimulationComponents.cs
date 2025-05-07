using Code.Progress;
using Entitas;

namespace Code.Meta.Features.Simulation
{
    [Meta] public class Tick : IComponent { public float Value; }
    [Meta] public class Duration : ISavedComponent { public float Value; }
    
    [Meta] public class GoldGainBoost : ISavedComponent { public float Value; }
    [Meta] public class GemGainBoost : ISavedComponent { public float Value; }
}