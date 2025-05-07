using Code.Common.Entity;
using Code.Infrastructure.Identifiers;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.View
{
    public class SelfInitializedEntityView : MonoBehaviour
    {
        public EntityBehaviour EntityBehaviour;
    
        private IIdentifierService _identifiers;

        [Inject]
        private void Construct(IIdentifierService identifiers) => 
            _identifiers = identifiers;

        private void Awake()
        {
            GameEntity entity = CreateEntity.Empty()
                .AddId(_identifiers.Next());
      
            EntityBehaviour.SetEntity(entity);
        }
    }
}