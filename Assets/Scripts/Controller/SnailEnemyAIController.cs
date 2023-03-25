using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class SnailEnemyAIController : IAIController
{
    private EnemyStates currentState = EnemyStates.IDLE;
    private bool isFacingRight = true;
    private Transform player;
    public float chaseTimeInterval = 2f;
    private float timeSinceLastChase = 0f;
    private Rigidbody2D rb;

    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdate()
    {
        // Check if we should chase the player after waiting for a bit
        if (currentState == EnemyStates.CHASE && Time.time - timeSinceLastChase > chaseTimeInterval)
        {
            currentState = EnemyStates.IDLE;
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
        }
    }

    public override void Update()
    {
        // Update the current state based on the enemy's behavior
        switch (currentState)
        {
            case EnemyStates.IDLE:
                if (CanSeePlayer())
                {
                    currentState = EnemyStates.CHASE;
                    timeSinceLastChase = Time.time;
                    animator.SetBool("isRunning", true);
                }
                else
                {
                    Patrol();
                }
                break;

            case EnemyStates.CHASE:
                if (CanSeePlayer())
                {
                    ChasePlayer();
                    timeSinceLastChase = Time.time;
                }
                else
                {
                    currentState = EnemyStates.IDLE;
                    animator.SetBool("isRunning", false);
                }
                break;

            case EnemyStates.ATTACK:
                if (CanSeePlayer())
                {
                    AttackPlayer();
                }
                else
                {
                    currentState = EnemyStates.CHASE;
                    animator.SetBool("isAttacking", false);
                }
                break;

            default:
                Debug.LogError("Unknown enemy state: " + currentState);
                break;
        }

        // Flip the enemy sprite based on the direction it's facing
        if ((player.position.x < transform.position.x && isFacingRight) || (player.position.x > transform.position.x && !isFacingRight))
        {
            Flip();
        }
    }

    public override void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * 3f;
        animator.SetBool("isRunning", true);

        // Stop chasing after a certain amount of time
        if (Time.time - timeSinceLastChase > chaseTimeInterval)
        {
            currentState = EnemyStates.IDLE;
            animator.SetBool("isRunning", false);
            rb.velocity = Vector2.zero;
        }

        // Attack the player if they're close enough
        if (Vector2.Distance(transform.position, player.position) < 1.5f)
        {
            currentState = EnemyStates.ATTACK;
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", true);
            rb.velocity = Vector2.zero;
        }
    }


    public override void Flip()
    {
        // Flip the enemy sprite horizontally
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentState = EnemyStates.ATTACK;
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentState = EnemyStates.CHASE;
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRunning", true);
        }
    }

    public override void HandleStates()
    {
    // Handle the different states of the enemy
        switch (currentState)
        {
            case EnemyStates.IDLE:
                HandleIdle();
                break;
            case EnemyStates.RUN:
                break;
            case EnemyStates.ATTACK:
                HandleAttack();
                break;
            case EnemyStates.PATROL:
                Patrol();
                break;
            case EnemyStates.CHASE:
                HandleChase();
                break;
            default:
                break;
        }
    }

    private void HandleChase()
    {
        if (CanSeePlayer())
        {
            ChasePlayer();
        }
        else
        {
            currentState = EnemyStates.IDLE;
            animator.SetBool("isRunning", false);
        }
    }

    public override void HandleIdle()
    {
        if (CanSeePlayer())
        {
            currentState = EnemyStates.CHASE;
            animator.SetBool("isRunning", true);
        }
        else
        {
            Patrol();
        }
    }

    public override void HandleAttack()
    {
    // TODO: Implement enemy attack logic here
    }

    public  bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }
        // Calculate the direction from the enemy to the player
        Vector3 direction = player.position - transform.position;

        // Cast a ray in the direction of the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Player"));

        // If the ray hit the player, return true
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }


    public override void HandleHit()
    {
    // TODO: Implement enemy hit reaction here
    }

    public override void HandleDeath()
    {
    // TODO: Implement enemy death logic here
    }

    public override void HandleSpecial()
    {
    // TODO: Implement enemy special attack logic here
    }

    public override void HandleDash()
    {
    // TODO: Implement enemy dash logic here
    }

    public override void HandleWallSlide()
    {
    // TODO: Implement enemy wall slide logic here
    }

    public override void HandleJump()
    {
    // TODO: Implement enemy jump logic here
    }

    public override void HandleBlock()
    {
    // TODO: Implement enemy block logic here
    }
}