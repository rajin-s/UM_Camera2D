using UnityEngine;

namespace UModules
{
    public class CameraWall : ExtendedBehaviour
    {
        private static readonly Color gizmoColor = new Color(1, 0.2f, 0.2f);

        [SerializeField]
        private bool useColliderValues = false;

        private BoxCollider2D _boxCollider;
        private BoxCollider2D BoxCollider { get { return _boxCollider ?? (_boxCollider = GetComponent<BoxCollider2D>()); } }

        [SerializeField]
        [DontShowIf("useColliderValues")]
        private Rect rect = new Rect(0, 0, 1, 1);
        public Rect WorldRect
        {
            get
            {
                if (useColliderValues)
                {
                    Rect r = new Rect() {
                        center = BoxCollider.offset + (Vector2)transform.position,
                        size = BoxCollider.size
                    };
                    return r;
                }
                else
                {
                    Rect r = rect;
                    r.center = (Vector2)transform.position + rect.center;
                    return r;
                }
            }
        }

        [SerializeField]
        [OnlyShowIf("useColliderValues")]
        private float colliderPadding;

        [SerializeField]
        [OnlyShowIf("useColliderValues")]
        private Vector2 colliderOffset;

        void OnDrawGizmos()
        {
            if (useColliderValues && BoxCollider == null) return;

            Rect worldRect = WorldRect;
            Color drawColor = gizmoColor;
            
            Gizmos.color = drawColor;
            Gizmos.DrawWireCube(worldRect.min, worldRect.size);

            drawColor.a = 0.25f;
            Gizmos.color = drawColor;
            Gizmos.DrawCube(worldRect.min, worldRect.size);
        }
    }
}