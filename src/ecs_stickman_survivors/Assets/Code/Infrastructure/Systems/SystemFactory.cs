using Entitas;
using Zenject;

namespace Code.Infrastructure.Systems
{
    public class SystemFactory : ISystemFactory
    {
        private readonly DiContainer _container;
        
        public SystemFactory(DiContainer container)
        {
            _container = container;
        }
        
        public T Create<T>() where T : ISystem
        {
            return _container.Instantiate<T>();
        }

        public T Create<T>(params object[] args) where T : ISystem
        {
            return _container.Instantiate<T>(args);
        }
    }
}