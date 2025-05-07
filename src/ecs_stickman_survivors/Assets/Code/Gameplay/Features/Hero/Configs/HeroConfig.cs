using UnityEngine;

namespace Code.Gameplay.Features.Hero.Configs
{
    [CreateAssetMenu(menuName = "ECS Survivors/Heroes", fileName = "HeroConfig")]
    public class HeroConfig : ScriptableObject
    {
        public float Hp = 100;
        public float Speed = 3;
    }
}