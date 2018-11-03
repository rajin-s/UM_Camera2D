using System.Collections.Generic;
using UnityEngine;

namespace UModules
{
    /// <summary>
    /// Camera targeting component that takes a weighted average of a set of influences.
    /// Used by the CameraMotion component to provide smooth tracking of multiple targets
    /// </summary>
    public class CameraFocusMulti : CameraFocus
    {
        /// <summary>
        /// Get the final movement target based on average of all target items
        /// </summary>
        /// <returns>A MovementTarget structure containing the target point and speed to move there</returns>
        public override MovementTarget GetMovementTarget()
        {
            Vector3 referencePosition = transform.position;

            // Start with base values
            Vector3 finalPoint = baseTarget.transform.position * baseTarget.weight;
            float finalSpeed = baseTarget.speed * baseTarget.weight;
            float totalWeight = baseTarget.weight;

            // Sum targets based on weight and distance
            for (int i = 0; i < targets.Count; i++)
            {
                TargetItem target = targets[i];
                Vector3 targetPosition = target.transform.position;
                if (Vector2.SqrMagnitude(referencePosition - targetPosition) < target.maxDistance * target.maxDistance)
                {
                    float distance = Vector2.Distance(target.transform.position, referencePosition);
                    float distanceScale = distanceAttenuationCurveTest.Evaluate(distance / target.maxDistance);
                    float adjustedWeight = target.weight * distanceScale;
                    finalPoint += target.transform.position * adjustedWeight;
                    finalSpeed += target.speed * adjustedWeight;
                    totalWeight += adjustedWeight;
                }
            }

            // Get final point and speed values based on total weight
            finalPoint /= totalWeight;
            finalSpeed /= totalWeight;

            return new MovementTarget(finalPoint, finalSpeed);
        }

        public CurveAsset distanceAttenuationCurveTest;
    }
}