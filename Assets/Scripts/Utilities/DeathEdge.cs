using System.Collections;
using System.Collections.Generic;
using RPG.Resources;
using UnityEngine;
using Cinemachine;

public class DeathEdge : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other) 
   {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(other.GetComponent<Health>().GetHealthPoint(), other.gameObject);
            CinemachineVirtualCameraBase cam = Camera.main.GetComponent<CinemachineVirtualCameraBase>();
            cam.Follow = null;
        }
   }
}
