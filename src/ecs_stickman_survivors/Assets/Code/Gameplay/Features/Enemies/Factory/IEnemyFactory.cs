using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Factory
{
    public interface IEnemyFactory
    {
        public GameEntity CreateEnemy(EnemyTypeId typeId, Vector3 at, int level);
    }
}