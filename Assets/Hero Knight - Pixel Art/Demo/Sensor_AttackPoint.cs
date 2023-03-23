using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class Sensor_AttackPoint : MonoBehaviour
{
    public HeroKnight heroKnight;
    private bool canHit = false;
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        canHit = true;
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        canHit = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
         Debug.Log("ONCOLLISION ATTACK: ");
        if(other.gameObject.CompareTag("Enemy") && canHit)
        {
            other.gameObject.GetComponent<Health>().TakeDamage(heroKnight.gameObject, heroKnight.damagePoint);
            Debug.Log("ATTACK: enemy "+ other.gameObject.name + " damage Point "+heroKnight.damagePoint);
            canHit = false;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy") && canHit)
        {
            other.gameObject.GetComponent<Health>().TakeDamage(heroKnight.gameObject, heroKnight.damagePoint);
            canHit = false;
        }
    }

   
   
}
