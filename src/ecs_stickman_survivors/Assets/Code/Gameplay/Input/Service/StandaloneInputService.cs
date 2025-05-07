using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Gameplay.Input.Service
{
  public class StandaloneInputService : IInputService
  {
    private Camera _mainCamera;
    private Vector3 _screenPosition;

    public Camera CameraMain
    {
      get
      {
        if(_mainCamera == null && Camera.main != null)
          _mainCamera = Camera.main;
        
        return _mainCamera;
      }
    }

    public Vector2 GetScreenMousePosition() => 
      CameraMain ? (Vector2) UnityEngine.Input.mousePosition : new Vector2();

    public Vector2 GetWorldMousePosition()
    {
      if(CameraMain == null)
        return Vector2.zero;
      
      _screenPosition.x = UnityEngine.Input.mousePosition.x;
      _screenPosition.y = UnityEngine.Input.mousePosition.y;
      return CameraMain.ScreenToWorldPoint(_screenPosition);
    }

    public bool HasAxisInput() => GetHorizontalAxis() != 0 || GetVerticalAxis() != 0;
    
    public float GetVerticalAxis() => UnityEngine.Input.GetAxis("Vertical");
    public float GetHorizontalAxis() => UnityEngine.Input.GetAxis("Horizontal");
    

    public bool GetLeftMouseButton() => 
      UnityEngine.Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject();

    public bool GetLeftMouseButtonDown() =>
      UnityEngine.Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
    
    public bool GetLeftMouseButtonUp() => 
      UnityEngine.Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject();
  }
}