using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : CharacterCombatController
{
    private PlatformerMotor2D _motor;
    public override void Start()
    {
        _motor = GetComponent<PlatformerMotor2D>();
    }
    public override void Update()
    {
        if(isAttacking)
        {
        }
    }
    public override void Attack()
    {
        HandlePreAttack();
        if (!isAttacking)
        {
            Debug.Log("COMBAT: Attack step 1 combo " +comboCount);
            isAttacking = true;
            animator.SetTrigger("attack"+comboCount); // Play the attack animation based on the combo count
            
            if (comboCount == 0)
            {
                comboCount++;
                 Debug.Log("COMBAT: Attack step 2 combo " +comboCount);
                lastAttackTime = Time.time;
                StartCoroutine(ResetCombo()); // Reset combo after a certain time
            }
            else if (comboCount < maxCombo && canCombo && Time.time - lastAttackTime < comboTime)
            {
                comboCount++;
                lastAttackTime = Time.time;
                StartCoroutine(ResetCombo()); // Reset combo after a certain time
            }
            else
            {
                comboCount = 0;
                StartCoroutine(ResetAttackCooldown()); // Reset attack cooldown after a certain time
            }
        }
    }

    private void HandlePreAttack()
    {
        _motor.ChangeState(PlatformerMotor2D.MotorState.Attacking);
    }

    public override IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        comboCount = 0;
        isAttacking = false;
    }
    public override IEnumerator ResetCombo()
    {
        canCombo = false;
        yield return new WaitForSeconds(comboTime);
        canCombo = true;
        comboCount = 0;
        isAttacking = false;
    }
    public override void Move(Vector2 direction)
    {
        ///This is controlled by PlayerController2d
        return;
    }


}
