using Code.Gameplay.Common.Visuals.Enchants;
using Code.Infrastructure.View.Registrars;

namespace Code.Gameplay.Features.Enchants.Registrar
{
    public class EnchantVisualsRegistrar : EntityComponentRegistrar
    {
        public EnchantVisuals EnchantVisuals;
        
        public override void RegisterComponents()
        {
            Entity.AddEnchantVisuals(EnchantVisuals);
        }

        public override void UnregisterComponents()
        {
            if(Entity.hasEnchantVisuals)
                Entity.RemoveEnchantVisuals();
        }
    }
}