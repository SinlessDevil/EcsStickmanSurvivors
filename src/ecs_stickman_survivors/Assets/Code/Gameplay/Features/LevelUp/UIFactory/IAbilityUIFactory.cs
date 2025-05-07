using Code.Gameplay.Features.LevelUp.Behaviours;
using UnityEngine;

namespace Code.Gameplay.Features.LevelUp.UIFactory
{
    public interface IAbilityUIFactory
    {
        AbilityCard CreateAbilityCard(Transform parent);
    }
}