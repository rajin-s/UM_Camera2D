/*
    UModules::CameraFocalPoint

    by: Rajin Shankar
    part of: UM_Camera2D

    available to use according to UM_Camera2D/LICENSE
 */

using UnityEngine;

namespace UModules
{
    /// <summary>
    /// A single focal point for a 2D camera
    /// </summary>
    /// <module>UM_Camera2D</module>
    public class CameraFocalPoint : ExtendedBehaviour
    {
        /// <summary>
        /// CameraFocus component to affect (initializes to MainCamera if null)
        /// </summary>
        [Tooltip("Initialized to Main Camera if unset")]
        [SerializeField]
        protected CameraFocus targetCameraFocus;

        // [SerializeField]
        // protected float _initialWeight = 10;
        // [SerializeField]
        // protected float _initialSpeed = 1;
        // [SerializeField]
        // protected float _initialMaxDistance = 10;

        /// <summary>
        /// Target item info used by target camera focus component. Transform defaults to the attached transform if unset.
        /// Note: the object is copied every time the focal point is activated.
        /// </summary>
        [Tooltip("Target transform defaults to this if unset")]
        [SerializeField]
        protected CameraFocus.TargetItem targetInfo;

        /// <summary>
        /// Public property to access target info weight
        /// </summary>
        /// <access>public float</access>
        public float Weight { get { return targetInfo.weight; } }
        /// <summary>
        /// Public property to access target info speed
        /// </summary>
        /// <access>public float</access>
        public float Speed { get { return targetInfo.speed; } }
        /// <summary>
        /// Public property to access target info max distance
        /// </summary>
        /// <access>public float</access>
        public float MaxDistance { get { return targetInfo.maxDistance; } }

        /// <summary>
        /// Is this focal point currently being used by the target camera focus?
        /// </summary>
        /// <value></value>
        public bool IsActive { get; protected set; }

        /// <summary>
        /// Initialize targetCameraFocus reference to MainCamera if null. 
        /// Note: Must happen after CameraExtension calls Initialize (which sets MainCamera values)
        /// </summary>
        /// <seealso cref="CameraExtension.Initialize" />
        public override void Initialize()
        {
            if (targetCameraFocus == null) targetCameraFocus = MainCamera.Focus;
        }

        /// <summary>
        /// Add the focal point to the current set of active points when it is enabled
        /// </summary>
        protected void OnEnable()
        {
            targetInfo = targetCameraFocus.AddTarget(targetInfo.transform == null ? transform : targetInfo.transform, targetInfo.weight, targetInfo.speed, targetInfo.maxDistance);
            IsActive = true;
        }

        /// <summary>
        /// Remove the focal point from the current set of active points when it is disabled or destroyed
        /// </summary>
        protected void OnDisable()
        {
            targetCameraFocus.RemoveTarget(transform);
            IsActive = false;
        }
    }
}