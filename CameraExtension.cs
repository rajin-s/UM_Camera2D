using UnityEngine;

namespace UModules
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraExtension : ExtendedBehaviour
    {
        private const float pullOutZoom = 4.0f, pullInZoom = 0.25f;

        private Camera _camera;
        public Camera Camera { get { return _camera ?? (_camera = GetComponent<Camera>()); } }

        [Header("Properties")]
        public float x;
    }
}
