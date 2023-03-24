using UnityEngine;
using System.Collections;

namespace  PC2D
{
   using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float boundaryOffset = 1f;
    public string boundEdgeTag = "BoundEdge";

    private bool isMoving = true;

    private void LateUpdate()
    {
        if (!isMoving) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Check if the camera's edges reach the BoundEdge object
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == boundEdgeTag)
            {
                Bounds cameraBounds = GetCameraBounds();
                Bounds edgeBounds = collider.bounds;

                // Check if the camera's edges reached the BoundEdge object's bounds
                if (cameraBounds.min.x <= edgeBounds.max.x && cameraBounds.max.x >= edgeBounds.min.x)
                {
                    isMoving = false;
                    break;
                }
            }
        }

        transform.position = smoothedPosition;
    }

    private Bounds GetCameraBounds()
    {
        float height = 2f * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;
        return new Bounds(transform.position, new Vector3(width, height, 0));
    }
}

}
