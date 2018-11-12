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
        /// <summary>Should this focal point target the main camera?</summary>
        /// <access>protected bool</access>
        [SerializeField]
        protected bool useMainCamera = true;

        /// <summary>CameraFocus component to affect (initializes to main camera if null or useMainCamera is true)</summary>
        /// <access>protected CameraFocus</access>
        [DontShowIf("useMainCamera")]
        [SerializeField]
        protected CameraFocus targetCameraFocus;

        /// <summary>Influence weight for average calculations</summary>
        /// <access>public float</access>
        [Header("Properties")]
        public float weight = 300;

        /// <summary>Max influence distance for average calculations</summary>
        /// <access>public float</access>
        public float maxDistance = 15;

        /// <summary>Target zoom scale</summary>
        /// <access>public float</access>
        [Range(0.25f, 4f)]
        public float zoom = 1;

        /// <summary>Target pull value</summary>
        /// <access>public float</access>
        [Range(-1, 1)]
        public float pull = 0;

        /// <summary>Target speed scale</summary>
        /// <access>public float</access>
        public float speed = 1;

        /// <summary>Weight scale value (for animation, etc.)</summary>
        /// <access>public float</access>
        [Range(0, 1)]
        public float influenceScale = 1;

        /// <summary>Is this focal point currently being used by the target camera focus?</summary>
        /// <access>public bool</access>
        public bool IsActive { get; protected set; }

        /// <summary>Initialize targetCameraFocus reference to main camera if null. Note: Must happen after CameraExtension calls Initialize (which sets MainCamera values)
        /// </summary>
        /// <access>public override void</access>
        /// <seealso cref="CameraExtension.Initialize" />
        public override void Initialize()
        {
            if (targetCameraFocus == null || useMainCamera) targetCameraFocus = MainCamera.Focus;
        }

        /// <summary>Add the focal point to the current set of active points when it is enabled</summary>
        /// <access>protected void</access>
        protected void OnEnable()
        {
            targetCameraFocus.AddFocalPoint(this);
            IsActive = true;
        }

        /// <summary>Remove the focal point from the current set of active points when it is disabled or destroyed</summary>
        /// <access>protected void</access>
        protected void OnDisable()
        {
            targetCameraFocus.RemoveFocalPoint(this);
            IsActive = false;
        }
    }
}