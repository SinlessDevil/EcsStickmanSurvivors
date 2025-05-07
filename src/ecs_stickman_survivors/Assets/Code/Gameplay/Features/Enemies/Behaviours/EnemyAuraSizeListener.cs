using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Behaviours
{
    public class EnemyAuraSizeListener : EntityDependant
    {
        public Transform Container;
        private float _radiusPrev;

        private void Update()
        {
            if (Mathf.Abs(Entity.RadiusToFindEnemy - _radiusPrev) < Mathf.Epsilon)
                return;
            
            SetAuraScale();
        }

        private void SetAuraScale()
        {
            float scale = Entity.RadiusToFindEnemy * 2;
            Container.localScale = new Vector3(scale, scale, scale);
            
            _radiusPrev = Entity.RadiusToFindEnemy;
        }
    }
}