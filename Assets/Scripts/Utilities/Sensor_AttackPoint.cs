using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class Sensor_AttackPoint : MonoBehaviour
{
    public HeroKnight heroKnight;
    public IAIController enemy;
    public int forPointAttack = -1;
    public string targetTag = "Enemy";

    private bool canHit = false;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if(heroKnight && !enemy)
            {
                if (enemyHealth && !heroKnight.enemies.Contains(enemyHealth))
                {
                    heroKnight.enemies.Add(enemyHealth);
                    canHit = true;
                }
            }
            else if(!heroKnight && enemy)
            {
                if(enemyHealth)
                    enemy.playerTarget = enemyHealth;
                canHit = true;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if(heroKnight && !enemy)
            {
                if (enemyHealth)
                {
                    heroKnight.enemies.Remove(enemyHealth);
                    canHit = heroKnight.enemies.Count > 0;
                }
            }
            else if(!heroKnight && enemy)
            {
                if (enemyHealth)
                    enemy.playerTarget = null;
                canHit = false;
            }
        }
    }
   
   
}
