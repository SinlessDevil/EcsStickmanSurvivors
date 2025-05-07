using System.Collections.Generic;
using Code.Gameplay.Features.Effects.Extensions;
using Entitas;

namespace Code.Gameplay.Features.Statuses.Systems.StatusVisuals
{
    public class UnapplyFreezeVisualsSystem : ReactiveSystem<GameEntity>
    {
        public UnapplyFreezeVisualsSystem(GameContext gameContext) : base(gameContext)
        {
            
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher
                .AllOf(
                    GameMatcher.Freeze,
                    GameMatcher.Status,
                    GameMatcher.Unapplied
                ).Added());

        protected override bool Filter(GameEntity entity) => entity.isStatus && entity.isFreeze && entity.hasTargetId;

        protected override void Execute(List<GameEntity> statuses)
        {
            foreach (GameEntity status in statuses)
            {
                GameEntity target = status.Target();
                if (target is {hasStatusVisuals: true}) 
                    target.StatusVisuals.UnapplyFreeze();
            }
        }
    }
}