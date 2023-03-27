using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEvent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            MonoAudioManager.instance.PlaySound("BossTheme", true, true);
            MonoAudioManager.instance.StopSound("GameplayBG", true);
        }
    }
}
