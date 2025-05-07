using Code.Common.Extensions;
using Code.Gameplay.Common.Physics;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class CastForPullablesSytem : IExecuteSystem
    {
        private readonly IPhysicsService _physicsService;
        private readonly IGroup<GameEntity> _looters;
        private readonly GameEntity[] _hitBuffer = new GameEntity[128];
        private readonly int _layerMask = CollisionLayer.Collectable.AsMask();

        public CastForPullablesSytem(GameContext game, IPhysicsService physicsService)
        {
            _physicsService = physicsService;
            _looters = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.WorldPosition,
                    GameMatcher.PickupRadius));
        }

        public void Execute()
        {
            foreach (GameEntity loot in _looters)
            {
                for (int i = 0; i < LootingRadius(loot); i++)
                {
                    if (_hitBuffer[i].isPullable)
                    {
                        _hitBuffer[i].isPullable = false;
                        _hitBuffer[i].isPulling = true;
                    }
                }

                ClearBuffer();
            }
        }


        private int LootingRadius(GameEntity loot)
        {
            return _physicsService
                .CircleCastNonAlloc(loot.worldPosition.Value, radius: loot.PickupRadius, _layerMask, _hitBuffer);
        }
        
        private void ClearBuffer()
        {
            for (int i = 0; i < _hitBuffer.Length; i++)
            {
                _hitBuffer[i] = null;
            }
        }
    }
}