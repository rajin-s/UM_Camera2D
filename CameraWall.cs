using UnityEngine;

namespace UModules
{
    /// <summary>
    /// A wall that blocks camera motion through the CameraArea and CameraMovement components (always axis-aligned)
    /// </summary>
    /// <module>UM_Camera2D</module>
    public class CameraWall : ExtendedBehaviour
    {
        /// <summary>
        /// The color to use for drawing CameraWall gizmos in the editor
        /// </summary>
        ///<access>private static readonly Color</access>
        private static readonly Color gizmoColor = new Color(1, 0.2f, 0.2f);

        /// <summary>
        /// Should this wall block the main camera?
        /// </summary>
        ///<access>protected bool</access>
        [SerializeField]
        protected bool blockMainCamera = true;

        /// <summary>
        /// CameraArea component to affect (initializes to main camera if null or blockMainCamera is true)
        /// </summary>
        ///<access>protected CameraArea</access>
        [DontShowIf("blockMainCamera")]
        [SerializeField]
        protected CameraArea targetCameraArea;

        /// <summary>
        /// How should a wall affect camera motion?
        /// Horizontal/Vertical affect one axis, Solid affects both axes, None affects neither.
        /// </summary>
        public enum WallMode { Solid, Horizontal, Vertical, None }
        /// <summary>
        /// How should this wall affect camera motion?
        /// </summary>
        ///<access>public WallMode</access>
        public WallMode mode = WallMode.Solid;

        /// <summary>
        /// Axis aligned bounding box of wall
        /// </summary>
        ///<access>protected Rect</access>
        [SerializeField]
        protected Rect rect = new Rect(0, 0, 1, 1);

        /// <summary>
        /// Calculate the world-space rectangle of the wall. 
        /// Takes position and scale into account.
        /// </summary>
        /// <access>protected virtual Rect</access>
        /// <returns>The world-space rectangle of the wall</returns>
        protected virtual Rect CalculateWorldRect()
        {
            Rect r = rect;
            r.size = r.size.Multiply(transform.lossyScale);
            r.center = (Vector2)transform.position + rect.position.Multiply(transform.lossyScale);
            return r;
        }

        /// <summary>
        /// Property to access Rect given by CalculateWorldRect()
        /// </summary>
        ///<access>public Rect</access>
        public Rect WorldRect { get { return CalculateWorldRect(); } }

        /// <summary>
        /// Initialize targetCameraArea reference to main camera if null or blockMainCamera is true.
        /// Note: Must happen after CameraExtension calls Initialize (which sets MainCamera values)
        /// </summary>
        /// <access>public override void</access>
        /// <seealso cref="CameraExtension.Initialize" />
        public override void Initialize()
        {
            if (targetCameraArea == null || blockMainCamera) targetCameraArea = MainCamera.Area;
        }

        /// <summary>
        /// Add the wall to the current set of active walls when it is enabled
        /// </summary>
        /// <access>protected void</access>
        protected void OnEnable()
        {
            targetCameraArea.AddWall(this);
        }
        /// <summary>
        /// Remove the wall from the current set of active walls when it is disabled
        /// </summary>
        /// <access>protected void</access>
        protected void OnDisable()
        {
            targetCameraArea.RemoveWall(this);
        }

        /// <summary>
        /// Draw an editor gizmo for a wall with a given base color
        /// </summary>
        /// <access>protected void</access>
        /// <param name="color">Base color to draw wall</param>
        protected void DrawWallGizmo(Color color)
        {
            const int axisDivisions = 5;
            const float maxInteriorOpacity = 0.5f;

            Rect worldRect = WorldRect;
            Color drawColor = color;

            Gizmos.color = drawColor;
            Gizmos.DrawWireCube(worldRect.center, worldRect.size);

            if (!enabled) return;

            if (mode == WallMode.Solid)
            {
                drawColor.a = maxInteriorOpacity;
                Gizmos.color = drawColor;
                Gizmos.DrawCube(worldRect.center, worldRect.size);
            }
            else if (mode == WallMode.Horizontal)
            {
                for (int i = 0; i < axisDivisions; i++)
                {
                    float opacityScale = i % 2 == 0 ? 1.2f : 0.6f;
                    drawColor.a = maxInteriorOpacity * opacityScale;
                    Gizmos.color = drawColor;
                    float size = worldRect.size.x / axisDivisions;
                    Gizmos.DrawCube(
                        worldRect.min + (i * size + size / 2) * Vector2.right + worldRect.size.y / 2 * Vector2.up,
                        new Vector2(size, worldRect.size.y)
                    );
                }
            }
            else if (mode == WallMode.Vertical)
            {
                for (int i = 0; i < axisDivisions; i++)
                {
                    float opacityScale = i % 2 == 0 ? maxInteriorOpacity * 2f : maxInteriorOpacity;
                    drawColor.a = maxInteriorOpacity * opacityScale;
                    Gizmos.color = drawColor;
                    float size = worldRect.size.y / axisDivisions;
                    Gizmos.DrawCube(
                        worldRect.min + (i * size + size / 2) * Vector2.up + worldRect.size.x / 2 * Vector2.right,
                        new Vector2(worldRect.size.x, size)
                    );
                }
            }
        }

        /// <summary>
        /// Call DrawWallGizmo with CameraWall.gizmoColor
        /// </summary>
        /// <access>private void</access>
        private void OnDrawGizmos()
        {
            DrawWallGizmo(gizmoColor);
        }
    }
}