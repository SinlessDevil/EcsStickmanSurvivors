using Code.Gameplay.Features.Armaments.Behaviours;
using Code.Infrastructure.View.Registrars;

namespace Code.Gameplay.Features.Armaments.Registrars
{
    public class ArmamentBombVisualRegistrar : EntityComponentRegistrar
    {
        public ArmamentBombVisuals ArmamentBombVisuals;
        
        public override void RegisterComponents()
        {
            Entity.AddArmamentBombVisual(ArmamentBombVisuals);
        }

        public override void UnregisterComponents()
        {
            if(Entity.hasArmamentBombVisual)
                Entity.RemoveArmamentBombVisual();
        }
    }
}