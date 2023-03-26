using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtons : MonoBehaviour
{
        public enum ButtonType
        {
            SKILL,
            DASH
        }
        public ButtonType buttonType;
        [SerializeField] HeroKnight player = null ;
        [SerializeField] RectTransform foreGround = null;

       /// <summary>
       /// Update is called every frame, if the MonoBehaviour is enabled.
       /// </summary>
       private void Update()
       {
           
           switch(buttonType)
           {
                case ButtonType.SKILL:
                    foreGround.GetComponent<Image>().fillAmount = player.CalculateSkillCoolDownTimeFaction();
                    break;
                case ButtonType.DASH:
                    foreGround.GetComponent<Image>().fillAmount = player.CalculateDashCoolDownTimeFaction();
                break;
           }
        
       }
}
