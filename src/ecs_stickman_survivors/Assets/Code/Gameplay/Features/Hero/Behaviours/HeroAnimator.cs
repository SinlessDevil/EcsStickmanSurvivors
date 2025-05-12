using System.Linq;
using Code.Gameplay.Common.Visuals;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Code.Gameplay.Features.Hero.Behaviours
{
    public class HeroAnimator : MonoBehaviour, IDamageTakenAnimator
    {
        private readonly int _isMovingHash = Animator.StringToHash("isMoving");
        private readonly int _attackHash = Animator.StringToHash("attack");
        private readonly int _diedHash = Animator.StringToHash("died");

        [SerializeField] private Animator _aniamtor;
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        [Space(10)] 
        [SerializeField] private Material _materialDamageTaken;
        [SerializeField] private Material _materialHealTaken;
        [SerializeField] private int _materialIndex = 1;
        [SerializeField] private int _blinkCount = 2;
        [SerializeField] private float _blinkDuration = 0.1f;
        
        private Material[] _originalMaterials;
        private Material _cachedBlinkMaterial;

        private void Awake()
        {
            _originalMaterials = _skinnedMeshRenderer.materials.ToArray();
        }
        
        public void PlayMove() => _aniamtor.SetBool(_isMovingHash, true);
        
        public void PlayIdle() => _aniamtor.SetBool(_isMovingHash, false);
        
        public void PlayDied() => _aniamtor.SetTrigger(_diedHash);

        [Button]
        public void PlayDamageTaken()
        {
            if (DOTween.IsTweening(Material))
                return;

            BlinkEffect(_materialDamageTaken);
        }

        [Button]
        public void PlayHealTaken()
        {
            if (DOTween.IsTweening(Material))
                return;

            BlinkEffect(_materialHealTaken);
        }
        
        public void ResetAll()
        {
            _aniamtor.ResetTrigger(_attackHash);
            _aniamtor.ResetTrigger(_diedHash);
        }
        
        private void BlinkEffect(Material blinkMaterial)
        {
            if (_skinnedMeshRenderer == null || blinkMaterial == null)
                return;

            var sequence = DOTween.Sequence();
            var mats = _skinnedMeshRenderer.materials;

            for (int i = 0; i < _blinkCount; i++)
            {
                sequence.AppendCallback(() =>
                {
                    mats[_materialIndex] = blinkMaterial;
                    _skinnedMeshRenderer.materials = mats;
                });

                sequence.AppendInterval(_blinkDuration);

                sequence.AppendCallback(() =>
                {
                    mats[_materialIndex] = _originalMaterials[_materialIndex];
                    _skinnedMeshRenderer.materials = mats;
                });

                sequence.AppendInterval(_blinkDuration);
            }

            sequence.SetTarget(this);
        }

        
        private Material Material => _skinnedMeshRenderer.materials[1];
    }
}