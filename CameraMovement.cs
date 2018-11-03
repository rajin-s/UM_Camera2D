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
    /// Camera movement driver script that uses CameraFocus and CameraArea to control motion
    /// Requires CameraFocus (any subclass) and CameraArea components on the same object
    /// </summary>
    [RequireComponent(typeof(CameraFocus))]
    public class CameraMovement : ExtendedBehaviour
    {
        /// <summary>
        /// Cached CameraFocus component, if it has been gotten
        /// </summary>
        private CameraFocus _CameraFocus;
        /// <summary>
        /// Get the CameraFocus component if it has been cached, return cached value otherwise
        /// </summary>
        public CameraFocus CameraFocus { get { return _CameraFocus ?? (_CameraFocus = GetComponent<CameraFocus>()); } }

        // test
        public float speed = 5;

        // test
        private void LateUpdate()
        {
            var targetInfo = CameraFocus.GetMovementTarget();
            Vector2 movePosition = Vector2.Lerp(transform.position, targetInfo.position, speed * targetInfo.speed * Time.deltaTime);
            transform.position = new Vector3(movePosition.x, movePosition.y, transform.position.z);
        }
    }
}