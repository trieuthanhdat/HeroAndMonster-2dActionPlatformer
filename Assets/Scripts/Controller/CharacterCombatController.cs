using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterCombatController : MonoBehaviour
{
    
    public Rigidbody2D rb;
    public float hitDamage = 10f;
    public Animator animator;

    
    public float attackCooldown = 0.5f;
    public int maxCombo = 3; // Maximum number of combo attacks allowed
    public float comboTime = 0.5f; // Time limit between attacks to perform a combo
    public bool isAttacking = false; // Flag to check if the character is attacking
    protected int comboCount = 0; // Current combo count
    protected float lastAttackTime = 0f; // Time of the last attack
    protected bool canCombo = false; // Flag to check if the character can perform a combo
    public virtual void Start()
    {
    }
    public virtual void Update()
    {
    }
    public virtual void Attack()
    {
    }

    public virtual IEnumerator ResetCombo()
    {
        yield break;
    }

    public virtual IEnumerator ResetAttackCooldown()
    {
        yield break;
    }
    
    public virtual void Move(Vector2 direction)
    {
    }
    public virtual void Hit()
    {
    }
    protected virtual void Die()
    {
        // Die and remove character from game
        animator.SetTrigger("death");
        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 2f);
    }
}

