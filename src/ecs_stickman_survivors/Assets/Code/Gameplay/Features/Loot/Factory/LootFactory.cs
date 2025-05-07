using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Identifiers;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public class LootFactory : ILootFactory
    {
        private readonly IIdentifierService _identifierService;
        private readonly IStaticDataService _staticDataService;

        public LootFactory(IIdentifierService identifierService, IStaticDataService staticDataService)
        {
            _identifierService = identifierService;
            _staticDataService = staticDataService;
        }

        public GameEntity CreateLootItem(LootTypeId lootTypeId, Vector3 at)
        {
            LootConfig lootConfig = _staticDataService.GetLootConfig(lootTypeId);

            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddWorldPosition(at)
                .AddLootTypeId(lootTypeId)
                .AddViewPrefab(lootConfig.Prefab)
                .With(x => x.AddExperience(lootConfig.Experience), when: lootConfig.Experience > 0)
                .With(x => x.AddEffectSetups(lootConfig.EffectSetups), when: !lootConfig.EffectSetups.IsNullOrEmpty())
                .With(x => x.AddStatusSetups(lootConfig.StatusSetups), when: !lootConfig.StatusSetups.IsNullOrEmpty())
                .With(x => x.isPullable = true);
        }
    }
}