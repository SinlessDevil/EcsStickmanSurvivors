using Code.Common.EntityIndices;
using Entitas;

namespace Code.Gameplay.Features.CharacterStats.Systems
{
    public class StatChangeSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _statOwners;

        public StatChangeSystem(GameContext game)
        {
            _game = game;

            _statOwners = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Id,
                    GameMatcher.BaseStats,
                    GameMatcher.StatModifiers));
        }

        public void Execute()
        {
            foreach (GameEntity statOwner in _statOwners)
            foreach (Stats statsKey in statOwner.BaseStats.Keys)
            {
                statOwner.StatModifiers[statsKey] = 0;
                
                foreach (GameEntity statChange in _game.TargetStatChanges(statsKey, statOwner.Id))
                    statOwner.StatModifiers[statsKey] += statChange.EffectValue;
            }
        }
    }
}