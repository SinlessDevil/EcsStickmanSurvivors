using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Features.LevelUp.Configs
{
    [CreateAssetMenu(menuName = "ECS Survivors/Levelup", fileName = "LevelupConfig")]
    public class LevelupConfig : ScriptableObject
    {
        public int MaxLevel;
        public List<float> ExperienceForLevel;
    }
}