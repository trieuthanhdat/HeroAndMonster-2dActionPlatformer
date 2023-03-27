using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;

public class BossLade : MonoBehaviour
{
    public float damageRate = 0.5f;
    private HeroKnight player;
    float timer =0;
    BoxCollider2D ladeCollider;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.GetComponent<HeroKnight>();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player = null;
        }
    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        ladeCollider = GetComponent<BoxCollider2D>();
    }
   /// <summary>
   /// Update is called every frame, if the MonoBehaviour is enabled.
   /// </summary>
   private void Update()
   {
        if(!player) return;
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = damageRate;
            HitPlayer();
        }
        
      
        
   }
   //ANIM EVENTS
   public void FinishCasting()
   {
        Destroy(gameObject);
   }

    private void HitPlayer()
    {
        player.GetComponent<Health>().TakeDamage(2, player.gameObject, player.HeroAnimator);
    }
}
