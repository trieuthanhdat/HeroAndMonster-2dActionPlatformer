using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : CharacterCombatController
{
    public float patrolDistance = 5f;
    public float attackRange = 1f;
    public int attackDamage = 10;
    public float moveSpeed = 3f;
    private Transform player;
    private float distanceToPlayer;
    private bool isChasingPlayer = false;
    private bool isFacingRight = true;
    private bool canAttack = true;

    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        // Check if player is in range
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= patrolDistance)
        {
            // Chase player
            ChasePlayer();
        }
        else
        {
            // Patrol
            Patrol();
        }
    }

    void Patrol()
    {
        // Move left and right within patrol distance
        Move(new Vector2(isFacingRight ? 1 : -1, 0));
    }

    void ChasePlayer()
    {
        // Move towards player
        isChasingPlayer = true;
        if (transform.position.x < player.position.x)
        {
            Move(new Vector2(1, 0));
        }
        else
        {
            Move(new Vector2(-1, 0));
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw patrol and attack range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    private void Flip(Vector2 direction)
    {
        if (direction.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
            
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
           
        }
    }
    #region 
    ////+=====OVERRIDE METHODS=====+////
    public override void Move(Vector2 direction)
    {
         rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        // Flip character sprite if moving left or right
        Flip(direction);
        animator.SetBool("isWalking", direction.x != 0);
    }
    public override void Attack()
    {
        // Attack player when in range
        if (canAttack)
        {
            animator.SetTrigger("attack");
            
            canAttack = false;
            StartCoroutine(ResetAttackCooldown());
        }
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        isChasingPlayer = false;
    }

    public override IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    #endregion
}