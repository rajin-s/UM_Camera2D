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
        /// Should this focal point target the main camera?
        /// </summary>
        /// <access>protected bool</access>
        [SerializeField]
        protected bool useMainCamera = true;
        /// <summary>
        /// CameraFocus component to affect (initializes to main camera if null or useMainCamera is true)
        /// </summary>
        /// <access>protected CameraFocus</access>
        [DontShowIf("useMainCamera")]
        [SerializeField]
        protected CameraFocus targetCameraFocus;

        /// <summary>
        /// Target item info used by target camera focus component. Transform defaults to the attached transform if unset.
        /// Note: the object is copied every time the focal point is activated.
        /// </summary>
        /// <access>protected CameraFocus.TargetItem</access>
        [Tooltip("Target transform defaults to this if unset")]
        [SerializeField]
        protected CameraFocus.TargetItem targetInfo;

        /// <summary>
        /// Public property to access target info weight
        /// </summary>
        /// <access>public float</access>
        public float Weight { get { return targetInfo.weight; } }
        /// <summary>
        /// Public property to access target info max distance
        /// </summary>
        /// <access>public float</access>
        public float MaxDistance { get { return targetInfo.maxDistance; } }

        /// <summary>
        /// Public property to access target info zoom
        /// </summary>
        /// <access>public float</access>
        public float Zoom { get { return targetInfo.zoom; } }
        /// <summary>
        /// Public property to access target info pull
        /// </summary>
        /// <access>public float</access>
        public float Pull { get { return targetInfo.pull; } }
        /// <summary>
        /// Public property to access target info speed
        /// </summary>
        /// <access>public float</access>
        public float Speed { get { return targetInfo.speed; } }

        /// <summary>
        /// Is this focal point currently being used by the target camera focus?
        /// </summary>
        /// <access>public bool</access>
        public bool IsActive { get; protected set; }

        /// <summary>
        /// Initialize targetCameraFocus reference to main camera if null. 
        /// Note: Must happen after CameraExtension calls Initialize (which sets MainCamera values)
        /// </summary>
        /// <access>public override void</access>
        /// <seealso cref="CameraExtension.Initialize" />
        public override void Initialize()
        {
            if (targetCameraFocus == null || useMainCamera) targetCameraFocus = MainCamera.Focus;
        }

        /// <summary>
        /// Add the focal point to the current set of active points when it is enabled
        /// </summary>
        /// <access>protected void</access>
        protected void OnEnable()
        {
            targetInfo = targetCameraFocus.AddTarget(
                targetInfo.transform == null ? transform : targetInfo.transform, 
                targetInfo.weight, 
                targetInfo.maxDistance,
                targetInfo.zoom, 
                targetInfo.pull, 
                targetInfo.speed
            );
            IsActive = true;
        }

        /// <summary>
        /// Remove the focal point from the current set of active points when it is disabled or destroyed
        /// </summary>
        /// <access>protected void</access>
        protected void OnDisable()
        {
            targetCameraFocus.RemoveTarget(transform);
            IsActive = false;
        }
    }
}