using UnityEngine;

namespace Code.Gameplay.Features.Visuals.Behaviors
{
    public class Sheep : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        
        public void PlayBouncing()
        {
            _animator.CrossFade("Bouncing", 0.1f);
        }
    }
}