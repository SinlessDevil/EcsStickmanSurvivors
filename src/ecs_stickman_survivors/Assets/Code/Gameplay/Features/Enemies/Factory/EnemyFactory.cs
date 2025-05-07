using System;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.CharacterStats;
using Code.Gameplay.Features.Enemies.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Identifiers;
using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Factory
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly IIdentifierService _identifierService;
        private readonly IStaticDataService _staticDataService;

        public EnemyFactory(
            IIdentifierService identifierService, 
            IStaticDataService staticDataService)
        {
            _identifierService = identifierService;
            _staticDataService = staticDataService;
        }
        
        public GameEntity CreateEnemy(EnemyTypeId typeId, Vector3 at, int level = 1)
        {
            return typeId switch
            {
                EnemyTypeId.Goblin => CreateGoblin(typeId, at, level),
                EnemyTypeId.GoblinBig => CreateGoblinBig(typeId, at, level),
                EnemyTypeId.GoblinShamanHealer => CreateGoblinShamanHealer(typeId, at, level),
                EnemyTypeId.GoblinShamanBuffer => CreateGoblinShamanBuffer(typeId, at, level),
                _ => throw new Exception($"Enemy with type id {typeId} does not exist")
            };
        }

        private GameEntity CreateGoblin(EnemyTypeId typeId, Vector3 at, int level)
        {
            EnemyLevel enemyLevel = _staticDataService.GetEnemyLevel(typeId, level);
            
            string viewPath = "Gameplay/Enemies/Goblins/Torch/goblin_torch_blue";
            
            return CreateEnemyEntity(typeId, at, viewPath, enemyLevel)
                .AddTargetsBuffer(new List<int>(1))
                .AddRadius(enemyLevel.RadiusToCollectTargets)
                .AddCollectTargetsInterval(enemyLevel.CollectTargetsInterval)
                .AddCollectTargetsTimer(0)
                .AddLayerMask(CollisionLayer.Hero.AsMask());
        }

        private GameEntity CreateGoblinBig(EnemyTypeId typeId, Vector3 at, int level)
        {
            EnemyLevel enemyLevel = _staticDataService.GetEnemyLevel(typeId, level);
            
            string viewPath = "Gameplay/Enemies/Goblins/Torch/goblin_torch_blue_big";
            
            return CreateEnemyEntity(typeId, at, viewPath, enemyLevel)
                .AddTargetsBuffer(new List<int>(1))
                .AddRadius(enemyLevel.RadiusToCollectTargets)
                .AddCollectTargetsInterval(enemyLevel.CollectTargetsInterval)
                .AddCollectTargetsTimer(0)
                .AddLayerMask(CollisionLayer.Hero.AsMask());
        }
        
        private GameEntity CreateGoblinShamanHealer(EnemyTypeId typeId, Vector3 at, int level)
        {
            EnemyLevel enemyLevel = _staticDataService.GetEnemyLevel(typeId, level);
            
            string viewPath = "Gameplay/Enemies/Goblins/Torch/goblin_torch_blue_shaman_healer";
            
            return CreateEnemyEntity(typeId, at, viewPath, enemyLevel)
                .With(x => x.isEnemyShaman = true)
                .AddRadiusToFindEnemy(enemyLevel.RadiusToFindEnemy)
                .AddCreateEffectInterval(enemyLevel.CreateEffectInterval);
        }

        private GameEntity CreateGoblinShamanBuffer(EnemyTypeId typeId, Vector3 at, int level)
        {
            EnemyLevel enemyLevel = _staticDataService.GetEnemyLevel(typeId, level);
            
            string viewPath = "Gameplay/Enemies/Goblins/Torch/goblin_torch_blue_shaman_buffer";
            
            return CreateEnemyEntity(typeId, at, viewPath, enemyLevel)
                .With(x => x.isEnemyShaman = true)
                .AddRadiusToFindEnemy(enemyLevel.RadiusToFindEnemy)
                .AddCreateEffectInterval(enemyLevel.CreateEffectInterval);
        }
        
        private GameEntity CreateEnemyEntity(EnemyTypeId typeId, Vector3 at, string viewPath, EnemyLevel enemyLevel)
        {
            Dictionary<Stats, float> baseStats = InitStats.EmptyStatDictionary()
                .With(x => x[Stats.Speed] = enemyLevel.Speed)
                .With(x => x[Stats.MaxHp] = enemyLevel.Hp)
                .With(x => x[Stats.Damage] = 1);
            
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddEnemyTypeID(typeId)
                .AddWorldPosition(at)
                .AddDirection(Vector2.zero)
                .AddBaseStats(baseStats)
                .AddStatModifiers(InitStats.EmptyStatDictionary())
                .AddSpeed(baseStats[Stats.Speed])
                .AddCurrentHp(baseStats[Stats.MaxHp])
                .AddMaxHp(baseStats[Stats.MaxHp])
                .AddEffectSetups(enemyLevel.EffectSetups)
                .AddStatusSetups(enemyLevel.StatusSetups)
                .AddViewPath(viewPath)
                .With(x => x.isEnemy = true)
                .With(x => x.isTurnedAlongDirection = true)
                .With(x => x.isMovementAvailable = true);
        }
    }
}