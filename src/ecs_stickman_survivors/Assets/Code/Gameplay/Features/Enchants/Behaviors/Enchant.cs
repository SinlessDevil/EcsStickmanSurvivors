using Code.Gameplay.Features.Enchants.Configs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Features.Enchants.Behaviors
{
    public class Enchant : MonoBehaviour
    {
        public Image Icon;
        public EnchantTypeId EnchantTypeId;

        private const float AnimationDuration = 0.5f;

        public void Set(EnchantConfig config)
        {
            Icon.sprite = config.Icon;
            EnchantTypeId = config.EnchantTypeId;

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, AnimationDuration)
                .SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            transform.DOScale(Vector3.zero, AnimationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}