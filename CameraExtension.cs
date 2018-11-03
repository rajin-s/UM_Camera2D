using UnityEngine;

namespace UModules
{
    /// <summary>
    /// Extended behavior for a 2D Camera.
    /// Includes size and position manipulation, as well as view rectangle calculation.
    /// Most properties are only relevant for perspective cameras and won't affect orthographic projecting cameras.
    /// This setup assumes the camera is unrotated.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraExtension : ExtendedBehaviour
    {
        /// <summary>
        /// The distance scale factor for a pull value of -1
        /// </summary>
        private const float pullOutZoom = 4.0f;
        /// <summary>
        /// The distance scale factor for a pull value of 1
        /// </summary>
        private const float pullInZoom = 0.25f;

        /// <summary>
        /// Cached Camera component, if it has been gotten yet
        /// </summary>
        private Camera _camera;
        /// <summary>
        /// Property that will get the attached camera component on first reference, then return cached value
        /// </summary>
        public Camera Camera { get { return _camera ?? (_camera = GetComponent<Camera>()); } }

        /// <summary>
        /// Is this component attached to the main camera? (Only one main camera can exist!)
        /// </summary>
        [Header("Properties")]
        [Tooltip("Only one main camera can exist at a time!")]
        [Button("Initialize", "Update Main Camera", true)]
        [SerializeField]
        private bool isMainCamera = true;
        /// <summary>
        /// The height of the camera view rectangle in world units
        /// </summary>
        [SerializeField]
        private float _worldHeight = 5;
        /// <summary>
        /// The base distance of the camera from the XY plane in world units
        /// </summary>
        [SerializeField]
        private float _baseDistance = 10;
        /// <summary>
        /// Scale factor for camera view rectangle size (1 is no zoom)
        /// </summary>
        [Range(0.25f, 4f)]
        [SerializeField]
        private float _zoomScale = 1;
        /// <summary>
        /// Pull factor to change FOV and distance such that the view rectangle size remains the same
        /// </summary>
        [Range(-1, 1)]
        [SerializeField]
        private float _pull = 0;

        /// <summary>
        /// World space view rectangle on the XY plane (not directly editable, set based on height, distance, and zoom)
        /// </summary>
        [Readonly]
        [SerializeField]
        private Rect _rect;
        /// <summary>
        /// Publicly accessible world space view rectangle on the XY plane
        /// </summary>
        public Rect WorldRect { get { return _rect; } }

        /// <summary>
        /// Update the center of the view rectangle on the XY plane
        /// </summary>
        private void UpdateRect()
        {
            _rect.center = transform.position;
        }
        /// <summary>
        /// Update the camera's position and fov based on height, distance, zoom, and pull
        /// </summary>
        private void UpdateProperties()
        {
            float distance = _baseDistance * Mathf.Lerp(1, _pull < 0 ? pullOutZoom : pullInZoom, Mathf.Abs(_pull));
            Camera.SetWorldHeight(_worldHeight, distance);

            Vector3 position = transform.position;
            position.z = -distance / _zoomScale;

            transform.position = position;

            _rect.height = _worldHeight / _zoomScale;
            _rect.width = ((float)Camera.pixelWidth / Camera.pixelHeight) * _rect.height;
            UpdateRect();
        }

        /// <summary>
        /// Get the target height of the camera view rectangle in world units or set it and update position and fov accordingly
        /// </summary>
        public float WorldHeight { get { return _worldHeight; } set { _worldHeight = value; UpdateProperties(); } }
        /// <summary>
        /// Get the zoom value of the camera or set it and update position and fov accordingly
        /// </summary>
        public float Zoom { get { return _zoomScale; } set { _zoomScale = value; UpdateProperties(); } }
        /// <summary>
        /// Get the pull value of the camera or set it and update position and fov accordingly
        /// </summary>
        public float Pull { get { return _pull; } set { _pull = value; UpdateProperties(); } }

        /// <summary>
        /// Configure main camera references on Awake
        /// </summary>
        private void Awake()
        {
            if (isMainCamera)
            {
                MainCamera.Set(this);
            }
        }
        /// <summary>
        /// Set initial values on start
        /// </summary>
        public override void Initialize()
        {
            UpdateProperties();
        }
        /// <summary>
        /// Update the view rect on the XY plane at the beginning of each frame
        /// </summary>
        private void Update()
        {
            UpdateRect();
        }

        /// <summary>
        /// Apply changes to properties in editor
        /// </summary>
        private void OnValidate()
        {
            UpdateProperties();
        }
        private void OnDrawGizmos()
        {
            // Draw view rectangle on XY plane (faded)
            Gizmos.color = new Color(1, 1, 1, 0.25f);
            Gizmos.DrawWireCube(WorldRect.center, WorldRect.size);
        }
        private void OnDrawGizmosSelected()
        {
            // Draw view rectangle on XY plane (bright)
            Gizmos.color = new Color(0, 1, 1, 1);
            Gizmos.DrawWireCube(WorldRect.center, WorldRect.size);
        }
    }

    /// <summary>
    /// Singleton container for main camera setup
    /// Values set by CameraExtension
    /// </summary>
    /// <seealso cref="CameraExtension.isMainCamera" />
    public static class MainCamera
    {
        private const string mainCameraTag = "MainCamera";
        /// <summary>
        /// Camera component of main camera
        /// </summary>
        public static Camera Camera { get; internal set; }
        /// <summary>
        /// CameraExtension component of main camera
        /// </summary>
        public static CameraExtension Extension { get; internal set; }
        /// <summary>
        /// CameraFocus component of main camera
        /// </summary>
        public static CameraFocus Focus { get; internal set; }
        
        // /// <summary>
        // /// Set the main camera based on a given camera object
        // /// </summary>
        // /// <param name="camera">The new main camera</param>
        // public static void Set(Camera camera)
        // {
        //     MainCamera.Camera = camera;
        //     MainCamera.Extension = camera.GetComponent<CameraExtension>();
        //     MainCamera.Focus = camera.GetComponent<CameraFocus>();
        //     camera.gameObject.tag = mainCameraTag;
        // }

        /// <summary>
        /// Set the main camera based on a given camera extension script
        /// </summary>
        /// <param name="cameraExtension">The camera extension script attached to the new main camera</param>
        public static void Set(CameraExtension cameraExtension)
        {
            MainCamera.Camera = cameraExtension.Camera;
            MainCamera.Extension = cameraExtension;
            MainCamera.Focus = cameraExtension.GetComponent<CameraFocus>();
            cameraExtension.gameObject.tag = mainCameraTag;
        }
    }

    /// <summary>
    /// Extension method container for working with basic Camera components
    /// </summary>
    public static class CameraExtensionMethods
    {
        /// <summary>
        /// Set a camera's height in world units using the current distance from the XY plane
        /// </summary>
        /// <param name="camera">The target camera</param>
        /// <param name="height">Total height of view rectangle on XY plane in world units</param>
        public static void SetWorldHeight(this Camera camera, float height)
        {
            SetWorldHeight(camera, height, camera.transform.position.z);
        }
        /// <summary>
        /// Set a camera's height in world using a given distance from the XY plane
        /// </summary>
        /// <param name="camera">The target camera</param>
        /// <param name="height">Total height of view rectangle on XY plane in world units</param>
        /// <param name="distance">Distance at which the height of the view rectangle is equal to the given height</param>
        public static void SetWorldHeight(this Camera camera, float height, float distance)
        {
            if (camera.orthographic) // Handle orthographic cameras
            {
                camera.orthographicSize = height / 2;
            }
            else // Handle perspective cameras
            {
                height = Mathf.Abs(height);
                distance = Mathf.Abs(distance);

                float max = height / 2;
                float fov = Mathf.Atan2(max, distance) * Mathf.Rad2Deg * 2;

                camera.fieldOfView = fov;
            }
        }
    }
}
