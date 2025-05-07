using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class DestroyAbilityEntitiesOnUpgrade : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _upgradeRequests;
        private readonly IGroup<GameEntity> _abilities;

        public DestroyAbilityEntitiesOnUpgrade(GameContext game)
        {
            _game = game;
            
            _abilities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.AbilityId,
                    GameMatcher.RecreatedOnUpgrade));
            
            _upgradeRequests = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.UpgradeRequest,
                    GameMatcher.AbilityId));
        }

        public void Execute()
        {
            foreach (GameEntity upgradeRequest in _upgradeRequests)
            foreach (GameEntity ability in _abilities)
            {
                if (upgradeRequest.AbilityId == ability.AbilityId)
                {
                    foreach (GameEntity entity in _game.GetEntitiesWithParentAbility(ability.AbilityId))
                        entity.isDestructed = true;

                    ability.isActive = false;
                }
            }
        }
    }
}