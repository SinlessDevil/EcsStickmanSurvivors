using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public GameObject LoadAsset(string path)
        {
            return Resources.Load<GameObject>(path);
        }

        public T LoadAsset<T>(string path) where T : Component
        {
            return Resources.Load<T>(path);
        }
    }
}