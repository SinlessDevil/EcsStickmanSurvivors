using Code.Gameplay.Features.Visuals.Behaviors;
using UnityEngine;

namespace Code.Gameplay.Features.Visuals.Factory
{
    public interface IVisualFactory
    {
        Sheep CreateSheep(Vector3 at, Transform parent);
    }
}