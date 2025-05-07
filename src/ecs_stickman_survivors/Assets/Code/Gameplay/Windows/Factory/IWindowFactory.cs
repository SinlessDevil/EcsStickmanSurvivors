using UnityEngine;

namespace Code.Gameplay.Windows.Factory
{
    public interface IWindowFactory
    {
        public void SetUIRoot(RectTransform uiRoot);
        public BaseWindow CreateWindow(WindowId windowId);
    }
}