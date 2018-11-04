/*
    UModules::CameraFocus

    by: Rajin Shankar
    part of: UM_Camera2D

    available to use according to UM_Camera2D/LICENSE
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UModules
{
    /// <summary>
    /// Generic camera targeting component shared by all targeting implementations.
    /// Used by the CameraMotion component to provide following behavior
    /// </summary>
    public abstract class CameraFocus : ExtendedBehaviour
    {
        /// <summary>
        /// Structure for representing a particular item and its influence on a camera.
        /// Comparable to other TargetItems by weight
        /// </summary>
        [Serializable]
        public class TargetItem : IComparable<TargetItem>
        {
            /// <summary>
            /// Transform used to get target point
            /// </summary>
            public Transform transform;
            /// <summary>
            /// Weight used to blend or prioritize targets
            /// </summary>
            public float weight;
            /// <summary>
            /// Speed to move toward target point
            /// </summary>
            public float speed;
            /// <summary>
            /// Max influence distance
            /// </summary>
            public float maxDistance;

            public TargetItem(Transform transform, float weight, float speed, float maxDistance)
            {
                this.transform = transform;
                this.weight = weight;
                this.speed = speed;
                this.maxDistance = maxDistance;
            }

            /// <summary>
            /// Compare a target item to another to determine ordering.
            /// Higher weight values will come before lower ones.
            /// </summary>
            /// <param name="other">The target item to compare this one's weight to</param>
            /// <returns>
            /// The relative weight of other with respect to this
            /// (negative if this > other, positive if other > this)
            /// </returns>
            public int CompareTo(TargetItem other)
            {
                return (int)(other.weight * 1000) - (int)(weight * 1000);
            }
        }

        /// <summary>
        /// Target item that is always included in average calculation
        /// </summary>
        [SerializeField]
        protected TargetItem baseTarget = new TargetItem(null, 500, 1, 0);

        /// <summary>
        /// Set the values of the base target item
        /// </summary>
        /// <param name="target">Transform to always track</param>
        /// <param name="weight">Base weight</param>
        /// <param name="speed">Base movement speed</param>
        /// <returns>The base target item</returns>
        public TargetItem SetBaseTarget(Transform target, float weight, float speed)
        {
            baseTarget.transform = target;
            baseTarget.weight = weight;
            baseTarget.speed = speed;
            return baseTarget;
        }
        /// <summary>
        /// Set the base target item transform
        /// </summary>
        /// <param name="target">Transform to always track</param>
        /// <returns>The base target item</returns>
        public TargetItem SetBaseTarget(Transform target)
        {
            return SetBaseTarget(target, baseTarget.weight, baseTarget.speed);
        }
        
        /// <summary>
        /// List of all targets to include in average calculation
        /// </summary>
        protected List<TargetItem> targets = new List<TargetItem>();

        /// <summary>
        /// Add a new target item to be included in average calculation
        /// </summary>
        /// <param name="target">Transform to track</param>
        /// <param name="weight">Weight of new target</param>
        /// <param name="speed">Speed to move toward new object</param>
        /// <param name="maxDistance">Max influence distance</param>
        /// <returns>The newly created target item</returns>
        public TargetItem AddTarget(Transform target, float weight, float speed, float maxDistance)
        {
            TargetItem newTarget = new TargetItem(target, weight, speed, maxDistance);
            targets.Add(newTarget);
            // targets.Sort();
            return newTarget;
        }

        /// <summary>
        /// Remove all targets with a given transform
        /// </summary>
        /// <param name="target">The transform to compare against</param>
        /// <returns>True if any targets were removed, false otherwise</returns>
        public bool RemoveTarget(Transform target)
        {
            int removed = targets.RemoveAll((x) => x.transform == target);
            return removed > 0;
        }

        /// <summary>
        /// Enum for whether distance calculation should be done relative to the base target (ie a player) or to the camera.
        /// </summary>
        protected enum DistanceCalculationMode
        {
            RelativeToBaseTarget,
            RelativeToCamera
        }
        /// <summary>
        /// Should distance calculation be done relative to the base target or to the camera?
        /// Note: Camera-relative distance calculation can cause motion to get stuck on one focal point.
        /// </summary>
        protected DistanceCalculationMode calculationMode = DistanceCalculationMode.RelativeToBaseTarget;

        /// <summary>
        /// Structure holding a target focal point and a speed to move toward that point.
        /// Used by the CameraMotion component to actually move the object
        /// </summary>
        public struct MovementTarget
        {
            /// <summary>
            /// The target position to move toward
            /// </summary>
            public readonly Vector2 position;
            /// <summary>
            /// The speed at which to move toward the target
            /// </summary>
            public readonly float speed;

            public MovementTarget(Vector2 position, float speed)
            {
                this.position = position;
                this.speed = speed;
            }
        }

        /// <summary>
        /// Get the final target information based on some calculation (in child classes)
        /// </summary>
        /// <returns>A MovementTarget structure containing the target point and speed to move there</returns>
        /// <seealso cref="CameraFocus.MovementTarget" />
        public virtual MovementTarget GetMovementTarget() { return new MovementTarget(); }
    }
}