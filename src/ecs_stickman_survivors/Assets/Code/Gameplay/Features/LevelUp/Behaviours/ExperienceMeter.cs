using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Features.LevelUp.Behaviours
{
    public class ExperienceMeter : MonoBehaviour
    {
        public Slider ProgressBar;
        public Image Fill;
        public float AnimationDuration = 0.5f;

        public void SetExperience(float experience, float experienceToNextLevel)
        {
            Fill.type = Image.Type.Tiled;
            float targetValue = experience / experienceToNextLevel;

            ProgressBar.DOKill();

            ProgressBar.DOValue(targetValue, AnimationDuration)
                .SetEase(Ease.OutQuad);
        }
    }
}