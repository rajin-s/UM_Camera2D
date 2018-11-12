using UnityEngine;

namespace UModules
{
    /// <summary>
    /// A wall that blocks camera motion through the CameraArea and CameraMovement components (always axis-aligned).
    /// Size and position are driven by a BoxCollider2D component.
    /// </summary>
    /// <module>UM_Camera2D</module>
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraWallFromBox : CameraWall
    {
        /// <summary>
        /// The color to use for drawing CameraWallFromBox gizmos in the editor
        /// </summary>
        ///<access>private static readonly Color</access>
        private static readonly Color gizmoColor = new Color(1, 0.7f, 0.2f);

        /// <summary>
        /// Cached BoxCollider2D component, if it has been gotten yet
        /// </summary>
        ///<access>private BoxCollider2D</access>
        private BoxCollider2D _box;
        /// <summary>
        /// Property that will get and return the attached BoxCollider2D component on first reference, then return cached value
        /// </summary>
        ///<access>private BoxCollider2D</access>
        private BoxCollider2D Box { get { return _box ? _box : (_box = GetComponent<BoxCollider2D>()); } }

        /// <summary>
        /// Calculate the world-space rectangle of the wall. 
        /// Takes box collider values, position, and scale into account.
        /// </summary>
        /// <access>protected override Rect</access>
        /// <returns>The world-space rectangle of the wall</returns>
        protected override Rect CalculateWorldRect()
        {
            Rect r = new Rect()
            {
                size = Box.size.Multiply(rect.size).Multiply(transform.lossyScale),
                center = (Vector2)transform.position + (Box.offset + Box.size.Multiply(rect.position)).Multiply(transform.lossyScale)
            };
            return r;
        }

        /// <summary>
        /// Call DrawWallGizmo with CameraWallFromBox.gizmoColor
        /// </summary>
        /// <access>private void</access>
        private void OnDrawGizmos()
        {
            DrawWallGizmo(gizmoColor);
        }
    }
}