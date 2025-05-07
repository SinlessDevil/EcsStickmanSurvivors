using UnityEngine;

namespace Code.Gameplay.Features.Armaments.Extensions
{
 
    public static class IntExtensions
    {
        public static Vector2 GetDirectionByRadian(this int index, float spreadAngle, int projectileCount)
        {
            float step = spreadAngle / projectileCount;
            float angle = -spreadAngle / 2 + index * step;
            float radian = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }   
}