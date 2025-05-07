using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.Enchants.Systems
{
    public class ApplyHexEnchantVisualsSystem : ReactiveSystem<GameEntity>
    {
        public ApplyHexEnchantVisualsSystem(GameContext gameContext) : base(gameContext)
        {
            
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher.
                AllOf(
                    GameMatcher.HexEnchant,
                    GameMatcher.Armament,
                    GameMatcher.EnchantVisuals)
                .Added());

        protected override bool Filter(GameEntity entity) => entity.isArmament && entity.hasEnchantVisuals;

        protected override void Execute(List<GameEntity> armaments)
        {
            foreach (GameEntity armament in armaments)
            {
                armament.EnchantVisuals.ApplyHex();
            }
        }
    }
}