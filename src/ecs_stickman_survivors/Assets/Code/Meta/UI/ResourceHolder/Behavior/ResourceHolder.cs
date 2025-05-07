using Code.Meta.UI.GoldHolder.Service;
using UnityEngine;
using Zenject;
using TMPro;

namespace Code.Meta.UI.GoldHolder.Behavior
{
    public class ResourceHolder : MonoBehaviour
    {
        public ResourceTypeId resourceTypeId;
        public TextMeshProUGUI resourceText;
        public TextMeshProUGUI boostText;
        
        private IStorageUIService _storage;

        [Inject]
        private void Construct(IStorageUIService storageUIService)
        {
            _storage = storageUIService;
        }
        
        private void Start()
        {
            _storage.ResourceChangedEvent += OnResourceChanged;
            _storage.ResourceBoostChangedEvent += OnUpdateBoost;
            
            OnResourceChanged(resourceTypeId);
            OnUpdateBoost(resourceTypeId);
        }

        private void OnUpdateBoost(ResourceTypeId type)
        {
            if (type != resourceTypeId) return;
            
            float boost = _storage.GetResourceBoost(type);
            
            if (boost > 0)
            {
                boostText.gameObject.SetActive(true);
                boostText.text = boost.ToString("+0%");
            }
            else
            {
                boostText.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _storage.ResourceChangedEvent -= OnResourceChanged;
            _storage.ResourceBoostChangedEvent -= OnUpdateBoost;
        }

        private void OnResourceChanged(ResourceTypeId type)
        {
            if (type != resourceTypeId) return;
            
            resourceText.text = _storage.GetResource(type).ToString("0");
        }
    }
}