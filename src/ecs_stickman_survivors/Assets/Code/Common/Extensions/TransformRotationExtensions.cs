using UnityEngine;

namespace Code.Common.Extensions
{
    public static class TransformRotationExtensions
    {
        public static void SmoothRotateToDirection(this Transform transform, Vector3 direction, float rotationSpeed)
        {
            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}