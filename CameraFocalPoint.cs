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
    public class CameraFocalPoint : ExtendedBehaviour
    {
        /// <summary>
        /// CameraFocus component to affect (initializes to MainCamera if null)
        /// </summary>
        [Tooltip("Initialized to Main Camera if unset")]
        [SerializeField]
        private CameraFocus targetCameraFocus;

        /// <summary>
        /// Weight of focal point on creation
        /// </summary>
        [SerializeField]
        private float initialWeight = 10;
        /// <summary>
        /// Speed of movement toward focal point on creation
        /// </summary>
        [SerializeField]
        private float initialSpeed = 10;
        /// <summary>
        /// Max distance to affect camera on creation
        /// </summary>
        [SerializeField]
        private float initialMaxDistance = 10;

        /// <summary>
        /// Initialize targetCameraFocus reference to MainCamera if null. 
        /// Note: Must happen after CameraExtension Initialize (which sets MainCamera values)
        /// </summary>
        /// <seealso cref="CameraExtension.Initialize" />
        public override void Initialize()
        {
            if (targetCameraFocus == null) targetCameraFocus = MainCamera.Focus;
        }

        /// <summary>
        /// Add the focal point to the current set of active points when it is enabled
        /// </summary>
        private void OnEnable()
        {
            targetCameraFocus.AddTarget(transform, initialWeight, initialSpeed, initialMaxDistance);
        }

        /// <summary>
        /// Remove the focal point from the current set of active points when it is disabled or destroyed
        /// </summary>
        private void OnDisable()
        {
            targetCameraFocus.RemoveTarget(transform);
        }
    }
}