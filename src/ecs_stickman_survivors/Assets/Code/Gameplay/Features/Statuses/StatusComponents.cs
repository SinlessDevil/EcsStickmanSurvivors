using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Features.Statuses
{
    [Game] public class Status : IComponent { }
    [Game] public class StatusTypeIdComponent : IComponent { public StatusTypeId Value; }
    [Game] public class Duration : IComponent { public float Value; }
    [Game] public class TimeLeft : IComponent { public float Value; }
    
    [Game] public class Period : IComponent { public float Value; }
    [Game] public class TimeSinceLastTick : IComponent { public float Value; }
    [Game] public class ApplierStatusLink : IComponent { [EntityIndex] public int Value; }
    
    [Game] public class Applied : IComponent { }
    [Game] public class Affected : IComponent { }
    [Game] public class Unapplied : IComponent { }
    
    [Game] public class Poison : IComponent { }
    [Game] public class Freeze : IComponent { }
    [Game] public class SpeedUp : IComponent { }
    [Game] public class MaxHpUp : IComponent { }
    [Game] public class Invulnerability : IComponent { }
    [Game] public class Hex : IComponent { }
}