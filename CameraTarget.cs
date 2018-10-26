using System;
using UnityEngine;

namespace UModules
{
    /// <summary>
    /// Generic camera targeting component shared by all targeting implementations.
    /// Used by the CameraMotion component to provide following behavior
    /// </summary>
    public abstract class CameraTarget : ExtendedBehaviour
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
        /// Structure holding a target focal point and a speed to move toward that point.
        /// Used by the CameraMotion component to actually move the object
        /// </summary>
        private struct MovementTarget
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
    }
}