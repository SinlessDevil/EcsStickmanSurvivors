using Code.Gameplay.Features.Abilities.Upgrade;
using Entitas;

namespace Code.Gameplay.Features.LevelUp
{
    public class UpgradeAbilityRequestSystem : IExecuteSystem
    {
        private readonly IAbilityUpgradeService _abilityUpgradeService;
        
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _levelUps;

        public UpgradeAbilityRequestSystem(GameContext game, IAbilityUpgradeService abilityUpgradeService)
        {
            _abilityUpgradeService = abilityUpgradeService;

            _levelUps = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.LevelUp));
            
            _requests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.UpgradeRequest,
                    GameMatcher.AbilityId));
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests)
            foreach (GameEntity levelUp in _levelUps)
            {
                _abilityUpgradeService.UpgradeAbility(request.AbilityId);

                levelUp.isProcessed = true;
                request.isDestructed = true;
            }
        }
    }
}