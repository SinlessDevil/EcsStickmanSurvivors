using Code.Gameplay.Features.Enchants.Behaviors;
using Code.Gameplay.Features.Enchants.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Enchants.UIFactories
{
    public class EnchantUIFactory : IEnchantUIFactory
    {
        private const string EnchantPrefabPath = "UI/Enchants/Enchant";
        
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;
        private readonly IStaticDataService _staticDataService;

        public EnchantUIFactory(IInstantiator instantiator,
            IAssetProvider assetProvider,
            IStaticDataService staticDataService)
        {
            _instantiator = instantiator;
            _assetProvider = assetProvider;
            _staticDataService = staticDataService;
        }

        public Enchant CreateEnchant(Transform transform, EnchantTypeId enchantTypeId)
        {
            EnchantConfig config = _staticDataService.GetEnchantConfig(enchantTypeId);
            Enchant enchant = _instantiator
                .InstantiatePrefabForComponent<Enchant>(_assetProvider.LoadAsset<Enchant>(EnchantPrefabPath),
                    transform);
            enchant.Set(config);
            return enchant;
        }
    }
}