using Code.Gameplay.Features.LevelUp.Behaviours;
using Code.Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.LevelUp.UIFactory
{
    public class AbilityUIFactory : IAbilityUIFactory
    {
        private const string AbilityCardPrefabPath = "UI/Abilities/AbilityCard";
        
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;
        
        public AbilityUIFactory(IInstantiator instantiator, IAssetProvider assetProvider)
        {
            _instantiator = instantiator;
            _assetProvider = assetProvider;
        }

        public AbilityCard CreateAbilityCard(Transform parent)
        {
            return _instantiator.InstantiatePrefabForComponent<AbilityCard>(
                _assetProvider.LoadAsset<AbilityCard>(AbilityCardPrefabPath), parent);
        }
    }
}