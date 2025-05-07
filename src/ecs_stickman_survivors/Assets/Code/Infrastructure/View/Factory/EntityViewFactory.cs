using Code.Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.View.Factory
{
    public class EntityViewFactory : IEntityViewFactory
    {
        private readonly Vector3 _farAway = new Vector3(-999, 999, 0);
        
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;

        public EntityViewFactory(
            IInstantiator instantiator, 
            IAssetProvider assetProvider)
        {
            _instantiator = instantiator;
            _assetProvider = assetProvider;
        }

        public EntityBehaviour CreateViewForEntity(GameEntity entity)
        {
            EntityBehaviour prefab = _assetProvider.LoadAsset<EntityBehaviour>(entity.ViewPath);
            EntityBehaviour view = _instantiator.InstantiatePrefabForComponent<EntityBehaviour>(
                prefab,
                position: _farAway,
                Quaternion.identity,
                parentTransform: null);
            
            view.SetEntity(entity);
            return view;
        }
        
        public EntityBehaviour CreateViewForEntityFromPrefab(GameEntity entity)
        {
            EntityBehaviour view = _instantiator.InstantiatePrefabForComponent<EntityBehaviour>(
                entity.ViewPrefab,
                position: _farAway,
                Quaternion.identity,
                parentTransform: null);
            
            view.SetEntity(entity);
            return view;
        }

    }
}