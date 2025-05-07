using System.Collections.Generic;

namespace Code.Gameplay.Features.Abilities.Upgrade
{
  public interface IAbilityUpgradeService
  {
    void UpgradeAbility(AbilityId ability);
    void InitializeAbility(AbilityId ability);
    List<AbilityUpgradeOption> GetUpgradeOptions();
    int GetAbilityLevel(AbilityId abilityId);
    void Cleanup();
  }
}