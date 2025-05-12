using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        GameObject LoadAsset(string path);
        T LoadAsset<T>(string path) where T : Component;
    }
}