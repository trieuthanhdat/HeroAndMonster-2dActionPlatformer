using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class SnailEnemyAIController : IAIController
{
    
    [SerializeField] Rigidbody2D projectile;         // the prefab of the projectile
    [SerializeField] Transform firePoint;             // the position where the projectile will be fired from
    [SerializeField] float forceMultiplier;           // the force that will be applied to the projectile
    [SerializeField] float shootCooldown = 2.0f;      // the time delay between shots
    [SerializeField] float attackRange = 6f;
    [SerializeField] float activeDistance = 5;
    
    private Transform playerTransform;       // the player's transform
    private EnemyStates currentState = EnemyStates.IDLE;
    private bool isFacingRight = true;
    private bool grounded = false;
    private Transform player;
    public float chaseTimeInterval = 2f;
    private float timeSinceLastChase = 0f;
    private float delayToIlde = 0;
    private Sensor_HeroKnight   groundSensor;
    private Rigidbody2D rb;
    private bool canAppear = false;
    private bool canDisappear = false;
    private bool firstAppeared = false;
    private Health health ;

    ///---ANIMATION EVENTS---///
    void FireProjectile()
    {
        // calculate the direction and force of the launch
        Vector2 launchDirection = playerTransform.position - firePoint.position;
        launchDirection.Normalize();
        float launchForce = Vector2.Distance(playerTransform.position, firePoint.position) * forceMultiplier;

        // instantiate the projectile at the fire point position
        Rigidbody2D projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation) as Rigidbody2D;

        // apply the force to the projectile
        projectileInstance.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
        health = GetComponent<Health>();
    }

    public void StartShooting()
    {
        FireProjectile();
    }
    public void SetFirstAppear()
    {
        firstAppeared = true;
    }
    ///----------------------///

    private void OnEnable()
    {
        currentState = EnemyStates.IDLE;
        animator.SetTrigger("Appear");
    }
    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        canAppear = true;
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        canDisappear = false;
    }

    public override void FixedUpdate()
    {

        if(firstAppeared == false) return;
        // Check if we should chase the player after waiting for a bit
        if (currentState == EnemyStates.CHASE && Time.time - timeSinceLastChase > chaseTimeInterval)
        {
            currentState = EnemyStates.IDLE;
        }
    }
    public void CheckGrounded()
    {
        //Check if character just landed on the ground
        if (!grounded && groundSensor.State())
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        //Check if character just started falling
        if (grounded && !groundSensor.State())
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }

    }
    public override void Update()
    {
        // Update the current state based on the enemy's behavior
        if(firstAppeared == false) return;
        CheckGrounded();
        switch (currentState)
        {
            case EnemyStates.IDLE:
                if (CanSeePlayer())
                {
                    currentState = EnemyStates.ATTACK;
                    timeSinceLastChase = Time.time;
                }
                else
                {
                    Patrol();
                }
                break;

            case EnemyStates.CHASE:
                break;

            case EnemyStates.ATTACK:
                if (CanSeePlayer())
                {
                    AttackPlayer();
                }
                else
                {
                    currentState = EnemyStates.IDLE;
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

    public override void Patrol()
    {
       

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
        if (Vector2.Distance(transform.position, player.position) < attackRange)
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
        GetComponent<SpriteRenderer>().flipX = !isFacingRight;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    private void HandleChase()
    {
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

    public override  bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }
        // Calculate the direction from the enemy to the player
        Vector3 direction = player.position - transform.position;
        // Cast a ray in the direction of the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, activeDistance, LayerMask.GetMask("Player"));
        // If the ray hit the player, return true
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }
    private void OnDrawGizmos()
    {
        // Draw a line showing the direction of the raycast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (player.position - transform.position).normalized * activeDistance);
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