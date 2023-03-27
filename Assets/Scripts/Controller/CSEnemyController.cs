using UnityEngine;
using System.Collections;
using System;
using RPG.Resources;
using System.Linq;
using System.Collections.Generic;


public class CSEnemyController : IAIController
{
   [SerializeField] Transform[] patrolPoints;
    // [SerializeField] Transform groundFowardCheck;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float stopTime = 2f;
    [SerializeField] float lookAroundTime = 3f;
    [SerializeField] float fieldOfViewAngle = 90f;
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackRate = 5f;
    [SerializeField] float attackDamage = 7f;
    [SerializeField] LayerMask playerLayer;
    // [SerializeField] LayerMask whatIsGround;
    // [SerializeField] private float groundCheckDistance;
    [SerializeField] Sensor_AttackPoint sensorAttackPoints;

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
    private Health health;
    private Sensor_HeroKnight   groundSensor;
    private bool grounded = false;
    private float delayToIlde = 0;
    private bool hasJustMoveAway = false;
    private bool doOnce = false;
    private bool groundForward = false;
    private int faceDirection = 1;

    private Health playerHealth;

    public override void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        HandleIdle();
        rb2D = GetComponent<Rigidbody2D>();
        lastKnownPlayerPosition = playerTransform.position;
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        enemySprite = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        playerHealth = playerTransform.GetComponent<Health>();
    }
 
    public override void Update()
    {
        CheckGrounded();
        
    }
    public override void FixedUpdate()
    {
        if (health.IsDead()) return;

        // if(playerHealth.IsDead()) 
        // {
        //     attacking = false;
        //     chasingPlayer = false;
        // }

        if (chasingPlayer )
        {
            // if(hasJustMoveAway)
            //     Invoke("ChasePlayer", 0.8f);
            // else
                ChasePlayer();
        }
        else if (attacking  )
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
        HandleStates();
        Flip();
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

   

    public override void Patrol()
    {
        // Move towards the current patrol point
        targetPosition = patrolPoints[currentPatrolPointIndex].position;
        Vector2 direction = (patrolPoints[currentPatrolPointIndex].position - transform.position).normalized;
        rb2D.velocity = direction * moveSpeed;


        // Check if the AI has reached the current patrol point
        if (Vector2.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) < 0.1f)
        {
            // Wait for a while before moving to the next patrol point
            stopTimeRemaining = stopTime;
            rb2D.velocity = Vector2.zero;
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

    public override void Flip()
    {
        if(CanSeePlayer())
            targetPosition = playerTransform.position;

        transform.localScale =new Vector2( targetPosition.x > transform.position.x ? -1 : 1, transform.localScale.y);
        sensorAttackPoints.transform.localScale = new Vector2(targetPosition.x > transform.position.x? -1 : 1, sensorAttackPoints.transform.localScale.y);
    }

    public override void StopAndLookAround()
    {
        // Stop moving and look around for a while
        stopTimeRemaining -= Time.fixedDeltaTime;

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
        if (lookAroundTimeRemaining <= 0f)
        {
            chasingPlayer = false;
        }
        else
        {
            lookAroundTimeRemaining -= Time.deltaTime;
        }
    }     
    public override void ChasePlayer()
    {
        hasJustMoveAway = false;
        // Move towards the player's last known position
        Vector2 direction = (lastKnownPlayerPosition - transform.position).normalized;
        rb2D.velocity = direction * moveSpeed;
        // Check if the AI can see the player
        if (CanSeePlayer())
        {
            // Update the last known player position
            lastKnownPlayerPosition = playerTransform.position;

            // Check if the AI is close enough to attack the player
            if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                attacking = true;
                chasingPlayer = false;
                rb2D.velocity = Vector2.zero;
            }
        }
        else
        {

            // Stop chasing the player if they are no longer in the AI's field of view
            chasingPlayer = false;
        }
    }

    public override bool CanSeePlayer()
    {
        // Check if the player is within the AI's field of view
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        float angle = Vector2.Angle(directionToPlayer, transform.right);
        if (angle < fieldOfViewAngle / 2f)
        {
            // Check if there are any obstacles between the AI and the player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, chaseRange, playerLayer);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public override void AttackPlayer()
    {
       
        // Check if the AI can still see the player
        if (CanSeePlayer())
        {
            // Update the last known player position
            lastKnownPlayerPosition = playerTransform.position;

            // Check if the AI is close enough to continue attacking the player
            if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                // Check if enough time has passed since the last attack
                if (timeSinceLastAttack >= attackRate)
                {
                    // Attack the player and reset the attack timer
                    HandleAttack();
                    
                    timeSinceLastAttack = 0f;
                }
                else
                {
                    // Increment the attack timer
                    timeSinceLastAttack += Time.fixedDeltaTime;
                }
            }
            else
            {
                // Stop attacking if the player is too far away
                attacking = false;
                chasingPlayer = true;
                if(doOnce == false)
                {
                    hasJustMoveAway = true;
                    doOnce = true;
                }
            }
        }
        else
        {
            // Stop attacking if the player is no longer in the AI's field of view
            attacking = false;
            chasingPlayer = false;
        }
    }

    public override void LookAround()
    {
        // Look around for a while
        lookAroundTimeRemaining -= Time.fixedDeltaTime;
        
        // Check if the AI can detect the player in its field of view
        if (CanSeePlayer())
        {
            chasingPlayer = true;
            lastKnownPlayerPosition = playerTransform.position;
        }

        // Check if the AI has finished looking around
        if (lookAroundTimeRemaining <= 0f)
        {
            chasingPlayer = false;
            attacking = false;
        }
    }

    ///---ANIMETION HANDLER---//
    public override void HandleHurt()
    {
        animator.SetTrigger("Hurt");
    }

    public override void HandleDeath()
    {
        animator.SetTrigger("Death");
        this.enabled = false;
    }

    public override void HandleAttack()
    {
        animator.SetTrigger("Attack1");
    }
    
    public override void HandleStates()
    {

        if (Mathf.Abs(rb2D.velocity.magnitude) > 0.1f)
        {
            // Reset timer
            delayToIlde = 0.05f;
            animator.SetInteger("AnimState", 1);
        }
        //Idle
        else
        {
            // Prevents flickering transitions to idle
            delayToIlde -= Time.deltaTime;
                if(delayToIlde < 0)
                    animator.SetInteger("AnimState", 0);
        }   
    }
    private void OnDrawGizmos()
    {
        if(playerTransform == null) return;
    // Draw the field of view arc
        float halfFOV = fieldOfViewAngle / 2f;
        Vector3 rightDir = Quaternion.AngleAxis(halfFOV, Vector3.forward) * transform.right;
        Vector3 leftDir = Quaternion.AngleAxis(-halfFOV, Vector3.forward) * transform.right;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, rightDir * chaseRange);
        Gizmos.DrawRay(transform.position, leftDir * chaseRange);
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Draw the line to the player
        if (CanSeePlayer())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }

    ///----ANIMATION EVENTS HANDLER----///
    public override void HandleHit()
    {
        if(playerHealth && !playerHealth.IsDead())
            playerHealth.TakeDamage(attackDamage,playerTransform.gameObject, playerTransform.GetComponent<HeroKnight>().HeroAnimator);
    }

}