using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.Resources
{
    public class BarOfHealth : MonoBehaviour
    {
        [SerializeField] Health health = null ;
        [SerializeField] RectTransform foreGround = null;
        [SerializeField] Canvas barOfHealthCanvas = null;

        public bool isGlobal = false;
       /// <summary>
       /// Update is called every frame, if the MonoBehaviour is enabled.
       /// </summary>
       private void Update()
       {
           if(Mathf.Approximately(health.CalculateFraction(), 1)|| Mathf.Approximately(health.CalculateFraction(), 0))
           {
                if(!isGlobal)
                    barOfHealthCanvas.enabled = false;
               return;
           }
           if(!isGlobal)
           {
                barOfHealthCanvas.enabled = true;
                foreGround.localScale = new Vector3(health.CalculateFraction(), foreGround.localScale.y);
           }
           else 
           {
                foreGround.GetComponent<Image>().fillAmount = health.CalculateFraction();
           }
        
       }

    }

}
