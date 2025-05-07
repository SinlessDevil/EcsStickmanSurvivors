using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Configs
{
    [CreateAssetMenu(menuName = "ECS Survivors/EnemySpawn", fileName = "EnemySpawnConfig")]
    public class EnemySpawnConfig : ScriptableObject
    {
        public List<EnemyWave> Waves = new()
        {
            new()
            {
                LevelRequirement = 1,
                SpawnInterval = 3f,
                Enemies = new List<EnemySpawnData>
                {
                    new() { EnemyType = EnemyTypeId.Goblin, MinCount = 2, MaxCount = 4 },
                    new() { EnemyType = EnemyTypeId.GoblinBig, MinCount = 1, MaxCount = 2 }
                }
            },
            new EnemyWave
            {
                LevelRequirement = 3,
                SpawnInterval = 2.5f,
                Enemies = new List<EnemySpawnData>
                {
                    new() { EnemyType = EnemyTypeId.Goblin, MinCount = 3, MaxCount = 5 },
                    new() { EnemyType = EnemyTypeId.GoblinShamanHealer, MinCount = 1, MaxCount = 2 }
                }
            },
            new EnemyWave
            {
                LevelRequirement = 5,
                SpawnInterval = 2f,
                Enemies = new List<EnemySpawnData>
                {
                    new() { EnemyType = EnemyTypeId.Goblin, MinCount = 4, MaxCount = 6 },
                    new() { EnemyType = EnemyTypeId.GoblinShamanBuffer, MinCount = 1, MaxCount = 2 },
                    new() { EnemyType = EnemyTypeId.GoblinBig, MinCount = 2, MaxCount = 3 }
                }
            }
        };
    }
            
    [Serializable]
    public class EnemyWave
    {
        public int LevelRequirement;
        public float SpawnInterval;
        public List<EnemySpawnData> Enemies;
    }
        
    [Serializable]
    public class EnemySpawnData
    {
        public EnemyTypeId EnemyType;
        public int MinCount;
        public int MaxCount;
    }
}