using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class BubbleProjectile : MonoBehaviour
{
    public float  attackDamage =3;

   
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HeroKnight>().HandleHurt(attackDamage);
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {

            return;    
        }
        
    }
}
