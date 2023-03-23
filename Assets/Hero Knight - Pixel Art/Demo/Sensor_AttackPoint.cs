using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class Sensor_AttackPoint : MonoBehaviour
{
    public HeroKnight heroKnight;
    public int forPointAttack = -1;

    private bool canHit = false;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth && !heroKnight.enemies.Contains(enemyHealth))
            {
                heroKnight.enemies.Add(enemyHealth);
                canHit = true;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth)
            {
                heroKnight.enemies.Remove(enemyHealth);
            }
            canHit = heroKnight.enemies.Count > 0;
        }
    }
   
   
}
