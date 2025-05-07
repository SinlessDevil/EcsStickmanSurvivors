using Code.Gameplay.Features.Statuses;
using Code.Gameplay.Features.Statuses.Applier;
using Entitas;

namespace Code.Gameplay.Features.EffectApplication.Systems
{
    public class ApplyStatusOnProducerAfterDeathTargetSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IStatusApplier _statusApplier;
        private readonly IGroup<GameEntity> _entities;

        public ApplyStatusOnProducerAfterDeathTargetSystem(GameContext game, IStatusApplier statusApplier)
        {
            _game = game;
            _statusApplier = statusApplier;

            _entities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.TargetsBuffer,
                    GameMatcher.StatusSetups));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            foreach (int targetId in entity.TargetsBuffer)
            foreach (StatusSetup setup in entity.StatusSetups)
            {
                if (setup.StatusApplicationTypeId != StatusApplicationTypeId.Producer)
                    continue;
                
                if(setup.StatusTriggerConditionTypeId != StatusTriggerConditionTypeId.OnDeath)
                    continue;
                
                GameEntity target = _game.GetEntityWithId(targetId);

                if (target == null || !target.hasCurrentHp)
                    continue;

                if (target.CurrentHp <= 0)
                {
                    _statusApplier.ApplyStatusOnProducer(setup, targetId,  ProducerId(entity));
                }   
            }
        }

        private int ProducerId(GameEntity entity)
        {
            return entity.hasProducerId ? entity.ProducerId : entity.Id;
        }
    }
}