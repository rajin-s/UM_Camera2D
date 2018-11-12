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
        /// <summary>All currently active focal points</summary>
        /// <access>protected List&lt;CameraFocalPoint&gt;</access>
        protected List<CameraFocalPoint> activeFocalPoints = new List<CameraFocalPoint>();

        /// <summary>Add a new focal point to be included in average calculation</summary>
        /// <access>public void</access>
        /// <param name="target" type="CameraFocalPoint">CameraFocalPoint to track</param>
        public void AddFocalPoint(CameraFocalPoint target)
        {
            activeFocalPoints.Add(target);
        }

        /// <summary>Remove a focal point from average calculation</summary>
        /// <access>public bool</access>
        /// <param name="target" type="CameraFocalPoint">CameraFocalPoint to remove</param>
        /// <returns>True if any targets were removed, false otherwise</returns>
        public bool RemoveFocalPoint(CameraFocalPoint target)
        {
            return activeFocalPoints.Remove(target);
        }

        /// <summary>Base focal point to use as distance reference</summary>
        [SerializeField]
        protected CameraFocalPoint baseFocalPoint;

        /// <summary>Enum for whether distance calculation should be done relative to the base target (ie a player) or to the camera.</summary>
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
            /// <summary>The target position to move toward</summary>
            ///<access>public readonly Vector2</access>
            public readonly Vector2 position;

            /// <summary>The speed at which to move toward the target</summary>
            ///<access>public readonly float</access>
            public readonly float speed;

            /// <summary>The zoom value at the target</summary>
            ///<access>public readonly float</access>
            public readonly float zoom;

            /// <summary>The pull value at the target</summary>
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

        /// <summary>Get the final target information based on some calculation (in child classes)</summary>
        /// <returns>A MovementTarget structure containing the target point and speed to move there</returns>
        /// <seealso cref="CameraFocus.MovementTarget" />
        ///<access>public virtual MovementTarget</access>
        public virtual MovementTarget GetMovementTarget() { return new MovementTarget(); }
    }
}