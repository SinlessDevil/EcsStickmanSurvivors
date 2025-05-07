using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.Enchants.Systems
{
    public class RemoveUnappliedEnchantFromHolder : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _holders;

        public RemoveUnappliedEnchantFromHolder(GameContext gameContext) : base(gameContext)
        {
            _holders = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.EnchantHolder));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.EnchantTypeId, 
                GameMatcher.Unapplied).Added());

        protected override bool Filter(GameEntity gameEntity) => true;

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity entity in entities)
            foreach (GameEntity holder in _holders)
                holder.EnchantHolder.RemoveEnchant(entity.EnchantTypeId);
        }
    }
}