using Code.Gameplay.Windows.Factory;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class UIInitializer : MonoBehaviour, IInitializable
    {
        public RectTransform UIRoot;
        
        private IWindowFactory _windowFactory;

        [Inject]
        private void Construct(IWindowFactory windowFactory)
        {
            _windowFactory = windowFactory;
        }

        public void Initialize()
        {
            _windowFactory.SetUIRoot(UIRoot);
        }
    }
}