using UnityEngine;
using System.Collections.Generic;

namespace UModules
{
    /// <summary>
    /// Component to provide 2D collision-like behavior to a camera. 
    /// Requires a CameraExtension component on the same object and CameraWall components in the scene.
    /// </summary>
    /// <module>UM_Camera2D</module>
    [RequireComponent(typeof(CameraExtension))]
    [DisallowMultipleComponent]
    public class CameraArea : ExtendedBehaviour
    {
        /// <summary>
        /// The current set of active walls to consider in collision calculations
        /// </summary>
        /// <access>private List&lt;CameraWall&gt;</access>
        private List<CameraWall> activeWalls = new List<CameraWall>();
        /// <summary>
        /// Add a wall to be considered in collision calculations
        /// </summary>
        /// <access>public void</access>
        /// <param name="wall">The wall to be added</param>
        public void AddWall(CameraWall wall)
        {
            activeWalls.Add(wall);
        }
        /// <summary>
        /// Remove a wall from consideration in collision calculations.
        /// Recommended if the wall is disabled or in a different part of the level, etc.
        /// </summary>
        /// <access>public void</access>
        /// <param name="wall">The wall to be removed</param>
        public void RemoveWall(CameraWall wall)
        {
            activeWalls.Remove(wall);
        }

        /// <summary>
        /// The attached CameraExtension after it has been cached
        /// </summary>
        /// <access>private CameraExtension</access>
        private CameraExtension _extension;
        /// <summary>
        /// Get the attached CameraExtension component and cache the result if it hasn't already been
        /// </summary>
        /// <access>private CameraExtension</access>
        private CameraExtension Extension { get { return _extension ?? (_extension = GetComponent<CameraExtension>()); } }

        /// <summary>
        /// Get the required offset from a position to ensure the camera view area is not overlapping any active walls
        /// </summary>
        /// <param name="targetPoint">The center of the camera view area</param>
        /// <returns>The offset from targetPoint at which point no overlaps are occuring</returns>
        public Vector2 GetOffset(Vector2 targetPoint)
        {
            Rect cameraRect = Extension.WorldRect;
            Vector2 offset = Vector2.zero;

            // Iterate through active walls
            for (int i = 0; i < activeWalls.Count; i++)
            {
                CameraWall wall = activeWalls[i];

                // Skip walls with mode None
                if (wall.mode == CameraWall.WallMode.None) continue;

                // Take current offset into account
                cameraRect.center = targetPoint + offset;

                // Wide X overlap -> Y correction
                // Wide Y overlap -> X correction
                Rect wallRect = wall.WorldRect;
                Vector2 overlap;

                // Get overlap values
                overlap.x =  cameraRect.width - Mathf.Max(0, (cameraRect.xMax - wallRect.xMax)) - Mathf.Max(0, (wallRect.xMin - cameraRect.xMin));
                overlap.y = cameraRect.height - Mathf.Max(0, (cameraRect.yMax - wallRect.yMax)) - Mathf.Max(0, (wallRect.yMin - cameraRect.yMin));
                overlap.x = Mathf.Max(0, overlap.x);
                overlap.y = Mathf.Max(0, overlap.y);

                // Skip this wall if it isn't actually overlapping
                if (overlap.x == 0 || overlap.y == 0) continue;

                // Decide which axis to correct based on mode
                // Solid -> based on larger overlap
                // Horizontal / Vertical -> always correct that axis
                bool correctVertical   = wall.mode == CameraWall.WallMode.Vertical   || wall.mode == CameraWall.WallMode.Solid && overlap.x > overlap.y;
                bool correctHorizontal = wall.mode == CameraWall.WallMode.Horizontal || wall.mode == CameraWall.WallMode.Solid && !correctVertical;
                
                if (correctVertical) // Add to vertical offset
                {
                    float dy;
                    // Correct up or down depending on relative position
                    if (cameraRect.center.y > wallRect.center.y)
                        dy = wallRect.yMax - cameraRect.yMin;
                    else
                        dy = wallRect.yMin - cameraRect.yMax;

                    offset += dy * Vector2.up;
                }

                if (correctHorizontal)
                {
                    float dx;
                    // Correct right or left depending on relative position
                    if (cameraRect.center.x > wallRect.center.x)
                        dx = wallRect.xMax - cameraRect.xMin;
                    else
                        dx = wallRect.xMin - cameraRect.xMax;

                    offset += dx * Vector2.right;
                }
            }

            return offset;
        }
    }
}