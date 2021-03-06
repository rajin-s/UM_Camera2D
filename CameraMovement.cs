/*
    UModules::CameraMovement

    by: Rajin Shankar
    part of: UM_Camera2D

    available to use according to UM_Camera2D/LICENSE
 */

using UnityEngine;

namespace UModules
{
    /// <summary>
    /// Camera movement driver script that uses CameraFocus and CameraArea to control motion.
    /// Uses CameraExtension, CameraShake, CameraArea, and CameraFocus components attached to the same object
    /// </summary>
    /// <module>UM_Camera2D</module>
    [RequireComponent(typeof(CameraExtension))]
    [DisallowMultipleComponent]
    public class CameraMovement : ExtendedBehaviour
    {
        /// <summary>
        /// The CameraExtension component to use.
        /// Always initialized to the component attached to the same object.
        /// </summary>
        /// <access>protected CameraExtension</access>
        protected CameraExtension extension;

        /// <summary>
        /// The CameraFocus component to use (can be null).
        /// Always initialized to the component attached to the same object.
        /// </summary>
        /// <access>protected CameraFocus</access>
        protected CameraFocus focus;

        /// <summary>
        /// The CameraArea component to use (can be null).
        /// Always initialized to the component attached to the same object.
        /// </summary>
        /// <access>protected CameraArea</access>
        protected CameraArea area;

        /// <summary>
        /// The CameraShake component to use (can be null).
        /// Always initialized to the component attached to the same object.
        /// </summary>
        /// <access>protected CameraShake</access>
        protected CameraShake shake;

        /// <summary>Base asymptotic speed for camera panning</summary>
        /// <access>protected float</access>
        [SerializeField]
        protected float panSpeed = 4;

        /// <summary>Base asymptotic speed for camera zooming</summary>
        /// <access>protected float</access>
        [SerializeField]
        protected float zoomSpeed = 1;

        /// <summary>Target position to move to, not counting camera shake to keep motion predictable</summary>
        /// <access>protected Vector2</access>
        protected Vector2 targetMovePosition;

        /// <summary>Get CameraFocus and CameraArea components</summary>
        /// <access>public override void</access>
        public override void Initialize()
        {
            extension = GetComponent<CameraExtension>();
            focus = GetComponent<CameraFocus>();
            area = GetComponent<CameraArea>();
            shake = GetComponent<CameraShake>();

            if (focus != null)
            {
                extension.Pan = focus.BaseFocalPoint.transform.position;
                targetMovePosition = extension.Pan;
            }
        }

        /// <summary>Update the camera's properties through CameraExtension</summary>
        /// <access>protected void</access>
        protected void LateUpdate()
        {
            Vector2 finalPosition = transform.position;
            float finalPanSpeed = panSpeed, finalZoomSpeed = zoomSpeed, finalZoom = float.NaN, finalPull = float.NaN;

            if (focus != null)
            {
                var targetInfo = focus.GetMovementTarget();
                finalPosition = targetInfo.position;
                finalZoom = targetInfo.zoom;
                finalPull = targetInfo.pull;
                finalPanSpeed *= targetInfo.speed;
                finalZoomSpeed *= targetInfo.speed;
            }
            if (area != null)
            {
                Vector2 areaOffset = area.GetOffset(finalPosition);
                finalPosition += areaOffset;
            }

            targetMovePosition = Vector2.Lerp(targetMovePosition, finalPosition, Time.deltaTime * finalPanSpeed);
            if (!(float.IsNaN(finalZoom) && float.IsNaN(finalPull)))
            {
                extension.Zoom = Mathf.Lerp(extension.Zoom, finalZoom, Time.deltaTime * finalZoomSpeed);
                extension.Pull = Mathf.Lerp(extension.Pull, finalPull, Time.deltaTime * finalZoomSpeed);
            }

            Vector2 movePosition = targetMovePosition;
            if (shake != null)
            {
                var shakeResult = shake.GetShake();
                movePosition += shakeResult.offset;
                extension.transform.rotation = Quaternion.Euler(Vector3.forward * shakeResult.rotation);
            }

            extension.Pan = movePosition;
        }
        // public Vector2 test;
        // public CameraShake.TraumaMode mode;
        // public void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.T))
        //     {
        //         shake.AddTrauma(test, mode);
        //     }
        // }
    }
}