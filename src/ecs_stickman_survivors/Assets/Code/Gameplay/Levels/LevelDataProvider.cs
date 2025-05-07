using UnityEngine;

namespace Code.Gameplay.Levels
{
  public class LevelDataProvider : ILevelDataProvider
  {
    public Vector3 StartPoint { get; private set; }

    public void SetStartPoint(Vector3 startPoint)
    {
      StartPoint = startPoint;
    }
  }
}