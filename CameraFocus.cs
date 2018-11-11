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
    /// <module>UM_Camera2D</module>
    [DisallowMultipleComponent]
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
            ///<access>public Transform</access>
            public Transform transform;

            /// <summary>
            /// Weight used to blend or prioritize targets
            /// </summary>
            ///<access>public float</access>
            public float weight = 100;
            /// <summary>
            /// Max influence distance
            /// </summary>
            ///<access>public float</access>
            public float maxDistance = 10;

            /// <summary>
            /// Target zoom when the camera is looking at this directly
            /// </summary>
            ///<access>public float</access>
            [Range(0.25f, 4f)]
            public float zoom = 1;
            /// <summary>
            /// Target pull when the camera is looking at this directly
            /// </summary>
            ///<access>public float</access>
            [Range(-1f, 1f)]
            public float pull = 0;
            /// <summary>
            /// Camera speed multiplier
            /// </summary>
            ///<access>public float</access>
            public float speed = 1;

            public TargetItem(Transform transform, float weight, float maxDistance, float zoom, float pull, float speed)
            {
                this.transform = transform;
                this.weight = weight;
                this.maxDistance = maxDistance;
                this.speed = speed;
                this.zoom = zoom;
                this.pull = pull;
            }

            /// <summary>
            /// Compare a target item to another to determine ordering.
            /// Higher weight values will come before lower ones.
            /// </summary>
            /// <access>public int</access>
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
        ///<access>protected TargetItem</access>
        [SerializeField]
        protected TargetItem baseTarget = new TargetItem(null, 500, 0, 1, 0, 1);

        /// <summary>
        /// Set the values of the base target item
        /// </summary>
        /// <access>public TargetItem</access>
        /// <param name="target">Transform to always track</param>
        /// <param name="weight">Base weight</param>
        /// <param name="speed">Base movement speed</param>
        /// <returns>The base target item</returns>
        public TargetItem SetBaseTarget(Transform target, float weight, float zoom, float pull, float speed)
        {
            baseTarget.transform = target;
            baseTarget.weight = weight;
            baseTarget.zoom = zoom;
            baseTarget.pull = pull;
            baseTarget.speed = speed;
            return baseTarget;
        }
        /// <summary>
        /// Set the base target item transform
        /// </summary>
        /// <access>public TargetItem</access>
        /// <param name="target">Transform to always track</param>
        /// <returns>The base target item</returns>
        public TargetItem SetBaseTarget(Transform target)
        {
            return SetBaseTarget(target, baseTarget.weight, baseTarget.zoom, baseTarget.pull, baseTarget.speed);
        }

        /// <summary>
        /// List of active targets to include in average calculation
        /// </summary>
        ///<access>protected List&lt;TargetItem&gt;</access>
        protected List<TargetItem> activeTargets = new List<TargetItem>();

        /// <summary>
        /// Add a new target item to be included in average calculation
        /// </summary>
        /// <access>public TargetItem</access>
        /// <param name="target">Transform to track</param>
        /// <param name="weight">Weight of new target</param>
        /// <param name="speed">Speed to move toward new object</param>
        /// <param name="maxDistance">Max influence distance</param>
        /// <returns>The newly created target item</returns>
        public TargetItem AddTarget(Transform target, float weight, float maxDistance, float zoom, float pull, float speed)
        {
            TargetItem newTarget = new TargetItem(target, weight, maxDistance, zoom, pull, speed);
            activeTargets.Add(newTarget);
            // targets.Sort();
            return newTarget;
        }

        /// <summary>
        /// Remove all targets with a given transform
        /// </summary>
        /// <access>public bool</access>
        /// <param name="target">The transform to compare against</param>
        /// <returns>True if any targets were removed, false otherwise</returns>
        public bool RemoveTarget(Transform target)
        {
            int removed = activeTargets.RemoveAll((x) => x.transform == target);
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
        ///<access>protected DistanceCalculationMode</access>
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
            ///<access>public readonly Vector2</access>
            public readonly Vector2 position;
            /// <summary>
            /// The speed at which to move toward the target
            /// </summary>
            ///<access>public readonly float</access>
            public readonly float speed;
            /// <summary>
            /// The zoom value at the target
            /// </summary>
            ///<access>public readonly float</access>
            public readonly float zoom;
            /// <summary>
            /// The pull value at the target
            /// </summary>
            ///<access>public readonly< float/access>
            public readonly float pull;

            public MovementTarget(Vector2 position, float zoom, float pull, float speed)
            {
                this.position = position;
                this.zoom = zoom;
                this.pull = pull;
                this.speed = speed;
            }
        }

        /// <summary>
        /// Get the final target information based on some calculation (in child classes)
        /// </summary>
        /// <returns>A MovementTarget structure containing the target point and speed to move there</returns>
        /// <seealso cref="CameraFocus.MovementTarget" />
        ///<access>public virtual MovementTarget</access>
        public virtual MovementTarget GetMovementTarget() { return new MovementTarget(); }
    }
}