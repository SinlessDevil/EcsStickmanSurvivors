using Code.Gameplay.Features.Visuals.Behaviors;
using Code.Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Visuals.Factory
{
    public class VisualFactory : IVisualFactory
    {
        private const string VisualPrefabPath = "Prefabs/Visuals/Sheep";
        
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;

        public VisualFactory(IInstantiator instantiator, IAssetProvider assetProvider)
        {
            _instantiator = instantiator;
            _assetProvider = assetProvider;
        }
        
        public Sheep CreateSheep(Vector3 at, Transform parent)
        {
            var sheep = _instantiator.InstantiatePrefabForComponent<Sheep>(
                _assetProvider.LoadAsset<Sheep>(VisualPrefabPath));

            sheep.transform.SetParent(parent);
            sheep.transform.localPosition = at;

            return sheep;
        }
    }
}