using System.Collections;
using UnityEngine;

namespace Code.Gameplay.Features.Armaments.Behaviours
{
    public class ArmamentBombVisuals : MonoBehaviour, IArmamentBombVisuals
    {
        [SerializeField] private GameObject _burningArea;
        [SerializeField] private GameObject _explosionAnimation;
        [SerializeField] private GameObject _armamentBomb;
        
        public void PlayExplosionArmament()
        {
            StartCoroutine(ExplosionArmamentRoutine());
        }

        public void HideArmament()
        {
            _burningArea.SetActive(false);
            _explosionAnimation.SetActive(false);
            _armamentBomb.SetActive(false);
        }
        
        private IEnumerator ExplosionArmamentRoutine()
        {
            _explosionAnimation.SetActive(true);
            _armamentBomb.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            _burningArea.SetActive(true);
        }
    }
}