using Code.Gameplay.Features.Statuses.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Statuses
{
    public class StatusFeature : Feature
    {
        public StatusFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<StatusDurationSystem>());
            Add(systemFactory.Create<PeriodicDamageStatusSystem>());
            Add(systemFactory.Create<ApplyFreezeStatusSystem>());
            Add(systemFactory.Create<ApplySpeedUpStatusSystem>());
            Add(systemFactory.Create<ApplyMaxHpUpStatusSystem>());
            Add(systemFactory.Create<ApplyInvulnerabilityStatusSystem>());
            Add(systemFactory.Create<ApplyHexStatusSystem>());
            
            //Sub features
            Add(systemFactory.Create<StatusVisualsFeature>());

            Add(systemFactory.Create<CleanupUnappliedStatusLinkedChanges>());
            Add(systemFactory.Create<CleanupUnappliedStatuses>());
        }
    }
}