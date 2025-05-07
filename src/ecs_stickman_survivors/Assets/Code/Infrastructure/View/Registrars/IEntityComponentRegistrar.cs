namespace Code.Infrastructure.View.Registrars
{
    public interface IEntityComponentRegistrar
    {
        public void RegisterComponents();
        public void UnregisterComponents();
    }
}