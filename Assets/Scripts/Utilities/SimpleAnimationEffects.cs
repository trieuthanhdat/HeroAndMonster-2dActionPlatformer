using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationEffects : MonoBehaviour
{
    public enum AnimationEffectType
    {
        NONE,
        FLASH,
        FADEIN,
        FADEOUT,
        ZOOMOUT,
        ZOOMIN
    }
    public AnimationEffectType type;
    public bool PlayOnAwake = false;
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        if(PlayOnAwake)
           HandleEffect(type);
    }

    public void HandleEffect(AnimationEffectType type)
    {
        if(!_animator) return;
        switch(type)
        {
            case AnimationEffectType.FLASH:
            _animator.SetTrigger("Flash");
                break;
            case AnimationEffectType.FADEIN:
            _animator.SetTrigger("FadeIn");
                break;
            case AnimationEffectType.FADEOUT:
            _animator.SetTrigger("FadeOut");
                break;
            case AnimationEffectType.ZOOMIN:
            _animator.SetTrigger("ZoomIn");
                break;
            case AnimationEffectType.ZOOMOUT:
            _animator.SetTrigger("ZoomOut");
                break;
        }
    }
}
