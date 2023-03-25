using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float xOffset = 2f;
    public float yOffset = 1f;
    public float xMargin = 1f;
    public float yMargin = 1f;
    public float maxSpeed = 10f;
    public float jumpThreshold = 1f;
    public float jumpSmoothTime = 0.3f;
    public float jumpHeight = 3f;
    public LayerMask collisionMask;

    private bool isJumping;
    private Vector3 jumpVelocity;
    private Vector3 currentVelocity;
    private bool leftEdgeDetected = false;
    private bool rightEdgeDetected = false;
    private bool getLeftOnce = false;
    private bool getRightOnce = false;
    private float leftDistance;
    private float rightDistance;
    private bool canFollow = true;
    private Vector3 currentLeftValidatePos ;
    private Vector3 currentRightValidatePos ;

    private void Start()
    {
        CalculateInitialDistanceFormEdges();
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + new Vector3(xOffset, yOffset, -10f);
        float distance = Mathf.Abs(transform.position.x - targetPosition.x);
        bool isCloseToEdge = distance > Screen.width / 2f - xMargin || target.position.y > transform.position.y + yMargin;
        float speed = isCloseToEdge ? Mathf.Clamp(distance - Screen.width / 2f + xMargin, 0f, maxSpeed) : smoothSpeed;
        Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, speed);

        if (target.position.x < transform.position.x - xMargin || target.position.x > transform.position.x + xMargin)
        {
            newPosition.x = transform.position.x;
        }

        if (target.position.y > transform.position.y + jumpThreshold)
        {
            isJumping = true;
        }

        if (isJumping)
        {
            float targetY = Mathf.Max(target.position.y + yOffset, transform.position.y + jumpHeight);
            newPosition.y = Mathf.SmoothDamp(transform.position.y, targetY, ref jumpVelocity.y, jumpSmoothTime);
            if (transform.position.y > targetY - 0.1f)
            {
                isJumping = false;
            }
        }
        else
        {
            newPosition.y = Mathf.SmoothDamp(transform.position.y, target.position.y + yOffset, ref currentVelocity.y, smoothSpeed);
        }

        transform.position = CheckPosition(ref newPosition);
    }

/// <summary>
/// Update is called every frame, if the MonoBehaviour is enabled.
/// </summary>
    private void Update()
    {
        RayCastToCameraEdge();
    }

   private Vector3 CheckPosition(ref Vector3 newPosition)
    {
        // Get the left and right edges of the screen
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
        
        // Check if the left edge is detected
        if (leftEdgeDetected)
        {
            // Check the facing direction of the target
            if (target.GetComponent<HeroKnight>().FaceDirection == 1)
            {
                if (target.transform.position.x >= currentLeftValidatePos.x)
                {
                    // Move towards the left edge
                    getLeftOnce = false;
                    newPosition.x = Mathf.SmoothDamp(newPosition.x, currentLeftValidatePos.x, ref jumpVelocity.x, smoothSpeed);
                }
                else
                {
                    // Stay at the same position
                    newPosition = new Vector3(transform.position.x, newPosition.y, newPosition.z);
                }
            }
            else
            {
                // Move towards the right edge
                newPosition = new Vector3(transform.position.x, newPosition.y, newPosition.z);
            }
        }
        
        // Check if the right edge is detected
        if (rightEdgeDetected)
        {
            // Check the facing direction of the target
            if (target.GetComponent<HeroKnight>().FaceDirection == -1)
            {
                if (target.transform.position.x <= currentRightValidatePos.x)
                {
                    // Move towards the right edge
                    getRightOnce = false;
                    newPosition.x = Mathf.SmoothDamp(newPosition.x, currentRightValidatePos.x, ref jumpVelocity.x, smoothSpeed);
                }
                else
                {
                    // Stay at the same position
                    newPosition = new Vector3(transform.position.x, newPosition.y, newPosition.z);
                }
            }
            else
            {
                // Move towards the left edge
                newPosition = new Vector3(transform.position.x, newPosition.y, newPosition.z);
            }
        }

        // Return the new position
        return newPosition;
    }           


    private void RayCastToCameraEdge()
    {
        // Calculate the left and right edges of the camera view in world coordinates
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));

        // Cast a ray to the left edge of the camera view
        RaycastHit2D leftHit = Physics2D.Raycast(target.transform.position, leftEdge - target.transform.position, leftDistance, collisionMask);
        if (leftHit.collider != null)
        {
            if(getLeftOnce == false)
                currentLeftValidatePos = target.transform.position;
            getLeftOnce = true;
            leftEdgeDetected = true;
            Debug.DrawLine(transform.position, leftHit.point, Color.green);
        }
        else
        {
            rightEdgeDetected = false;
            currentLeftValidatePos = Vector3.zero;
            Debug.DrawLine(transform.position, leftEdge, Color.red);
        }

        // Cast a ray to the right edge of the camera view
        RaycastHit2D rightHit = Physics2D.Raycast(target.transform.position, rightEdge - target.transform.position, rightDistance, collisionMask);
        if (rightHit.collider != null)
        {
            if(getRightOnce == false)
                currentRightValidatePos = target.transform.position;
            getRightOnce = true;
            rightEdgeDetected = true;
            Debug.DrawLine(transform.position, rightHit.point, Color.green);
        }
        else
        {
            currentRightValidatePos = Vector3.zero;
            rightEdgeDetected = false;
            Debug.DrawLine(transform.position, rightEdge, Color.red);
        }
    }

    private void CalculateInitialDistanceFormEdges()
    {
        // Calculate the left and right edges of the camera view in world coordinates
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));

        // Calculate the distance from the player to the left and right edges of the camera view
        leftDistance = Vector3.Distance(target.transform.position, leftEdge);
        rightDistance = Vector3.Distance(target.transform.position, rightEdge);
    }
}

       
