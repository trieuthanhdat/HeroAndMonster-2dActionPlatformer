using System;
using RPG.Resources;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float stopTime = 2f;
    [SerializeField] float lookAroundTime = 3f;
    [SerializeField] float fieldOfViewAngle = 90f;
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackRate = 1f;
    [SerializeField] float attackDamage = 7f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform attackPoint;

    private int currentPatrolPointIndex = 0;
    private bool chasingPlayer = false;
    private bool attacking = false;
    private float timeSinceLastAttack = 0f;
    private float stopTimeRemaining = 0f;
    private float lookAroundTimeRemaining = 0f;
    private Transform playerTransform;
    private Vector3 lastKnownPlayerPosition;
    private SpriteRenderer enemySprite;
    private Vector2 targetPosition;
    private Health health ;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastKnownPlayerPosition = playerTransform.position;
        enemySprite = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if(health.IsDead()) return;
        if (chasingPlayer)
        {
            ChasePlayer();
        }
        else if (attacking)
        {
            AttackPlayer();
        }
        else if (stopTimeRemaining > 0f)
        {
            StopAndLookAround();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        // Move towards the current patrol point
        targetPosition = patrolPoints[currentPatrolPointIndex].position;
        Vector2 direction = (patrolPoints[currentPatrolPointIndex].position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        // transform.LookAt(patrolPoints[currentPatrolPointIndex]);
        Flip();

        // Check if the AI has reached the current patrol point
        if (Vector2.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) < 0.1f)
        {
            // Wait for a while before moving to the next patrol point
            stopTimeRemaining = stopTime;
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
        }

        // Check if the AI can detect the player in its field of view
        if (CanSeePlayer())
        {
            chasingPlayer = true;
            lastKnownPlayerPosition = playerTransform.position;
            targetPosition = lastKnownPlayerPosition;
        }
    }

    private void Flip()
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        if (direction.x < 0) 
            enemySprite.flipX = true;
        else
            enemySprite.flipX = false;
    }

    private void StopAndLookAround()
    {
        // Stop moving and look around for a while
        // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), Time.deltaTime);
        stopTimeRemaining -= Time.deltaTime;

        // Check if the AI can detect the player in its field of view
        if (CanSeePlayer())
        {
            chasingPlayer = true;
            lastKnownPlayerPosition = playerTransform.position;
        }

        // Check if the AI has finished looking around
        if (stopTimeRemaining <= 0f)
        {
            lookAroundTimeRemaining = lookAroundTime;
        }
    }

    private void ChasePlayer()
    {
        // Move towards the last known position of the player
        Vector2 direction = (lastKnownPlayerPosition - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        // transform.LookAt(lastKnownPlayerPosition);

        // Check if the AI has reached the player or lost sight of the player
        if (Vector2.Distance(transform.position, lastKnownPlayerPosition) < attackRange)
        {
            // Within attacking range
            attacking = true;
            chasingPlayer = false;
        }
        else if (Vector2.Distance(transform.position, lastKnownPlayerPosition) > chaseRange)
        {
            // Lost sight of player, stop chasing
            chasingPlayer = false;
        }
        else if (!CanSeePlayer())
        {
            // Player is not visible, but still within chase range, update last known position
            lastKnownPlayerPosition = playerTransform.position;
        }
    }

    private void AttackPlayer()
    {
        // Keep looking at the player while attacking
        transform.LookAt(lastKnownPlayerPosition);

        // Attack if it's time to attack
        if (timeSinceLastAttack >= attackRate)
        {
            // Check if player is still within attack range
            if (Vector2.Distance(transform.position, lastKnownPlayerPosition) < attackRange)
            {
                // Hit player with attack
                playerTransform.GetComponent<Health>().TakeDamage(playerTransform.gameObject, attackDamage);


                // Reset attack timer
                timeSinceLastAttack = 0f;
            }
            else
            {
                // Player moved out of attack range
                attacking = false;
                chasingPlayer = true;
            }
        }
        else
        {
            // Wait before attacking again
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    private bool CanSeePlayer()
    {
        // Check if player is within field of view angle and range
        Collider[] playersInFOV = Physics.OverlapSphere(transform.position, chaseRange, playerLayer);
        foreach (Collider playerCollider in playersInFOV)
        {
            Vector3 directionToPlayer = (playerCollider.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= fieldOfViewAngle * 0.5f)
            {
                // Player is within field of view angle, check for line of sight
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseRange))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        // Player is visible
                        return true;
                    }
                }
            }
        }
        // Player is not visible
        return false;
    }
}