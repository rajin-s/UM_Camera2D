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
            Vector3 referencePosition;
            if (calculationMode == DistanceCalculationMode.RelativeToBaseTarget && baseFocalPoint != null)
                referencePosition = baseFocalPoint.transform.position;
            else
                referencePosition = transform.position;

            // Default values if no influence
            Vector3 finalPoint = transform.position;
            float finalZoom = 1, finalPull = 0, finalSpeed = 1, totalWeight = 0;

            // Start with base values scaled by base weight
            if (baseFocalPoint != null)
            {
                finalPoint = baseFocalPoint.transform.position * baseFocalPoint.weight;
                finalZoom = baseFocalPoint.zoom * baseFocalPoint.weight;
                finalPull = baseFocalPoint.pull * baseFocalPoint.weight;
                finalSpeed = baseFocalPoint.speed * baseFocalPoint.weight;
                totalWeight = baseFocalPoint.weight;
            }

            // Sum targets based on weight and distance
            for (int i = 0; i < activeFocalPoints.Count; i++)
            {
                CameraFocalPoint target = activeFocalPoints[i];
                Vector3 targetPosition = target.transform.position;
                if (target.weight > 0 && Vector2.SqrMagnitude(referencePosition - targetPosition) < target.maxDistance * target.maxDistance)
                {
                    float distance = Vector2.Distance(target.transform.position, referencePosition);
                    float distanceScale = distanceAttenuationCurve.Evaluate(distance / target.maxDistance);
                    float adjustedWeight = target.weight * target.influenceScale * distanceScale;
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