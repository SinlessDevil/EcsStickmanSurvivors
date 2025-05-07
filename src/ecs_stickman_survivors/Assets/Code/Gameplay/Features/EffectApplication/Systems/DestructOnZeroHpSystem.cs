using Entitas;

namespace Code.Gameplay.Features.EffectApplication.Systems
{
    public class DestructOnZeroHpSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public DestructOnZeroHpSystem(GameContext game)
        {
            _entities = game.GetGroup(GameMatcher.CurrentHp);
        }
        
        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if(entity.CurrentHp <= 0)
                    entity.isDestructed = true;
            }
        }
    }
}