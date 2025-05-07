using Code.Gameplay.Input.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Input
{
  public class InputFeature : Feature
  {
    public InputFeature(ISystemFactory systemFactory)
    {
      Add(systemFactory.Create<InitializeInputSystem>());
      Add(systemFactory.Create<EmitInputSystem>());
    }
  }
}