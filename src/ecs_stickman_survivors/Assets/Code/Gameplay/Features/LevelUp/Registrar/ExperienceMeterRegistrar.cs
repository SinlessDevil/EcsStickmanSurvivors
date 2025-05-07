using Code.Gameplay.Features.LevelUp.Behaviours;
using Code.Infrastructure.View.Registrars;

namespace Code.Gameplay.Features.LevelUp.Registrar
{
    public class ExperienceMeterRegistrar : EntityComponentRegistrar
    {
        public ExperienceMeter ExperienceMeter;
        
        public override void RegisterComponents() =>
        Entity.AddExperienceMeter(ExperienceMeter);

        public override void UnregisterComponents()
        {
            if (Entity.hasExperienceMeter)
                Entity.RemoveExperienceMeter();
        }
    }
}