using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class SnailEnemyAIController : IAIController
{
    
    [SerializeField] GameObject projectilePrefab;         // the prefab of the projectile
    [SerializeField] Transform firePoint;             // the position where the projectile will be fired from
    [SerializeField] private float launchAngle = 45f;
    [SerializeField] private float offsetY = 0.6f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] float forceMultiplier;           // the force that will be applied to the projectile
    [SerializeField] float shootCooldown = 2.0f;      // the time delay between shots
    [SerializeField] float attackRange = 6f;
    [SerializeField] float activeDistance = 5;
    [SerializeField] float attackCooldownTime = 3f;
    [SerializeField] float limitIdleTimeRange = 10f;
    
    private Transform playerTransform;       // the player's transform
    private EnemyStates currentState = EnemyStates.IDLE;
    private bool isFacingRight = true;
    private bool grounded = false;
    public float chaseTimeInterval = 2f;
    private float timeSinceLastChase = 0f;
    private float delayToIlde = 0;
    private float attackCooldownTimeRemaining = 0;
    private float idleTimeRemaining = 0;
    private Sensor_HeroKnight   groundSensor;
    private Rigidbody2D rb;
    private bool firstAppeared = false;
    private Health health ;
    private Health playerHealth;
    ///---ANIMATION EVENTS---///
   public void FireProjectile()
    {
        // Instantiate the projectile prefab at the fire point
        GameObject projectileInstance = Instantiate(projectilePrefab.gameObject, firePoint.position, Quaternion.identity);

        // Calculate the direction vector towards the player
        Vector2 truePosPlayer = new Vector2( playerHealth.transform.position.x, playerHealth.transform.position.y +offsetY);
        Vector2 direction = (truePosPlayer - (Vector2)firePoint.position).normalized;
        // Set the velocity of the projectile
        projectileInstance.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    }
    public void StartShooting()
    {
        FireProjectile();
    }
    public void SetFirstAppear()
    {
        firstAppeared = true;
    }
    public void StartDisappear()
    {
        currentState = EnemyStates.IDLE;
        gameObject.SetActive(false);

    }
    public void StartDie()
    {
        currentState = EnemyStates.DEATH;
        Destroy(gameObject);
    }

    public override  void OnEnable()
    {
        HandleAppear();
        currentState = EnemyStates.IDLE;
    }
    public bool IsDead()
    {
         return health.IsDead();
    }
    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        idleTimeRemaining = UnityEngine.Random.Range(5, limitIdleTimeRange);
        health = GetComponent<Health>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerHealth.OnDie.AddListener(HandleDisappear);
    }
    public override void FixedUpdate()
    {
        if(firstAppeared == false && !health.IsDead()) return;
        // Check if we should attack the player after waiting for a bit
        if (currentState == EnemyStates.ATTACK && Time.time - timeSinceLastChase > chaseTimeInterval)
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
        }

        //Check if character just started falling
        if (grounded && !groundSensor.State())
        {
            grounded = false;
        }

    }
    public override void Update()
    {
        // Update the current state based on the enemy's behavior
        if(firstAppeared == false) return;
       
        CheckGrounded();
        Timer();
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
            case EnemyStates.DISAPPEAR:
                HandleDisappear();
                break;
            case EnemyStates.APPEAR:
                break;
            case EnemyStates.DEATH:
                gameObject.SetActive(false);
                break;
            default:
                Debug.LogError("Unknown enemy state: " + currentState);
                break;
        }

        // Flip the enemy sprite based on the direction it's facing
        if(!gameObject.activeInHierarchy) return;
        if ((playerTransform.position.x < transform.position.x && isFacingRight) || (playerTransform.position.x > transform.position.x && !isFacingRight))
        {
            Flip();
        }
    }

    private void Timer()
    {
        attackCooldownTimeRemaining -= Time.deltaTime;
        idleTimeRemaining -= Time.deltaTime;
    }

    public override void Patrol()
    {
       if(idleTimeRemaining <=0)
       {
            currentState = EnemyStates.DISAPPEAR;
       }
    }
   
    public override void ChasePlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
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
        if (Vector2.Distance(transform.position, playerTransform.position) < attackRange)
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
        if(CanSeePlayer())
            isFacingRight = playerTransform.position.x > transform.position.x;
        GetComponent<SpriteRenderer>().flipX = isFacingRight;
    }

    public void SetPlayer(Transform player)
    {
        this.playerTransform = player;
    }

    public override void HandleIdle()
    {
        if (CanSeePlayer())
        {
            currentState = EnemyStates.ATTACK;
            HandleAttack();
        }
        else
        {
            Patrol();
        }
    }
    public override void AttackPlayer()
    {
        HandleAttack();
    }
    public override void HandleAttack()
    {
        if(attackCooldownTimeRemaining <= 0 )
        {
            animator.SetTrigger("Attack");
            attackCooldownTimeRemaining = attackCooldownTime;
        }
    }

    public override  bool CanSeePlayer()
    {
        if (playerTransform == null && playerHealth.IsDead())
        {
            return false;
        }
        // Calculate the direction from the enemy to the player
        Vector3 direction = playerTransform.position - transform.position;
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
        if(playerTransform == null )return;
        // Draw a line showing the direction of the raycast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (playerTransform.position - transform.position).normalized * activeDistance);
    }

    public override void HandleDisappear()
    {
        animator.SetTrigger("Disappear");
    }
    public override void HandleAppear()
    {
        animator.SetTrigger("Appear");
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