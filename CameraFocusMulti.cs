/*
    UModules::CameraFocusMulti

    by: Rajin Shankar
    part of: UM_Camera2D

    available to use according to UM_Camera2D/LICENSE
 */

using System.Collections.Generic;
using UnityEngine;

namespace UModules
{
    /// <summary>
    /// Camera targeting component that takes a weighted average of a set of influences.
    /// Used by the CameraMotion component to provide smooth tracking of multiple targets
    /// </summary>
    /// <module>UM_Camera2D</module>
    public class CameraFocusMulti : CameraFocus
    {
        /// <summary>
        /// Curve to define how much a focal point should affect the camera over its max distance
        /// </summary>
        /// <access>protected CurveAsset</access>
        [SerializeField]
        protected CurveAsset distanceAttenuationCurve;
        
        /// <summary>
        /// Get the final movement target based on average of all target items
        /// </summary>
        /// <access>public override MovementTarget</access>
        /// <returns>A MovementTarget structure containing the target point and speed to move there</returns>
        public override MovementTarget GetMovementTarget()
        {
            Vector3 referencePosition = calculationMode == DistanceCalculationMode.RelativeToCamera ? transform.position : baseTarget.transform.position;

            // Default values if no influence
            Vector3 finalPoint = transform.position;
            float finalZoom = 1, finalPull = 0, finalSpeed = 1, totalWeight = 0;

            // Start with base values scaled by base weight
            if (baseTarget != null && baseTarget.transform != null)
            {
                finalPoint = baseTarget.transform.position * baseTarget.weight;
                finalZoom = baseTarget.zoom * baseTarget.weight;
                finalPull = baseTarget.pull * baseTarget.weight;
                finalSpeed = baseTarget.speed * baseTarget.weight;
                totalWeight = baseTarget.weight;
            }
            

            // Sum targets based on weight and distance
            for (int i = 0; i < activeTargets.Count; i++)
            {
                TargetItem target = activeTargets[i];
                Vector3 targetPosition = target.transform.position;
                if (Vector2.SqrMagnitude(referencePosition - targetPosition) < target.maxDistance * target.maxDistance)
                {
                    float distance = Vector2.Distance(target.transform.position, referencePosition);
                    float distanceScale = distanceAttenuationCurve.Evaluate(distance / target.maxDistance);
                    float adjustedWeight = target.weight * distanceScale;
                    finalPoint += target.transform.position * adjustedWeight;
                    finalZoom += target.zoom * adjustedWeight;
                    finalPull += target.pull * adjustedWeight;
                    finalSpeed += target.speed * adjustedWeight;
                    totalWeight += adjustedWeight;
                }
            }

            // Get final point and speed values based on total weight
            finalPoint /= totalWeight;
            finalZoom /= totalWeight;
            finalPull /= totalWeight;
            finalSpeed /= totalWeight;

            return new MovementTarget(finalPoint, finalZoom, finalPull, finalSpeed);
        }

    }
}