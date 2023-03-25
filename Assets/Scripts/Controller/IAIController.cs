using System;
using RPG.Resources;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Add this line to require a Rigidbody2D component
public class IAIController : MonoBehaviour
{
    public enum EnemyStates
    {
        NONE,
        IDLE,
        RUN,
        ATTACK,
        PATROL,
        CHASE
    }
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody2D rb2D; // Add this line to store a reference to the Rigidbody2D component
    
    public virtual void FixedUpdate()
    {
        
    }
    public virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>(); // Add this line to get a reference to the Rigidbody2D component
    }

    public virtual void Update()
    {
        
    }

    public virtual void Patrol()
    {
        
    }

    public virtual void Flip()
    {
        
    }

    public virtual void StopAndLookAround()
    {
        
    }

    public virtual void ChasePlayer()
    {
        
    }

    public virtual void AttackPlayer()
    {
        
    }

    public virtual bool CanSeePlayer()
    {
       return false;
    }
    public virtual void LookAround(){}

    ///---ANIMETION HANDLER---//'
    public virtual void HandleIdle()
    {
    }

    public virtual void HandleHurt()
    {
    }

    public virtual void HandleDeath()
    {
    }

    public virtual void HandleWallSlide()
    {
    }
    public virtual void HandleJump()
    {
    }
    public virtual void HandleAttack()
    {
    }
    
    public virtual void HandleBlock()
    {
    }
    public virtual void HandleSpecial()
    {
    }
    public virtual void HandleDash()
    {
    }
    public virtual void HandleStates()
    {
    }
    public virtual void HandleHit(){}
}
