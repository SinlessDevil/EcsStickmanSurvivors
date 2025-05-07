using Entitas;

namespace Code.Gameplay.Features.Enchants.Systems
{
    public class AddEnchantsToHolderSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _holders;
        private readonly IGroup<GameEntity> _enchants;
        
        public AddEnchantsToHolderSystem(GameContext game)
        {
            _holders = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.EnchantHolder));
            
            _enchants = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.EnchantTypeId,
                    GameMatcher.TimeLeft));
        }

        public void Execute()
        {
            foreach (GameEntity holder in _holders)
            foreach (GameEntity enchant in _enchants)
                holder.EnchantHolder.AddEnchant(enchant.EnchantTypeId);
        }
    }
}