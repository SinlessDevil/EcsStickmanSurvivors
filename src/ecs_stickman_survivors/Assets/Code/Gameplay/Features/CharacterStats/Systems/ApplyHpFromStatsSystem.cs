using Code.Common.Extensions;
using Entitas;

namespace Code.Gameplay.Features.CharacterStats.Systems
{
    public class ApplyHpFromStatsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _statOwners;

        public ApplyHpFromStatsSystem(GameContext game)
        {
            _statOwners = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.BaseStats,
                    GameMatcher.StatModifiers,
                    GameMatcher.MaxHp));
        }

        public void Execute()
        {
            foreach (GameEntity statOwner in _statOwners)
            {
                statOwner.ReplaceMaxHp(MoveMaxHp(statOwner).ZeroIfNegative());
            }
        }
        
        private float MoveMaxHp(GameEntity statOwner)
        {
            return statOwner.BaseStats[Stats.MaxHp] + statOwner.StatModifiers[Stats.MaxHp];
        }
        
        private bool HasMoveMaxHp(GameEntity statOwner)
        {
            return statOwner.StatModifiers[Stats.MaxHp] != 0;
        }
    }
}